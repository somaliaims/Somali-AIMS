using AIMS.APIs.IATILib.Parsers;
using AIMS.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public ScheduleTask(IServiceScopeFactory serviceScopeFactory, IConfiguration config) : base(serviceScopeFactory)
        {
            configuration = config;
        }

        protected override string Schedule => "*/1 * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            // Create an XmlNamespaceManager to resolve namespaces.
            /*NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("iati-extra", "");
            // Create an XmlParserContext.  The XmlParserContext contains all the information
            // required to parse the XML fragment, including the entity information and the
            // XmlNamespaceManager to use for namespace resolution.
            XmlParserContext xmlParserContext = new XmlParserContext(nameTable, nsmgr, null, XmlSpace.None);
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.NameTable = nameTable;
            string url = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=" + countryCode;*/
            //XmlReader xReader = XmlReader.Create(url, xmlReaderSettings, xmlParserContext);
            string url = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=SOM";
            XmlReader xReader = XmlReader.Create(url);
            XDocument xDoc = XDocument.Load(xReader);
            var activity = (from el in xDoc.Descendants("iati-activity")
                            select el.FirstAttribute).FirstOrDefault();

            IParser parser;
            ICollection<IATIActivity> activityList = new List<IATIActivity>();
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
            Debug.WriteLine("IATI Parsed");
            return Task.CompletedTask;
        }
    }
}
