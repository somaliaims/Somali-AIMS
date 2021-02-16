using AIMS.DAL.EF;
using AIMS.IATILib.Parsers;
using AIMS.Models;
using AIMS.Services;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AIMS.APIs.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        IConfiguration configuration;
        IWebHostEnvironment hostingEnvironment;
        private readonly IServiceScopeFactory scopeFactory;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory, IConfiguration config,
            IServiceScopeFactory _scopeFactory, IWebHostEnvironment _hostingEnvironment) : base(serviceScopeFactory)
        {
            configuration = config;
            scopeFactory = _scopeFactory;
            hostingEnvironment = _hostingEnvironment;
        }

        //"*/10 * * * *"; 10 mins
        //protected override string Schedule => "*/2 * * * *";
        protected override string Schedule => "1 0 * * *"; //Expression represents 1 minute past every night

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            string url = configuration.GetValue<string>("IATI:Url");
            string countryCode = configuration.GetValue<string>("IATI:Country");
            string currencyUrl = configuration.GetValue<string>("IATI:CurrencyUrl");
            string transactionTypesUrl = configuration.GetValue<string>("IATI:TransactionTypesUrl");
            string financeTypesUrl = configuration.GetValue<string>("IATI:FinanceTypesUrl");
            string sectorVocabularyUrl = configuration.GetValue<string>("IATI:SectorsVocabulary");
            string organizationTypesUrl = configuration.GetValue<string>("IATI:OrganizationTypesUrl");
            string countriesUrl = configuration.GetValue<string>("IATI:CountriesUrl");
            string sectorsUrl = configuration.GetValue<string>("IATI:SectorsUrl");
            string filePath = Path.Combine(sWebRootFolder, "IATISomali.xml");
            string sectorsFilePath = Path.Combine(sWebRootFolder, "Sectors");
            string organizationTypesPath = Path.Combine(sWebRootFolder, "OrganizationTypes.json");
            string currenciesFilePath = Path.Combine(sWebRootFolder, "Currency.json");
            string countriesFilePath = Path.Combine(sWebRootFolder, "Country.json");
            string transactionTypesPath = Path.Combine(sWebRootFolder, "IATITransactionTypes.json");
            string financeTypesPath = Path.Combine(sWebRootFolder, "IATIFinanceTypes.json");
            string sectorsVocabPath = Path.Combine(sWebRootFolder, "IATISectorVocabulary.json");
            string xml = "", sectorsXml = "", json = "", transactionTypesJson = "", financeTypesJson = "", sectorsVocabJson = "",
                organizationTypesJson = "", errorsFile = "errors.txt", errorsFilePath = "";
            //, countriesJson = ""

            using (var scope = scopeFactory.CreateScope())
            {
                bool isIATIDownloading = false;
                HttpClient httpClient = new HttpClient();
                IWebHostEnvironment hostingEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                ExchangeRateHttpService httpService = new ExchangeRateHttpService(httpClient);
                AIMSDbContext dbContext = scope.ServiceProvider.GetRequiredService<AIMSDbContext>();
                IMapper imapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                IUserService userService = new UserService(dbContext, imapper);
                INotificationService notificationService = new NotificationService(dbContext, imapper);
                IATIService service = new IATIService(dbContext);
                IFinancialYearSettingsService fySettingsService = new FinancialYearSettingsService(dbContext);
                ISectorTypesService sectorTypeService = new SectorTypesService(dbContext, imapper);
                IProjectService projectService = new ProjectService(dbContext, imapper);
                IOrganizationMergeService orgMergeService = new OrganizationMergeService(dbContext);
                IDataBackupService backupService = new DataBackupService(dbContext);
                IContactMessageService contactService = new ContactMessageService(dbContext, imapper);
                IEmailService emailService = new EmailService(dbContext);
                IProjectDeletionService projectDeletionService = new ProjectDeletionService(dbContext, imapper);
                backupService.SetDirectoryPath(hostingEnvironment.WebRootPath);
                IFinancialYearTransitionService financialYearTransitionService = new FinancialYearTransitionService(dbContext);

                try
                {
                    using (var client = new WebClient())
                    {
                        json = client.DownloadString(currencyUrl);
                    }

                    using (var client = new WebClient())
                    {
                        organizationTypesJson = client.DownloadString(organizationTypesUrl);
                    }

                    using (var client = new WebClient())
                    {
                        transactionTypesJson = client.DownloadString(transactionTypesUrl);
                    }

                    using (var client = new WebClient())
                    {
                        financeTypesJson = client.DownloadString(financeTypesUrl);
                    }

                    using (var client = new WebClient())
                    {
                        sectorsVocabJson = client.DownloadString(sectorVocabularyUrl);
                    }

                    //Save sectors to db
                    var iatiSettings = service.GetIATISettings();
                    if (iatiSettings != null)
                    {
                        if (!string.IsNullOrEmpty(iatiSettings.BaseUrl))
                        {
                            url = iatiSettings.BaseUrl;
                            isIATIDownloading = iatiSettings.IsDownloading;
                        }
                    }

                    if (!isIATIDownloading)
                    {
                        //Download latest iati
                        using (var client = new WebClient())
                        {
                            service.SetIATIDownloading();
                            xml = client.DownloadString(url);
                        }
                        File.WriteAllText(filePath, xml);
                        service.SetIATIDownloaded();
                    }

                    var sectorTypesSources = sectorTypeService.GetSectorSources();
                    if (sectorTypesSources.Count() > 0)
                    {
                        using (var client = new WebClient())
                        {
                            int fileCounter = 1;
                            foreach (var stype in sectorTypesSources)
                            {
                                if (!string.IsNullOrEmpty(stype.SourceUrl))
                                {
                                    sectorsXml = client.DownloadString(stype.SourceUrl);
                                    int counter = (stype.IATICode == null) ? fileCounter : Convert.ToInt32(stype.IATICode);
                                    var newFilePath = sectorsFilePath + counter + ".xml";
                                    File.WriteAllText(newFilePath, sectorsXml);
                                    stype.FilePath = newFilePath;
                                }
                                ++fileCounter;
                            }
                        }
                    }

                    var cleanedTTypeJson = service.ExtractTransactionTypesJson(transactionTypesJson);
                    var cleanedFTypeJson = service.ExtractFinanceTypesJson(financeTypesJson);
                    var cleanedSectorVocabJson = service.ExtractSectorsVocabJson(sectorsVocabJson);
                    var cleanedOrgTypesVocabJson = service.ExtractOrganizationsVocabJson(organizationTypesJson);
                    File.WriteAllText(transactionTypesPath, cleanedTTypeJson);
                    File.WriteAllText(financeTypesPath, cleanedFTypeJson);
                    File.WriteAllText(sectorsVocabPath, cleanedSectorVocabJson);
                    File.WriteAllText(organizationTypesPath, cleanedOrgTypesVocabJson);

                    userService.SetNotificationsForUsers();
                    var sectorResponse = service.ExtractAndSaveIATISectors(filePath, sectorsVocabPath);
                    if (sectorTypesSources.Count() > 0)
                    {
                        foreach (var stype in sectorTypesSources)
                        {
                            if (!string.IsNullOrEmpty(stype.FilePath))
                            {
                                service.NameSectorsCorrectly(stype.FilePath, stype.Id);
                            }
                        }
                    }

                    service.ExtractAndSaveLocations(filePath);
                    var orgResponse = service.ExtractAndSaveOrganizations(filePath, cleanedOrgTypesVocabJson);
                    notificationService.SendNotificationsForNewSectors(sectorResponse.ReturnedId);

                    var currencyList = httpService.ParseAndExtractCurrencyList(json);
                    if (currencyList.Count > 0)
                    {
                        ICurrencyService currencyService = new CurrencyService(dbContext, imapper);
                        currencyService.AddMultiple(currencyList);
                    }

                    var fySettings = fySettingsService.Get();
                    int fyMonth = 1, fyDay = 1;
                    int currentMonth = DateTime.Now.Month, currentDay = DateTime.Now.Day;
                    if (fySettings != null)
                    {
                        fyMonth = fySettings.Month;
                        fyDay = fySettings.Day;
                    }

                    var financialTransitionForYear = financialYearTransitionService.IsFinancialTransitionApplied();
                    if ((fyMonth == currentMonth && fyDay == currentDay) || financialTransitionForYear.Exists == false)
                    {
                        int year = 0;
                        if (!financialTransitionForYear.Exists)
                        {
                            year = financialTransitionForYear.Year;
                            var backup = backupService.BackupData(connectionString);
                            var response = projectService.AdjustDisbursementsForProjectsAsync(null, year, true).GetAwaiter().GetResult();
                            if (response.Success)
                            {
                            }
                        }
                    }

                    var pendingOrgMergeRequests = orgMergeService.GetTwoWeeksOlderRequests();
                    if (pendingOrgMergeRequests.Any())
                    {
                        var requests = (from r in pendingOrgMergeRequests
                                        select r.RequestId).ToList();
                        orgMergeService.MergeOrganizationsAuto(requests).GetAwaiter().GetResult();
                    }
                    var pendingContactMessages = contactService.GetUnRepliedMessages();
                    foreach (var message in pendingContactMessages)
                    {
                        EmailModel emailModel = new EmailModel()
                        {
                            EmailsList = new List<EmailAddress>() { new EmailAddress() { Email = message.SenderEmail } },
                            Subject = message.Subject,
                            Message = message.Message,
                            FooterMessage = null,
                        };
                        emailService.SendEmailForPendingMessages(emailModel, message.SenderName, message.SenderEmail, message.ProjectTitle);
                    }
                    contactService.SetMessagesNotifiedAsync().GetAwaiter().GetResult();

                    projectDeletionService.SendPendingDeletionRequestsToManagement();

                    //File cleanup
                    string excelFiles = Path.Combine(sWebRootFolder, "ExcelFiles");
                    var directory = Directory.CreateDirectory(excelFiles);

                    if (directory.GetFiles().Any())
                    {
                        string[] files = Directory.GetFiles(excelFiles);

                        if (files.Length > 0)
                        {
                            foreach (string file in files)
                            {
                                FileInfo fi = new FileInfo(file);
                                if (fi.LastAccessTime < DateTime.Now.AddMinutes(-120))
                                {
                                    fi.Delete();
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    if (!isIATIDownloading)
                    {
                        service.SetIATIDownloaded();
                    }
                    errorsFilePath = Path.Combine(sWebRootFolder, errorsFile);
                    if (!File.Exists(errorsFilePath))
                    {
                        File.Create(errorsFilePath);
                        FileIOPermission fp = new FileIOPermission(FileIOPermissionAccess.AllAccess, errorsFilePath);
                    }
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMessage = ex.InnerException.Message;
                    }
                    File.AppendAllText(errorsFilePath, errorMessage);
                }
            }

            return Task.CompletedTask;
        }
    }
}
