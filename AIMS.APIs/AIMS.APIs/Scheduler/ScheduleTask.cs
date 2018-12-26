using AIMS.APIs.IATILib.Parsers;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AIMS.APIs.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        IConfiguration configuration;
        private readonly IServiceScopeFactory scopeFactory;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory, IConfiguration config, 
            IServiceScopeFactory _scopeFactory) : base(serviceScopeFactory)
        {
            configuration = config;
            scopeFactory = _scopeFactory;
        }

        protected override string Schedule => "*/1 * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            try
            {
                string message = "IATI updated successfully at: " + DateTime.Now.ToLongDateString();
                string country = configuration.GetValue<string>("IATI:Country");
                string url = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=" + country;
                XmlReader xReader = XmlReader.Create(url);
                XDocument xDoc = XDocument.Load(xReader);
                var activity = (from el in xDoc.Descendants("iati-activity")
                                select el.FirstAttribute).FirstOrDefault();

                IParser parser;
                ICollection<IATIActivity> activityList = new List<IATIActivity>();
                ICollection<Organization> organizations = new List<Organization>();
                string version = "";
                version = activity.Value;
                switch (version)
                {
                    case "1.03":
                        parser = new ParserIATIVersion13();
                        activityList = parser.ExtractAcitivities(xDoc);
                        break;

                    case "2.01":
                        parser = new ParserIATIVersion21(configuration);
                        activityList = parser.ExtractAcitivities(xDoc);
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
                                        organizations.Add(new Organization()
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
