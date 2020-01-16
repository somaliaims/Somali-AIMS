using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.IATILib.Parsers
{
    public class ParserIATIVersion13 : IParser
    {
        public ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc, string criteria, List<IATITransactionTypes> transactionTypes = null, List<IATIFinanceTypes> financeTypes = null)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title") != null && activity.Element("title").Value.Contains(criteria)
                             select activity;

            this.ExtractAndFillActivities(activities, activityList);
            return activityList;
        }

        public ICollection<IATIActivity> ExtractAcitivitiesForIds(XDocument xmlDoc, IEnumerable<string> Ids, List<IATITransactionTypes> transactionTypes = null, List<IATIFinanceTypes> financeTypes = null)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title") != null &&
                             Ids.Contains(activity.Element("iati-identifier").Value)
                             select activity;

            this.ExtractAndFillActivities(activities, activityList);
            return activityList;
        }

        public ICollection<SourceSectorModel> ExtractSectorsFromSource(XDocument xmlDoc)
        {
            List<SourceSectorModel> sectorsList = new List<SourceSectorModel>();
            return sectorsList;
        }

        public ICollection<IATIProject> ExtractProjects(XDocument xmlDoc)
        {
            List<IATIProject> projectsList = new List<IATIProject>();
            string message = "";
            try
            {
                var activities = from activity in xmlDoc.Descendants("iati-activity")
                                 where activity.Element("title") != null
                                 select activity;

                string currency = "";
                int activityCounter = 1;
                foreach (var activity in activities)
                {
                    string startDate = "", endDate = "", projectTitle = "";
                    currency = activity.Attribute("default-currency").Value;
                    projectTitle = activity.Element("title")?.Value;

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

                    projectsList.Add(new IATIProject()
                    {
                        Id = activityCounter,
                        IATIIdentifier = activity.Element("iati-identifier")?.Value,
                        Title = projectTitle,
                        Description = activity.Element("description")?.Value,
                        DefaultCurrency = currency,
                        StartDate = startDate,
                        EndDate = endDate
                    });
                    ++activityCounter;
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }
            return projectsList;
        }

        public ICollection<IATISectorModel> ExtractSectors(XDocument xmlDoc)
        {
            List<IATISectorModel> sectorsList = new List<IATISectorModel>();
            string message = "";
            try
            {
                var activities = from activity in xmlDoc.Descendants("iati-activity")
                                 where activity.Element("title") != null
                                 select activity;

                foreach (var activity in activities)
                {
                    //Extracting Sectors
                    var aSectors = activity.Elements("sector");
                    List<IATISectorModel> sectors = new List<IATISectorModel>();
                    var sectorPercentage = (100 / aSectors.Count());
                    foreach (var sector in aSectors)
                    {
                        sectors.Add(new IATISectorModel()
                        {
                            SectorName = sector.Value,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return sectorsList;
        }

        public ICollection<IATILocation> ExtractLocations(XDocument xmlDoc)
        {
            List<IATILocation> locationsList = new List<IATILocation>();
            string message = "";
            try
            {
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return locationsList;
        }

        public ICollection<IATIOrganizationModel> ExtractOrganizations(XDocument xmlDoc)
        {
            List<IATIOrganizationModel> organizationsList = new List<IATIOrganizationModel>();
            string message = "";
            try
            {
                var activities = from activity in xmlDoc.Descendants("iati-activity")
                                 where activity.Element("title") != null
                                 select activity;

                foreach (var activity in activities)
                {
                    var organizations = activity.Elements("participating-org");
                    foreach (var organization in organizations)
                    {
                        string orgName = organization?.Value;
                        string isOrgExists = (from org in organizationsList
                                              where org.Name.Trim().ToLower() == orgName.Trim().ToLower()
                                              select org.Name).FirstOrDefault();

                        if (isOrgExists == null && !string.IsNullOrEmpty(orgName))
                        {
                            organizationsList.Add(new IATIOrganizationModel()
                            {
                                Name = organization?.Value,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return organizationsList;
        }

        private void ExtractAndFillActivities(IEnumerable<XElement> activities, List<IATIActivity> activityList)
        {
            string currency = "";
            int transactionCounter = 1;
            foreach (var activity in activities)
            {
                string startDate, endDate, projectTitle = "";
                currency = activity.Attribute("default-currency").Value;
                projectTitle = activity.Element("title")?.Value;

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
                List<IATIOrganization> organizationList = new List<IATIOrganization>();
                foreach (var organization in organizations)
                {
                    organizationList.Add(new IATIOrganization()
                    {
                        Project = projectTitle,
                        Name = organization?.Value,
                        Role = organization.Attribute("role")?.Value
                    });
                }

                //Extracting transactions
                var transactions = activity.Elements("transaction");
                List<IATITransaction> transactionsList = new List<IATITransaction>();
                foreach (var transaction in transactions)
                {
                    transactionsList.Add(new IATITransaction()
                    {
                        Id = transactionCounter,
                        Amount = transaction.Element("value")?.Value,
                        Currency = transaction.Element("value")?.FirstAttribute.Value,
                        Dated = transaction.Element("transaction-date")?.Attribute("iso-date").Value,
                        //AidType = transaction.Element("aid-type")?.Value,
                        TransactionType = transaction.Element("transaction-type")?.Value,
                        //Description = transaction.Element("description")?.Value
                    });
                    ++transactionCounter;
                }

                //Extracting Receipient Countries
                //decimal percentage = 100;
                /*var recipientCountries = activity.Elements("recipient-country");
                List<IATICountry> countries = new List<IATICountry>();
                if (recipientCountries.Count() > 1)
                {
                    percentage = (100 / recipientCountries.Count());
                }

                foreach (var country in recipientCountries)
                {
                    countries.Add(new IATICountry()
                    {
                        Code = country.Attribute("code")?.Value,
                        ContributionPercentage = percentage.ToString()
                    });
                }*/

                //Extracting Receipient Regions
                /*var recipientRegions = activity.Elements("recipient-region");
                List<IATIRegion> regions = new List<IATIRegion>();
                decimal regionPercentage = 100;
                if (recipientRegions.Count() > 1)
                {
                    regionPercentage = (100 / recipientRegions.Count());
                }

                foreach (var region in recipientRegions)
                {
                    regions.Add(new IATIRegion()
                    {
                        Code = region.Attribute("code")?.Value,
                        ContributionPercentage = regionPercentage.ToString()
                    });
                }*/

                //Extracting Sectors
                var aSectors = activity.Elements("sector");
                List<IATISector> sectors = new List<IATISector>();
                var sectorPercentage = (100 / aSectors.Count());
                foreach (var sector in aSectors)
                {
                    sectors.Add(new IATISector()
                    {
                        Code = sector.Value,
                        FundsPercentage = sectorPercentage.ToString()
                    });
                }

                var funders = (from f in organizationList
                               where f.Role == "Funding"
                               select f).ToList<IATIOrganization>();
                var implementers = (from i in organizationList
                                    where i.Role == "Implementing"
                                    select i).ToList<IATIOrganization>();

                activityList.Add(new IATIActivity()
                {
                    Identifier = activity.Element("iati-identifier")?.Value,
                    Title = projectTitle,
                    Description = activity.Element("description")?.Value,
                    Sectors = sectors,
                    DefaultCurrency = currency,
                    Transactions = transactionsList,
                    Funders = funders,
                    Implementers = implementers
                });
            }
        }
    }
}
