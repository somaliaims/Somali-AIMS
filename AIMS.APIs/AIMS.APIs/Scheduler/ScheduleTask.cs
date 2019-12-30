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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AIMS.APIs.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        IConfiguration configuration;
        IHostingEnvironment hostingEnvironment;
        private readonly IServiceScopeFactory scopeFactory;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory, IConfiguration config, 
            IServiceScopeFactory _scopeFactory, IHostingEnvironment _hostingEnvironment) : base(serviceScopeFactory)
        {
            configuration = config;
            scopeFactory = _scopeFactory;
            hostingEnvironment = _hostingEnvironment;
        }

        //"*/10 * * * *"; 10 mins
        //protected override string Schedule => "*/2 * * * *";
        protected override string Schedule => "1 0 * * *"; //Expression represents 1 minutes past every night

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            try
            {
                string sWebRootFolder = hostingEnvironment.WebRootPath;
                string url = configuration.GetValue<string>("IATI:Url");
                string countryCode = configuration.GetValue<string>("IATI:Country");
                string currencyUrl = configuration.GetValue<string>("IATI:CurrencyUrl");
                string transactionTypesUrl = configuration.GetValue<string>("IATI:TransactionTypesUrl");
                string financeTypesUrl = configuration.GetValue<string>("IATI:FinanceTypesUrl");
                string sectorVocabularyUrl = configuration.GetValue<string>("IATI:SectorsVocabulary");
                string organizationTypesUrl = configuration.GetValue<string>("IATI:OrganizationTypesUrl");
                string countriesUrl = configuration.GetValue<string>("IATI:CountriesUrl");
                string filePath = sWebRootFolder + "/IATISomali.xml";
                string organizationTypesPath = sWebRootFolder + "/OrganizationTypes.json";
                string currenciesFilePath = sWebRootFolder + "/Currency.json";
                string countriesFilePath = sWebRootFolder + "/Country.json";
                string transactionTypesPath = sWebRootFolder + "/IATITransactionTypes.json";
                string financeTypesPath = sWebRootFolder + "/IATIFinanceTypes.json";
                string sectorsVocabPath = sWebRootFolder + "/IATISectorVocabulary.json";
                string xml = "", json = "", transactionTypesJson = "", financeTypesJson = "", sectorsVocabJson = "",
                    organizationTypesJson = "", countriesJson = "";

          

                using (var client = new WebClient())
                {
                    json = client.DownloadString(currencyUrl);
                }

                using (var client = new WebClient())
                {
                    countriesJson = client.DownloadString(countriesUrl);
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
                using (var scope = scopeFactory.CreateScope())
                {
                    HttpClient httpClient = new HttpClient();
                    ExchangeRateHttpService httpService = new ExchangeRateHttpService(httpClient);

                    AIMSDbContext dbContext = scope.ServiceProvider.GetRequiredService<AIMSDbContext>();
                    IMapper imapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    IUserService userService = new UserService(dbContext, imapper);
                    INotificationService notificationService = new NotificationService(dbContext, imapper);
                    IATIService service = new IATIService(dbContext);
                    ICountryService countryService = new CountryService(dbContext, imapper);

                    var activeCountryCode = countryService.GetActiveCountry();
                    if (!string.IsNullOrEmpty(activeCountryCode))
                    {
                        string[] urlParts = url.Split("?");
                        if (urlParts.Length > 1)
                        {
                            url = urlParts[0] + "?recipient-country=" + activeCountryCode + "&stream=true";
                        }
                    }
                    //Download latest iati
                    using (var client = new WebClient())
                    {
                        xml = client.DownloadString(url);
                    }
                    File.WriteAllText(filePath, xml);

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
                    service.ExtractAndSaveLocations(filePath);
                    //Oldservice.ExtractAndSaveOrganizationTypes(cleanedOrgTypesVocabJson);
                    var orgResponse = service.ExtractAndSaveOrganizations(filePath, cleanedOrgTypesVocabJson);
                    notificationService.SendNotificationsForNewOrganizations(orgResponse.ReturnedId, Convert.ToInt32(orgResponse.Message));
                    notificationService.SendNotificationsForNewSectors(sectorResponse.ReturnedId);

                    var currencyList = httpService.ParseAndExtractCurrencyList(json);
                    if (currencyList.Count > 0)
                    {
                        ICurrencyService currencyService = new CurrencyService(dbContext, imapper);
                        currencyService.AddMultiple(currencyList);
                    }

                    var countriesList = service.ExtractCountriesList(countriesJson);
                    if (countriesList.Count > 0)
                    {
                        countryService.AddList(countriesList);
                    }

                }

                //File cleanup
                string excelFiles = sWebRootFolder + "\\ExcelFiles";
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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
