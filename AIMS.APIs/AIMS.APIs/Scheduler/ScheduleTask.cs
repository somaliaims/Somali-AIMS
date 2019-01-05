using AIMS.DAL.EF;
using AIMS.IATILib.Parsers;
using AIMS.Models;
using AIMS.Services;
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

        protected override string Schedule => "*/20 * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            try
            {
                string sWebRootFolder = hostingEnvironment.WebRootPath;
                string url = configuration.GetValue<string>("IATI:Url");
                string fileToWrite = sWebRootFolder + "/IATISomali.xml";
                string xml = "";

                using (var client = new WebClient())
                {
                    xml = client.DownloadString(url);
                }
                File.WriteAllText(fileToWrite, xml);
                /*string country = configuration.GetValue<string>("IATI:Url");
                string url = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=" + country + "&stream=true";
                XmlReader xReader = XmlReader.Create(url);
                XDocument xDoc = XDocument.Load(xReader);
                var activity = (from el in xDoc.Descendants("iati-activity")
                                select el.FirstAttribute).FirstOrDefault();

                IParser parser;
                ICollection<IATIActivity> activityList = new List<IATIActivity>();
                ICollection<IATIOrganization> organizations = new List<IATIOrganization>();
                string version = "";
                version = activity.Value;
                switch (version)
                {
                    case "1.03":
                        parser = new ParserIATIVersion13();
                        activityList = parser.ExtractAcitivities(xDoc, "");
                        break;

                    case "2.01":
                        parser = new ParserIATIVersion21();
                        activityList = parser.ExtractAcitivities(xDoc, "");
                        break;
                }

                //Extract organizations for future use
                if (activityList.Count > 0)
                {
                    if (activityList != null)
                    {
                        var organizationList = from a in activityList
                                               select a.ParticipatingOrganizations;

                        if (organizationList.Count() > 0)
                        {
                            foreach (var orgCollection in organizationList)
                            {
                                var orgList = from list in orgCollection
                                              select list;

                                foreach (var org in orgList)
                                {
                                    var orgExists = (from o in organizations
                                                     where o.Name.ToLower().Equals(org.Name.ToLower())
                                                     select o).FirstOrDefault();

                                    if (orgExists == null)
                                    {
                                        organizations.Add(new IATIOrganization()
                                        {
                                            Project = org.Project,
                                            Name = org.Name,
                                            Role = org.Role
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                Debug.WriteLine("IATI Data updated at: " + DateTime.Now.ToLongDateString());
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AIMSDbContext>();
                    IATIService service = new IATIService(dbContext);
                    IATIModel model = new IATIModel()
                    {
                        Data = JsonConvert.SerializeObject(activityList),
                        Organizations = JsonConvert.SerializeObject(organizations)
                    };
                    service.Add(model);
                    Debug.WriteLine(message);
                }*/

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
