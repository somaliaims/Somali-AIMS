using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.IATILib.Parsers
{
    public class ParserIATIVersion21 : IParser
    {
        public ParserIATIVersion21()
        {
        }

        public ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc, string criteria, List<IATITransactionTypes> transactionTypes = null)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            //Pick up all narratives
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null &&
                             activity.Element("title").Element("narrative").Value.Contains(criteria, StringComparison.OrdinalIgnoreCase)
                             select activity;
            this.ParseIATIAndFillList(activities, activityList, transactionTypes);

            //Pick up all titles
            var titleActivities = from activity in xmlDoc.Descendants("iati-activity")
                                  where activity.Element("title") != null &&
                                  activity.Element("title").Value.Contains(criteria, StringComparison.OrdinalIgnoreCase)
                                  select activity;
            this.ParseIATIAndFillList(titleActivities, activityList, transactionTypes);
            return activityList;
        }

        public ICollection<IATIActivity> ExtractAcitivitiesForIds(XDocument xmlDoc, IEnumerable<string> Ids, List<IATITransactionTypes> transactionTypes = null)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            //Pick up all narratives
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null &&
                             Ids.Contains(activity.Element("iati-identifier").Value)
                             select activity;
            this.ParseIATIAndFillList(activities, activityList, transactionTypes);
            return activityList;
        }

        public ICollection<IATIProject> ExtractProjects(XDocument xmlDoc)
        {
            List<IATIProject> projectsList = new List<IATIProject>();
            //Pick up all narratives
            var activities = (from activity in xmlDoc.Descendants("iati-activity")
                              where activity.Element("title").Element("narrative") != null ||
                              activity.Element("title") != null
                              select activity);

            this.ParseAndFillProjects(activities, projectsList);
            return projectsList;
        }

        public ICollection<IATISectorModel> ExtractSectors(XDocument xmlDoc)
        {
            List<IATISectorModel> sectorsList = new List<IATISectorModel>();
            //Pick up all narratives
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null ||
                             activity.Element("title") != null
                             select activity;

            this.ParseAndFillSectors(activities, sectorsList);
            return sectorsList;
        }

        public ICollection<IATIOrganizationModel> ExtractOrganizations(XDocument xmlDoc)
        {
            List<IATIOrganizationModel> organizationsList = new List<IATIOrganizationModel>();
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null ||
                             activity.Element("title") != null
                             select activity;

            this.ParseAndFillOrganizations(activities, organizationsList);
            return organizationsList;
        }

        private void ParseIATIAndFillList(IEnumerable<XElement> activities, List<IATIActivity> activityList, List<IATITransactionTypes> transactionTypes)
        {
            string message = "";
            try
            {
                string currency = "";
                if (activities != null)
                {
                    int activityCounter = 1;
                    int orgCounter = 1;
                    int documentCounter = 1;
                    int transactionCounter = 1;
                    int budgetCounter = 1;
                    int disbursementCounter = 1;
                    foreach (var activity in activities)
                    {
                        string startDate = "", startPlanned = "", endDate = "", endPlanned = "", projectTitle = "";
                        if (activity.HasAttributes)
                        {
                            if (activity.Attribute("default-currency") != null)
                            {
                                currency = activity.Attribute("default-currency").Value;
                            }

                            if (activity.Element("title") != null && activity.Element("title").Element("narrative") != null)
                            {
                                projectTitle = activity.Element("title").Element("narrative")?.Value;
                            }
                        }

                        //Extracting dates
                        var dates = activity.Elements("activity-date");
                        if (dates != null)
                        {
                            foreach (var date in dates)
                            {
                                if (date.HasAttributes && date.Attribute("type") != null)
                                {
                                    if (date.Attribute("type").Value.Equals("1"))
                                    {
                                        startPlanned = date.Attribute("iso-date")?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("2"))
                                    {
                                        startDate = date.Attribute("iso-date")?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("3"))
                                    {
                                        endPlanned = date.Attribute("iso-date")?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("4"))
                                    {
                                        endDate = date.Attribute("iso-date")?.Value;
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(startDate))
                            {
                                startDate = startPlanned;
                            }
                            if (string.IsNullOrEmpty(endDate))
                            {
                                endDate = endPlanned;
                            }
                        }

                        //Extracting participating organizations
                        var organizations = activity.Elements("participating-org");
                        List<IATIOrganization> organizationList = new List<IATIOrganization>();

                        if (organizations != null)
                        {
                            foreach (var organization in organizations)
                            {
                                OrganizationRole role;

                                if (organization.HasAttributes && organization.Attribute("role") != null)
                                {
                                    role = (OrganizationRole)Enum.Parse(typeof(OrganizationRole), organization.Attribute("role").Value);
                                    var narratives = organization.Elements("narrative");
                                    string organizationName = "";

                                    if (narratives.Count() > 0)
                                    {
                                        if (narratives.FirstOrDefault().HasAttributes)
                                        {
                                            organizationName = (from n in narratives
                                                                where n.FirstAttribute.Value == "en"
                                                                select n.Value).FirstOrDefault();
                                        }
                                        else
                                        {
                                            if (organization.HasElements && organization.Element("narrative") != null)
                                            {
                                                organizationName = organization.Element("narrative")?.Value;
                                            }
                                        }
                                    }

                                    organizationList.Add(new IATIOrganization()
                                    {
                                        Id = orgCounter,
                                        Project = projectTitle,
                                        Name = organizationName,
                                        Role = role.ToString()
                                    });
                                    ++orgCounter;
                                }
                            }
                        }

                        //Extracting documents
                        var documents = activity.Elements("document-link");
                        List<IATIDocument> documentsList = new List<IATIDocument>();

                        if (documents != null)
                        {
                            foreach (var document in documents)
                            {
                                string url = "";
                                string title = "";
                                if (document.HasAttributes)
                                {
                                    url = document.Attribute("url")?.Value;
                                }
                                if (document.Element("title") != null && document.Element("title").Element("narrative") != null)
                                {
                                    title = document.Element("title").Element("narrative")?.Value;
                                }

                                if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(url))
                                {
                                    documentsList.Add(new IATIDocument()
                                    {
                                        Id = documentCounter,
                                        DocumentTitle = title,
                                        DocumentUrl = url
                                    });
                                    ++documentCounter;
                                }
                            }
                        }

                        //Extracting transactions
                        var transactions = activity.Elements("transaction");
                        List<IATITransaction> transactionsList = new List<IATITransaction>();

                        if (transactions != null)
                        {
                            foreach (var transaction in transactions)
                            {

                                string transactionCode = transaction.Element("transaction-type").Attribute("code")?.Value;
                                string transactionType = (from t in transactionTypes
                                                          where t.Code.Equals(transactionCode)
                                                          select t.Name).FirstOrDefault();

                                transactionsList.Add(new IATITransaction()
                                {
                                    Id = transactionCounter,
                                    Amount = transaction.Element("value")?.Value,
                                    Currency = transaction.Element("value")?.Attribute("currency")?.Value,
                                    Dated = transaction.Element("transaction-date")?.Attribute("iso-date")?.Value,
                                    TransactionType = transactionType,
                                });
                                ++transactionCounter;
                            }
                        }

                        //Extracting budgets
                        var budgets = activity.Elements("budget");
                        List<IATIBudget> budgetsList = new List<IATIBudget>();
                        if (budgets != null)
                        {
                            foreach (var budget in budgets)
                            {
                                string budgetStartDate = budget.Element("period-start").Attribute("iso-date")?.Value;
                                string budgetEndDate = budget.Element("period-end").Attribute("iso-date")?.Value;
                                string budgetAmountStr = budget.Element("value")?.Value;
                                string budgetCurrency = budget.Element("value").Attribute("currency")?.Value;
                                if (string.IsNullOrEmpty(budgetCurrency))
                                {
                                    budgetCurrency = currency;
                                }

                                decimal budgetAmount = 0;
                                Decimal.TryParse(budgetAmountStr, out budgetAmount);

                                budgetsList.Add(new IATIBudget()
                                {
                                    Id = budgetCounter,
                                    StartDate = budgetStartDate,
                                    EndDate = budgetEndDate,
                                    Currency = budgetCurrency,
                                    Amount = budgetAmount
                                });
                                ++budgetCounter;
                            }
                        }

                        //Extracting budgets
                        var disbursements = activity.Elements("planned-disbursement");
                        List<IATIDisbursement> disbursementsList = new List<IATIDisbursement>();
                        if (disbursements != null)
                        {
                            foreach (var disbursement in disbursements)
                            {
                                string disbursementStartDate = disbursement.Element("period-start").Attribute("iso-date")?.Value;
                                string disbursementEndDate = disbursement.Element("period-end").Attribute("iso-date")?.Value;
                                string disbursementAmountStr = disbursement.Element("value")?.Value;
                                string disbursementCurrency = disbursement.Element("value").Attribute("currency")?.Value;
                                if (string.IsNullOrEmpty(disbursementCurrency))
                                {
                                    disbursementCurrency = currency;
                                }

                                decimal disbursementAmount = 0;
                                Decimal.TryParse(disbursementAmountStr, out disbursementAmount);

                                disbursementsList.Add(new IATIDisbursement()
                                {
                                    Id = disbursementCounter,
                                    StartDate = disbursementStartDate,
                                    EndDate = disbursementEndDate,
                                    Currency = disbursementCurrency,
                                    Amount = disbursementAmount
                                });
                                ++budgetCounter;
                            }
                        }


                        var recipientCountries = activity.Elements("recipient-country");
                        List<IATICountry> countries = new List<IATICountry>();
                        if (recipientCountries != null)
                        {
                            foreach (var country in recipientCountries)
                            {
                                countries.Add(new IATICountry()
                                {
                                    Code = country.Attribute("code")?.Value,
                                    ContributionPercentage = country.Attribute("percentage")?.Value
                                });
                            }
                        }

                        //Extracting Receipient Regions
                        var recipientRegions = activity.Elements("recipient-region");
                        List<IATIRegion> regions = new List<IATIRegion>();

                        if (recipientRegions != null)
                        {
                            foreach (var region in recipientRegions)
                            {
                                regions.Add(new IATIRegion()
                                {
                                    Code = region.Attribute("code")?.Value,
                                    ContributionPercentage = region.Attribute("percentage")?.Value
                                });
                            }
                        }

                        var aSectors = activity.Elements("sector");
                        List<IATISector> sectors = new List<IATISector>();
                        if (aSectors != null)
                        {
                            foreach (var sector in aSectors)
                            {
                                string sectorName = "";
                                var setorNarrative = sector.Element("narrative");
                                if (setorNarrative != null)
                                {
                                    sectorName = sector.Element("narrative").Value;
                                }
                                sectors.Add(new IATISector()
                                {
                                    Code = sector.Attribute("code")?.Value,
                                    SectorName = sectorName,
                                    FundsPercentage = sector.Attribute("percentage")?.Value
                                });
                            }
                        }

                        var aLocations = activity.Elements("location");
                        List<IATILocation> locations = new List<IATILocation>();
                        if (aLocations != null)
                        {
                            foreach (var location in aLocations)
                            {
                                string locationName = "", latitude = "", longitude = "";
                                XElement nameElement = (from name in location.Descendants("name")
                                                        select name).FirstOrDefault();

                                if (nameElement != null)
                                {
                                    XElement narrative = (from narr in nameElement.Descendants("narrative")
                                                          select narr).FirstOrDefault();

                                    if (narrative != null)
                                    {
                                        locationName = narrative?.Value;
                                    }
                                    else
                                    {
                                        locationName = nameElement?.Value;
                                    }
                                }

                                XElement locationPoint = (from point in location.Descendants("point")
                                                          select point).FirstOrDefault();

                                XElement locationCoordinates = (from coordinate in location.Descendants("coordinates")
                                                                select coordinate).FirstOrDefault();

                                if (locationPoint != null)
                                {
                                    var position = locationPoint.Element("pos")?.Value;
                                    if (position != null)
                                    {
                                        string[] arr = position.Split();
                                        latitude = arr[0];
                                        longitude = arr[1];
                                    }
                                }

                                if (locationCoordinates != null)
                                {
                                    if (locationCoordinates.HasAttributes)
                                    {
                                        if (locationCoordinates.Attribute("latitude") != null)
                                            latitude = locationCoordinates.Attribute("latitude")?.Value;

                                        if (locationCoordinates.Attribute("longitude") != null)
                                            longitude = locationCoordinates.Attribute("longitude")?.Value;
                                    }
                                }

                                locations.Add(new IATILocation()
                                {
                                    Name = locationName,
                                    Latitude = latitude,
                                    Longitude = longitude
                                });
                            }
                        }

                        string trimmedTitle = Regex.Replace(projectTitle, @"\s+", " ");
                        var isActivityAdded = (from a in activityList
                                               where a.TrimmedTitle.Contains(trimmedTitle, StringComparison.OrdinalIgnoreCase)
                                               select a).FirstOrDefault();

                        if (isActivityAdded == null)
                        {

                            activityList.Add(new IATIActivity()
                            {
                                Id = activityCounter,
                                Identifier = activity.Element("iati-identifier")?.Value,
                                Title = projectTitle,
                                StartDate = string.IsNullOrEmpty(startDate) ? startDate : Convert.ToDateTime(startDate).ToLongDateString(),
                                EndDate = string.IsNullOrEmpty(endDate) ? endDate : Convert.ToDateTime(endDate).ToLongDateString(),
                                TrimmedTitle = trimmedTitle,
                                Locations = locations,
                                Documents = documentsList,
                                Description = activity.Element("description")?.Value,
                                Budgets = budgetsList,
                                Disbursements = disbursementsList,
                                Sectors = sectors,
                                DefaultCurrency = currency,
                                Transactions = transactionsList,
                                ParticipatingOrganizations = organizationList
                            });
                            ++activityCounter;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void ParseAndFillProjects(IEnumerable<XElement> activities, List<IATIProject> projectsList)
        {
            string message = "";
            try
            {
                string currency = "";
                if (activities != null)
                {
                    int activityCounter = 1;
                    foreach (var activity in activities)
                    {
                        string startDate = "", startPlanned = "", endDate = "", endPlanned = "", projectTitle = "";
                        if (activity.HasAttributes)
                        {
                            if (activity.Attribute("default-currency") != null)
                            {
                                currency = activity.Attribute("default-currency").Value;
                            }

                            if (activity.Element("title") != null && activity.Element("title").Element("narrative") != null)
                            {
                                projectTitle = activity.Element("title").Element("narrative")?.Value;
                            }
                        }

                        //Extracting dates
                        var dates = activity.Elements("activity-date");
                        if (dates != null)
                        {
                            foreach (var date in dates)
                            {
                                if (date.HasAttributes && date.Attribute("type") != null)
                                {
                                    if (date.Attribute("type").Value.Equals("1"))
                                    {
                                        startPlanned = date.FirstAttribute?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("2"))
                                    {
                                        startDate = date.FirstAttribute?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("3"))
                                    {
                                        endPlanned = date.FirstAttribute?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("2"))
                                    {
                                        endDate = date.FirstAttribute?.Value;
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(startDate))
                            {
                                startDate = startPlanned;
                            }
                            if (string.IsNullOrEmpty(endDate))
                            {
                                endDate = endPlanned;
                            }
                        }

                        if (string.IsNullOrEmpty(startDate))
                        {
                            startDate = "N/A";
                        }

                        string trimmedTitle = Regex.Replace(projectTitle, @"\s+", " ");
                        var isProjectAdded = (from p in projectsList
                                              where p.TrimmedTitle.Contains(trimmedTitle, StringComparison.OrdinalIgnoreCase)
                                              select p).FirstOrDefault();

                        if (isProjectAdded == null)
                        {
                            projectsList.Add(new IATIProject()
                            {
                                Id = activityCounter,
                                DefaultCurrency = currency,
                                IATIIdentifier = activity.Element("iati-identifier")?.Value,
                                Title = projectTitle,
                                TrimmedTitle = trimmedTitle,
                                Description = activity.Element("description")?.Value,
                                StartDate = string.IsNullOrEmpty(startDate) ? startDate : Convert.ToDateTime(startDate).ToLongDateString(),
                                EndDate = string.IsNullOrEmpty(endDate) ? endDate : Convert.ToDateTime(endDate).ToLongDateString()
                            });
                            ++activityCounter;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void ParseAndFillSectors(IEnumerable<XElement> activities, List<IATISectorModel> sectorsList)
        {
            string message = "";
            try
            {
                if (activities != null)
                {
                    foreach (var activity in activities)
                    {
                        var aSectors = activity.Elements("sector");
                        if (aSectors != null)
                        {
                            foreach (var sector in aSectors)
                            {
                                string sectorName = "";
                                var setorNarrative = sector.Element("narrative");
                                if (setorNarrative != null)
                                {
                                    sectorName = sector.Element("narrative").Value;
                                }
                                sectorsList.Add(new IATISectorModel()
                                {
                                    SectorName = sectorName,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void ParseAndFillOrganizations(IEnumerable<XElement> activities, List<IATIOrganizationModel> organizationsList)
        {
            string message = "";
            try
            {
                if (activities != null)
                {
                    foreach (var activity in activities)
                    {
                        var organizations = activity.Elements("participating-org");
                        if (organizations != null)
                        {
                            foreach (var organization in organizations)
                            {
                                if (organization.HasAttributes && organization.Attribute("role") != null)
                                {
                                    var narratives = organization.Elements("narrative");
                                    string organizationName = "";

                                    if (narratives.Count() > 0)
                                    {
                                        if (narratives.FirstOrDefault().HasAttributes)
                                        {
                                            organizationName = (from n in narratives
                                                                where n.FirstAttribute.Value == "en"
                                                                select n.Value).FirstOrDefault();
                                        }
                                        else
                                        {
                                            if (organization.HasElements && organization.Element("narrative") != null)
                                            {
                                                organizationName = organization.Element("narrative")?.Value;
                                            }
                                        }
                                    }

                                    organizationsList.Add(new IATIOrganizationModel()
                                    {
                                        Name = organizationName,
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        /*public List<AidTypes> FillAidTypes(IEnumerable<IConfigurationSection> dataArray)
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
        }*/


    }
}
