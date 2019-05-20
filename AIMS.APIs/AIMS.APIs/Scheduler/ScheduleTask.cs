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
        protected override string Schedule => "1 0 * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            try
            {
                string sWebRootFolder = hostingEnvironment.WebRootPath;
                string url = configuration.GetValue<string>("IATI:Url");
                string currencyUrl = configuration.GetValue<string>("IATI:CurrencyUrl");
                string transactionTypesUrl = configuration.GetValue<string>("IATI:TransactionTypesUrl");
                string filePath = sWebRootFolder + "/IATISomali.xml";
                string currenciesFilePath = sWebRootFolder + "/Currency.json";
                string transactionTypesPath = sWebRootFolder + "/TransactionTypes.json";
                string xml = "";
                string json = "", transactionTypesJson = "";

                using (var client = new WebClient())
                {
                    xml = client.DownloadString(url);
                }
                File.WriteAllText(filePath, xml);

                using (var client = new WebClient())
                {
                    json = client.DownloadString(currencyUrl);
                }
                //File.WriteAllText(currenciesFilePath, json);

                using (var client = new WebClient())
                {
                    transactionTypesJson = client.DownloadString(transactionTypesUrl);
                }

                //Save sectors to db
                using (var scope = scopeFactory.CreateScope())
                {
                    HttpClient httpClient = new HttpClient();
                    ExchangeRateHttpService httpService = new ExchangeRateHttpService(httpClient);
                    //var currencyList = httpService.GetCurrencyWithNames().GetAwaiter().GetResult();

                    AIMSDbContext dbContext = scope.ServiceProvider.GetRequiredService<AIMSDbContext>();
                    IMapper imapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    UserService userService = new UserService(dbContext, imapper);
                    IATIService service = new IATIService(dbContext);
                    userService.SetNotificationsForUsers();
                    service.ExtractAndSaveDAC5Sectors(filePath);
                    service.ExtractAndSaveLocations(filePath);
                    service.ExtractAndSaveOrganizations(filePath);

                    var currencyList = httpService.ParseAndExtractCurrencyList(json);
                    if (currencyList.Count > 0)
                    {
                        ICurrencyService currencyService = new CurrencyService(dbContext, imapper);
                        currencyService.AddMultiple(currencyList);
                    }
                    
                }

                //File cleanup
                string excelFiles = sWebRootFolder + "/ExcelSheets";
                if (Directory.Exists(excelFiles))
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
