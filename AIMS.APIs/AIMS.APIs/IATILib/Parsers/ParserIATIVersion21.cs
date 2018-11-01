using AIMS.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.APIs.IATILib.Parsers
{
    public class ParserIATIVersion21 : IParser
    {
        IConfiguration configuration;
        ICollection<AidTypes> aidTypes;
        ICollection<TransactionTypes> transactionTypes;
        public ParserIATIVersion21(IConfiguration config)
        {
            this.configuration = config;
            var aidData = configuration.GetSection("AidTypes").GetChildren();
            var transactionData = configuration.GetSection("TransactionTypes").GetChildren();
            aidTypes = this.FillAidTypes(aidData);
            transactionTypes = this.FillTransactionTypes(transactionData);
        }

        public ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             select activity;

            string currency = "";
            foreach (var activity in activities)
            {
                string startDate, endDate = "";
                currency = activity.Attribute("default-currency").Value;

                //Extracting dates
                var dates = activity.Elements("activity-date");
                foreach (var date in dates)
                {
                    if (date.Attribute("type").Value.Equals("start-actual"))
                    {
                        startDate = date.FirstAttribute?.Value;
                    }
                    else if (date.Attribute("type").Value.Equals("end-planned"))
                    {
                        endDate = date.FirstAttribute?.Value;
                    }
                }

                //Extracting participating organizations
                var organizations = activity.Elements("participating-org");
                List<Organization> organizationList = new List<Organization>();
                foreach (var organization in organizations)
                {
                    var role = (OrganizationRole)Enum.Parse(typeof(OrganizationRole), organization.Attribute("role").Value);
                    var narratives = organization.Elements("narrative");
                    string organizationName = "";

                    if (narratives.Count() > 0)
                    {
                        if (narratives.FirstOrDefault().HasAttributes)
                        {
                            organizationName = (from n in narratives
                                                where n.FirstAttribute.Value == "en"
                                                select n.FirstAttribute.Value).FirstOrDefault();
                        }
                        else
                        {
                            organizationName = organization.Element("narrative")?.Value;
                        }
                    }
                    
                    organizationList.Add(new Organization()
                    {
                        Name = organizationName,
                        Role = role.ToString()
                    });
                }

                string aidType = "";
                var aidTypeObj = activity.Element("default-aid-type");
                if (aidTypeObj != null && aidTypeObj.HasAttributes)
                {
                    string aidTypeCode = aidTypeObj.Attribute("code")?.Value;
                    aidType = (from t in aidTypes
                               where t.Code.Equals(aidTypeCode)
                               select t.Name).FirstOrDefault();
                }

                //Extracting transactions
                var transactions = activity.Elements("transaction");
                List<IATITransaction> transactionsList = new List<IATITransaction>();
                foreach (var transaction in transactions)
                {
                    
                    string transactionCode = transaction.Element("transaction-type").Attribute("code")?.Value;
                    string transactionType = (from t in transactionTypes
                                              where t.Code.Equals(transactionCode)
                                              select t.Name).FirstOrDefault();

                    transactionsList.Add(new IATITransaction()
                    {
                        Amount = transaction.Element("value")?.Value,
                        Currency = transaction.Element("value")?.FirstAttribute.Value,
                        Dated = transaction.Element("transaction-date")?.Attribute("iso-date").Value,
                        AidType = aidType,
                        TransactionType = transactionType,
                        Description = transaction.Element("description")?.Value
                    });
                }

                var recipientCountries = activity.Elements("recipient-country");
                List<Country> countries = new List<Country>();
                foreach(var country in recipientCountries)
                {
                    countries.Add(new Country()
                    {
                        Code = country.Attribute("code")?.Value,
                        ContributionPercentage = country.Attribute("percentage")?.Value
                    });
                }

                //Extracting Receipient Regions
                var recipientRegions = activity.Elements("recipient-region");
                List<Region> regions = new List<Region>();
                foreach (var region in recipientRegions)
                {
                    regions.Add(new Region()
                    {
                        Code = region.Attribute("code")?.Value,
                        ContributionPercentage = region.Attribute("percentage")?.Value 
                    });
                }

                var aSectors = activity.Elements("sector");
                List<Sector> sectors = new List<Sector>();
                foreach (var sector in aSectors)
                {
                    sectors.Add(new Sector()
                    {
                        Code = sector.Attribute("code")?.Value,
                        FundPercentage = sector.Attribute("percentage")?.Value
                    });
                }

                activityList.Add(new IATIActivity()
                {
                    Identifier = activity.Element("iati-identifier")?.Value,
                    Title = activity.Element("title")?.Value,
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

        public List<AidTypes> FillAidTypes(IEnumerable<IConfigurationSection> dataArray)
        {
            List<AidTypes> list = new List<AidTypes>();
            foreach (var data in dataArray)
            {
                list.Add(new AidTypes()
                {
                    Code = data.GetValue<string>("Code"),
                    Name = data.GetValue<string>("Name")
                });
            }
            return list;
        }

        public List<TransactionTypes> FillTransactionTypes(IEnumerable<IConfigurationSection> dataArray)
        {
            List<TransactionTypes> list = new List<TransactionTypes>();
            foreach (var data in dataArray)
            {
                list.Add(new TransactionTypes()
                {
                    Code = data.GetValue<string>("Code"),
                    Name = data.GetValue<string>("Name")
                });
            }
            return list;
        }

    }
}
