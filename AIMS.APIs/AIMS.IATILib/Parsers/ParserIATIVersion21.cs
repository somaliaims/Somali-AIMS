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

        public ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc, string criteria, List<IATITransactionTypes> transactionTypes = null, List<IATIFinanceTypes> financeTypes = null)
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

        public ICollection<IATIActivity> ExtractAcitivitiesForIds(XDocument xmlDoc, IEnumerable<string> Ids, List<IATITransactionTypes> transactionTypes = null, List<IATIFinanceTypes> financeTypes = null)
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
                              where activity.Element("title").Element("narrative") != null
                              select activity);

            this.ParseAndFillProjects(activities, projectsList);

            activities = (from activity in xmlDoc.Descendants("iati-activity")
                          where activity.Element("title") != null
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

        public ICollection<IATILocation> ExtractLocations(XDocument xmlDoc)
        {
            List<IATILocation> locationsList = new List<IATILocation>();
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null ||
                             activity.Element("title") != null
                             select activity;

            this.ParseAndFillLocations(activities, locationsList);
            return locationsList;
        }

        private void ParseIATIAndFillList(IEnumerable<XElement> activities, List<IATIActivity> activityList, List<IATITransactionTypes> transactionTypes = null, List<IATIFinanceTypes> financeTypes = null)
        {
            string message = "";
            try
            {
                DateTime todaysDate = DateTime.Now;
                DateTime parsedDate = DateTime.Now;
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
                        string startDate = "", startPlanned = "", endDate = "", endPlanned = "", projectTitle = "", defaultFinanceType = "";

                        /*var activityStatus = activity.Element("activity-status");
                        if (activityStatus.Attribute("code") != null)
                        {
                            int activityStatusVal = 0;
                            if (int.TryParse(activityStatus.Attribute("code")?.Value, out activityStatusVal))
                            {
                                if (activityStatusVal == 4 || activityStatusVal == 5)
                                {
                                    continue;
                                }
                            }
                        }*/

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

                        /*if (!string.IsNullOrEmpty(endDate))
                        {
                            if (DateTime.TryParse(endDate, out parsedDate))
                            {
                                TimeSpan timeSpan = todaysDate - parsedDate;
                                if (timeSpan.Days > 365)
                                {
                                    continue;
                                }
                            }
                        }*/

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

                        var financeType = activity.Element("default-finance-type");
                        if (financeType != null)
                        {
                            var financeCode = financeType.Attribute("Code")?.Value;
                            if (!string.IsNullOrEmpty(financeCode))
                            {
                                if (financeTypes != null)
                                {
                                    defaultFinanceType = (from f in financeTypes
                                                          where f.Code.Equals(financeCode)
                                                          select f.Name).FirstOrDefault();
                                }
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
                                                organizationName = organizationName != null ? organizationName.Trim() : organizationName;
                                            }
                                        }
                                    }

                                    var isOrganizationExists = (from o in organizationList
                                                                where o.Name.ToLower() == organizationName.ToLower()
                                                                select o).FirstOrDefault();

                                    if (isOrganizationExists == null && !string.IsNullOrEmpty(organizationName))
                                    {
                                        organizationList.Add(new IATIOrganization()
                                        {
                                            Id = orgCounter,
                                            Project = projectTitle,
                                            Name = organizationName,
                                            Role = role.ToString()
                                        });
                                    }
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
                        List<IATIFundingTransaction> fundingTransactions = new List<IATIFundingTransaction>();
                        List<IATIDisbursementTransaction> disbursementTransactions = new List<IATIDisbursementTransaction>();

                        if (transactions != null)
                        {
                            foreach (var transaction in transactions)
                            {
                                FinanceDisplayType financeDisplayType = 0;
                                string transactionCode = transaction.Element("transaction-type").Attribute("code")?.Value;
                                if (transactionCode == "2" || transactionCode == "11")
                                {
                                    financeDisplayType = FinanceDisplayType.Funding;
                                }
                                else
                                {
                                    financeDisplayType = FinanceDisplayType.Disbursement;
                                }

                                string transactionType = (from t in transactionTypes
                                                          where t.Code.Equals(transactionCode)
                                                          select t.Name).FirstOrDefault();

                                var isTransactionExists = (from t in transactionsList
                                                           where currency == t.Currency
                                                           && t.TransactionType == transactionType
                                                           select t).FirstOrDefault();


                                if (isTransactionExists != null)
                                {
                                    string newValue = transaction.Element("value")?.Value;
                                    decimal amount = isTransactionExists.Amount != null ? Convert.ToDecimal(isTransactionExists.Amount) : 0;
                                    decimal newAmount = newValue != null ? Convert.ToDecimal(newValue) : 0;
                                    isTransactionExists.Amount = (amount + newAmount).ToString();

                                    if (financeDisplayType == FinanceDisplayType.Funding)
                                    {
                                        var isFundingExists = (from f in fundingTransactions
                                                               where currency == f.Currency
                                                               select f).FirstOrDefault();

                                        if (isFundingExists != null)
                                        {
                                            isFundingExists.Amount = (amount + newAmount).ToString();
                                        }
                                    }

                                    if (financeDisplayType == FinanceDisplayType.Disbursement)
                                    {
                                        var isDisbursementExists = (from f in disbursementTransactions
                                                                    where currency == f.Currency
                                                                    select f).FirstOrDefault();

                                        if (isDisbursementExists != null)
                                        {
                                            isDisbursementExists.Amount = (amount + newAmount).ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    transactionsList.Add(new IATITransaction()
                                    {
                                        Id = transactionCounter,
                                        Amount = transaction.Element("value")?.Value,
                                        Currency = transaction.Element("value")?.Attribute("currency")?.Value,
                                        Dated = transaction.Element("transaction-date")?.Attribute("iso-date")?.Value,
                                        TransactionType = transactionType,
                                        FinanceType = financeDisplayType
                                    });
                                    ++transactionCounter;

                                    if (financeDisplayType == FinanceDisplayType.Funding)
                                    {
                                        fundingTransactions.Add(new IATIFundingTransaction()
                                        {
                                            Id = transactionCounter,
                                            Amount = transaction.Element("value")?.Value,
                                            Currency = transaction.Element("value")?.Attribute("currency")?.Value,
                                            Dated = transaction.Element("transaction-date")?.Attribute("iso-date")?.Value,
                                            TransactionType = transactionType,
                                        });
                                    }
                                    else if (financeDisplayType == FinanceDisplayType.Disbursement)
                                    {
                                        disbursementTransactions.Add(new IATIDisbursementTransaction()
                                        {
                                            Id = transactionCounter,
                                            Amount = transaction.Element("value")?.Value,
                                            Currency = transaction.Element("value")?.Attribute("currency")?.Value,
                                            Dated = transaction.Element("transaction-date")?.Attribute("iso-date")?.Value,
                                            TransactionType = transactionType,
                                        });
                                    }
                                }
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

                                var isCurrencyExists = (from b in budgetsList
                                                        where b.Currency == budgetCurrency
                                                        select b).FirstOrDefault();

                                if (isCurrencyExists != null)
                                {
                                    isCurrencyExists.Amount += budgetAmount;
                                    isCurrencyExists.EndDate = budgetEndDate;
                                }
                                else
                                {
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
                                    sectorName = sector.Element("narrative")?.Value;
                                    sectorName = sectorName != null ? sectorName.Trim() : sectorName;
                                }

                                var isSectorExists = (from s in sectors
                                                      where s.SectorName.ToLower() == sectorName.ToLower()
                                                      select s).FirstOrDefault();

                                if (isSectorExists == null && !string.IsNullOrEmpty(sectorName))
                                {
                                    sectors.Add(new IATISector()
                                    {
                                        Code = sector.Attribute("code")?.Value,
                                        SectorName = sectorName,
                                        FundsPercentage = sector.Attribute("percentage")?.Value
                                    });
                                }
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
                                    locationName = locationName != null ? locationName.Trim() : locationName;
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

                                var isLocationExists = (from l in locations
                                                        where l.Name.ToLower() == locationName.ToLower()
                                                        select l).FirstOrDefault();

                                if (isLocationExists == null && !string.IsNullOrEmpty(locationName))
                                {
                                    locations.Add(new IATILocation()
                                    {
                                        Name = locationName,
                                        Latitude = latitude,
                                        Longitude = longitude
                                    });
                                }
                            }
                        }

                        string trimmedTitle = Regex.Replace(projectTitle, @"\s+", " ");
                        var isActivityAdded = (from a in activityList
                                               where a.TrimmedTitle.Contains(trimmedTitle, StringComparison.OrdinalIgnoreCase)
                                               select a).FirstOrDefault();

                        if (isActivityAdded == null)
                        {
                            var funders = (from org in organizationList
                                           where org.Role == "Funding"
                                           select org).ToList<IATIOrganization>();
                            var implementers = (from org in organizationList
                                                where org.Role == "Implementing"
                                                select org).ToList<IATIOrganization>();

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
                                DefaultFinanceType = defaultFinanceType,
                                Transactions = transactionsList,
                                FundingTransactions = fundingTransactions,
                                DisbursementTransactions = disbursementTransactions,
                                Funders = funders,
                                Implementers = implementers
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

        private void ParseAndFillProjects(IEnumerable<XElement> activities, List<IATIProject> projectsList, List<IATIFinanceTypes> financeTypes = null)
        {
            string message = "";
            try
            {
                string currency = "";
                if (activities != null)
                {
                    DateTime todaysDate = DateTime.Now;
                    DateTime parsedDate = DateTime.Now;
                    int activityCounter = 1, orgCounter = 1;
                    foreach (var activity in activities)
                    {
                        string startDate = "", startPlanned = "", endDate = "", endPlanned = "", projectTitle = "", defaultFinanceType = "";

                        var transactions = activity.Elements("transaction");
                        if (transactions != null)
                        {
                            DateTime transactionDate;
                            DateTime anYearOldDate = DateTime.Now.AddYears(-1);
                            bool isIncludeActivity = false;
                            foreach (var transaction in transactions)
                            {
                                if (DateTime.TryParse(transaction.Element("transaction-date")?.Attribute("iso-date")?.Value, out transactionDate))
                                {
                                    if (transactionDate > anYearOldDate)
                                    {
                                        isIncludeActivity = true;
                                        break;
                                    }
                                }
                            }

                            if (!isIncludeActivity)
                            {
                                continue;
                            }
                        }

                        var activityStatus = activity.Element("activity-status");
                        if (activityStatus.Attribute("code") != null)
                        {
                            int activityStatusVal = 0;
                            if (int.TryParse(activityStatus.Attribute("code")?.Value, out activityStatusVal))
                            {
                                if (activityStatusVal == 4 || activityStatusVal == 5)
                                {
                                    continue;
                                }
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

                        if (!string.IsNullOrEmpty(endDate))
                        {
                            if (DateTime.TryParse(endDate, out parsedDate))
                            {
                                TimeSpan timeSpan = todaysDate - parsedDate;
                                if (timeSpan.Days > 365)
                                {
                                    continue;
                                }
                            }
                        }

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

                        string trimmedTitle = Regex.Replace(projectTitle, @"\s+", " ");
                        var isProjectAdded = (from p in projectsList
                                              where p.TrimmedTitle.Contains(trimmedTitle, StringComparison.OrdinalIgnoreCase)
                                              select p).FirstOrDefault();

                        if (isProjectAdded != null)
                        {
                            continue;
                        }

                        var financeType = activity.Element("default-finance-type");
                        if (financeType != null)
                        {
                            var financeCode = financeType.Attribute("Code")?.Value;
                            if (!string.IsNullOrEmpty(financeCode))
                            {
                                if (financeTypes != null)
                                {
                                    defaultFinanceType = (from f in financeTypes
                                                          where f.Code.Equals(financeCode)
                                                          select f.Name).FirstOrDefault();
                                }
                            }
                        }

                        //Extracting participating organizations
                        var organizations = activity.Elements("participating-org");
                        List<IATIOrganizationView> organizationList = new List<IATIOrganizationView>();

                        if (organizations != null)
                        {
                            foreach (var organization in organizations)
                            {

                                if (organization.HasAttributes)
                                {
                                    var narratives = organization.Elements("narrative");
                                    string organizationName = "";

                                    if (narratives != null)
                                    {
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
                                                    organizationName = organizationName != null ? organizationName.Trim() : organizationName;
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(organizationName))
                                        {
                                            var isOrganizationExists = (from o in organizationList
                                                                        where o.Name.ToLower() == organizationName.ToLower()
                                                                        select o).FirstOrDefault();

                                            if (isOrganizationExists == null && !string.IsNullOrEmpty(organizationName))
                                            {
                                                organizationList.Add(new IATIOrganizationView()
                                                {
                                                    Id = orgCounter,
                                                    Name = organizationName,
                                                });
                                            }
                                            ++orgCounter;
                                        }
                                    }
                                }
                            }
                        }

                        var aSectors = activity.Elements("sector");
                        List<IATISectorView> sectors = new List<IATISectorView>();
                        if (aSectors != null)
                        {
                            foreach (var sector in aSectors)
                            {
                                string sectorName = "";
                                var setorNarrative = sector.Element("narrative");
                                if (setorNarrative != null)
                                {
                                    sectorName = sector.Element("narrative")?.Value;
                                    sectorName = sectorName != null ? sectorName.Trim() : sectorName;
                                }

                                var isSectorExists = (from s in sectors
                                                      where s.Name.ToLower() == sectorName.ToLower()
                                                      select s).FirstOrDefault();

                                if (isSectorExists == null && !string.IsNullOrEmpty(sectorName))
                                {
                                    sectors.Add(new IATISectorView()
                                    {
                                        Name = sectorName,
                                    });
                                }
                            }
                        }

                        var aLocations = activity.Elements("location");
                        List<IATILocationView> locations = new List<IATILocationView>();
                        if (aLocations != null)
                        {
                            foreach (var location in aLocations)
                            {
                                string locationName = "";
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
                                    locationName = locationName != null ? locationName.Trim() : locationName;
                                }

                                var isLocationExists = (from l in locations
                                                        where l.Name.ToLower() == locationName.ToLower()
                                                        select l).FirstOrDefault();

                                if (isLocationExists == null && !string.IsNullOrEmpty(locationName))
                                {
                                    locations.Add(new IATILocationView()
                                    {
                                        Name = locationName,
                                    });
                                }
                            }
                        }

                        var validStartDate = new DateTime();
                        var validEndDate = new DateTime();
                        bool isValidStartDate = DateTime.TryParse(startDate, out validStartDate);
                        bool isValidEndDate = DateTime.TryParse(endDate, out validEndDate);

                        projectsList.Add(new IATIProject()
                        {
                            Id = activityCounter,
                            DefaultCurrency = currency,
                            DefaultFinanceType = defaultFinanceType,
                            IATIIdentifier = activity.Element("iati-identifier")?.Value,
                            Title = projectTitle,
                            TrimmedTitle = trimmedTitle,
                            Description = activity.Element("description")?.Value,
                            StartDate = isValidStartDate ? Convert.ToDateTime(startDate).ToShortDateString() : "N/a",
                            EndDate = isValidEndDate ? Convert.ToDateTime(endDate).ToShortDateString() : "N/a",
                            Organizations = organizationList,
                            Locations = locations,
                            Sectors = sectors
                        });
                        ++activityCounter;
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
                                int? sectorTypeVocabulary = null;
                                string sectorName = "";
                                if (sector.Attribute("vocabulary")?.Value != null)
                                {
                                    int sectorVocabCode = 0;
                                    int.TryParse(sector.Attribute("vocabulary")?.Value, out sectorVocabCode);
                                    if (sectorVocabCode != 0)
                                    {
                                        sectorTypeVocabulary = sectorVocabCode;
                                    }
                                }

                                var setorNarrative = sector.Element("narrative");
                                if (setorNarrative != null)
                                {
                                    sectorName = sector.Element("narrative")?.Value;
                                    sectorName = sectorName != null ? sectorName.Trim() : sectorName;
                                }

                                if (!string.IsNullOrEmpty(sectorName) && !string.IsNullOrWhiteSpace(sectorName))
                                {
                                    var isSectorExists = (from s in sectorsList
                                                          where s.SectorName.ToLower() == sectorName.ToLower()
                                                          select s).FirstOrDefault();

                                    if (isSectorExists == null)
                                    {
                                        sectorsList.Add(new IATISectorModel()
                                        {
                                            SectorTypeCode = sectorTypeVocabulary,
                                            SectorName = sectorName,
                                        });
                                    }
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

        private void ParseAndFillLocations(IEnumerable<XElement> activities, List<IATILocation> locaitonsList)
        {
            string message = "";
            try
            {
                if (activities != null)
                {
                    foreach (var activity in activities)
                    {
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
                                    locationName = locationName != null ? locationName.Trim() : locationName;
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

                                var isLocationExists = (from l in locations
                                                        where l.Name.ToLower() == locationName.ToLower()
                                                        select l).FirstOrDefault();

                                if (isLocationExists == null && !string.IsNullOrEmpty(locationName))
                                {
                                    locations.Add(new IATILocation()
                                    {
                                        Name = locationName,
                                        Latitude = latitude,
                                        Longitude = longitude
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
                                                organizationName = organizationName != null ? organizationName.Trim() : organizationName;
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(organizationName) && !string.IsNullOrWhiteSpace(organizationName))
                                    {
                                        var isOrgExists = (from org in organizationsList
                                                           where org.Name.ToLower() == organizationName.ToLower()
                                                           select org).FirstOrDefault();

                                        if (isOrgExists == null)
                                        {
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
