using IATILib.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.APIs.IATILib.Parsers
{
    public class ParserIATIVersion13 : IParser
    {
        public ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             select activity;

            string currency = "";
            foreach(var activity in activities)
            {
                string startDate, endDate, projectTitle = "";
                currency = activity.Attribute("default-currency").Value;
                projectTitle = activity.Element("title")?.Value;

                //Extracting dates
                var dates = activity.Elements("activity-date");
                foreach(var date in dates)
                {
                    if(date.Attribute("type").Value.Equals("start-actual"))
                    {
                        startDate = date.FirstAttribute?.Value;
                    }
                    else if(date.Attribute("type").Value.Equals("end-planned"))
                    {
                        endDate = date.FirstAttribute?.Value;
                    }
                }

                //Extracting participating organizations
                var organizations = activity.Elements("participating-org");
                List<Organization> organizationList = new List<Organization>();
                foreach(var organization in organizations)
                {
                    organizationList.Add(new Organization()
                    {
                        Project = projectTitle,
                        Name = organization?.Value,
                        Role = organization.Attribute("role")?.Value
                    });
                }

                //Extracting transactions
                var transactions = activity.Elements("transaction");
                List<IATITransaction> transactionsList = new List<IATITransaction>();
                foreach(var transaction in transactions)
                {
                    transactionsList.Add(new IATITransaction()
                    {
                        Amount = transaction.Element("value")?.Value,
                        Currency = transaction.Element("value")?.FirstAttribute.Value,
                        Dated = transaction.Element("transaction-date")?.Attribute("iso-date").Value,
                        AidType = transaction.Element("aid-type")?.Value,
                        TransactionType = transaction.Element("transaction-type")?.Value,
                        Description = transaction.Element("description")?.Value
                    });
                }

                //Extracting Receipient Countries
                decimal percentage = 100;
                var recipientCountries = activity.Elements("recipient-country");
                List<Country> countries = new List<Country>();
                if (recipientCountries.Count() > 1)
                {
                    percentage = (100 / recipientCountries.Count());
                }
                    
                foreach(var country in recipientCountries)
                {
                    countries.Add(new Country()
                    {
                        Code = country.Attribute("code")?.Value,
                        ContributionPercentage = percentage.ToString()
                    });
                }

                //Extracting Receipient Regions
                var recipientRegions = activity.Elements("recipient-region");
                List<Region> regions = new List<Region>();
                decimal regionPercentage = 100;
                if (recipientRegions.Count() > 1)
                {
                    regionPercentage = (100 / recipientRegions.Count());
                }
                
                foreach (var region in recipientRegions)
                {
                    regions.Add(new Region()
                    {
                        Code = region.Attribute("code")?.Value,
                        ContributionPercentage = regionPercentage.ToString()
                    });
                }

                //Extracting Sectors
                var aSectors = activity.Elements("sector");
                List<Sector> sectors = new List<Sector>();
                var sectorPercentage = (100 / aSectors.Count());
                foreach (var sector in aSectors)
                {
                    sectors.Add(new Sector()
                    {
                        Code = sector.Value,
                        FundPercentage = sectorPercentage.ToString()
                    });
                }

                activityList.Add(new IATIActivity()
                {
                    Identifier = activity.Element("iati-identifier")?.Value,
                    Title = projectTitle,
                    Countries = countries,
                    Regions = regions,
                    Description = activity.Element("description")?.Value,
                    Sectors = sectors,
                    DefaultCurrency = currency,
                    Transactions = transactionsList,
                    ParticipatingOrganizations = organizationList
                });
            }
            return activityList;
        }
    }
}
