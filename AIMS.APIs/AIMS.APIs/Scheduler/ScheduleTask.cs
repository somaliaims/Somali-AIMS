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
                string filePath = sWebRootFolder + "/IATISomali.xml";
                string xml = "";

                using (var client = new WebClient())
                {
                    xml = client.DownloadString(url);
                }
                File.WriteAllText(filePath, xml);

                //Save sectors to db
                using (var scope = scopeFactory.CreateScope())
                {
                    AIMSDbContext dbContext = scope.ServiceProvider.GetRequiredService<AIMSDbContext>();
                    IATIService service = new IATIService(dbContext);
                    service.ExtractAndSaveDAC5Sectors(filePath);

                    dbContext = scope.ServiceProvider.GetRequiredService<AIMSDbContext>();
                    IMapper imapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    UserService userService = new UserService(dbContext, imapper);
                    userService.SetNotificationsForUsers();
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
