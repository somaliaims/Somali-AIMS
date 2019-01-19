using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.IATILib.Parsers
{
    public class ParserIATIVersion21 : IParser
    {
        public ParserIATIVersion21()
        {
        }

        public ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc, string criteria)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            //Pick up all narratives
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null && 
                             activity.Element("title").Element("narrative").Value.IndexOf(criteria, 0, StringComparison.OrdinalIgnoreCase) >= 0
                             select activity;
            this.ParseIATIAndFillList(activities, activityList);

            //Pick up all titles
            var titleActivities = from activity in xmlDoc.Descendants("iati-activity")
                                  where activity.Element("title") != null &&
                                  activity.Element("title").Value.IndexOf(criteria, 0, StringComparison.OrdinalIgnoreCase) >= 0
                                  select activity;
            this.ParseIATIAndFillList(titleActivities, activityList);
            return activityList;
        }

        public ICollection<IATIActivity> ExtractAcitivitiesForIds(XDocument xmlDoc, IEnumerable<string> Ids)
        {
            List<IATIActivity> activityList = new List<IATIActivity>();
            //Pick up all narratives
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null &&
                             Ids.Contains(activity.Element("iati-identifier").Value)
                             select activity;
            this.ParseIATIAndFillList(activities, activityList);
            return activityList;
        }

        public ICollection<IATIProject> ExtractProjects(XDocument xmlDoc)
        {
            List<IATIProject> projectsList = new List<IATIProject>();
            //Pick up all narratives
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null ||
                             activity.Element("title") != null
                             select activity;

            this.ParseAndFillProjects(activities, projectsList);
            return projectsList;
        }

        private void ParseIATIAndFillList(IEnumerable<XElement> activities, List<IATIActivity> activityList)
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
                        string startDate, endDate, projectTitle = "";
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
                                    if (date.Attribute("type").Value.Equals("start-actual"))
                                    {
                                        startDate = date.FirstAttribute?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("end-planned"))
                                    {
                                        endDate = date.FirstAttribute?.Value;
                                    }
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
                                                                select n.FirstAttribute.Value).FirstOrDefault();
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
                                        Project = projectTitle,
                                        Name = organizationName,
                                        Role = role.ToString()
                                    });
                                }
                            }
                        }

                        string aidType = "";
                        var aidTypeObj = activity.Element("default-aid-type");
                        if (aidTypeObj != null && aidTypeObj.HasAttributes)
                        {
                            string aidTypeCode = aidTypeObj.Attribute("code")?.Value;
                            /*aidType = (from t in aidTypes
                                       where t.Code.Equals(aidTypeCode)
                                       select t.Name).FirstOrDefault();*/
                        }

                        //Extracting documents
                        var documents = activity.Elements("document-link");
                        List<IATIDocument> documentsList = new List<IATIDocument>();

                        if (documents != null)
                        {
                            int dCounter = 1;
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
                                        Id = dCounter,
                                        DocumentTitle = title,
                                        DocumentUrl = url
                                    });
                                    ++dCounter;
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
                                /*string transactionType = (from t in transactionTypes
                                                          where t.Code.Equals(transactionCode)
                                                          select t.Name).FirstOrDefault();*/

                                transactionsList.Add(new IATITransaction()
                                {
                                    Amount = transaction.Element("value")?.Value,
                                    Currency = transaction.Element("value")?.FirstAttribute.Value,
                                    Dated = transaction.Element("transaction-date")?.Attribute("iso-date").Value,
                                    AidType = aidType,
                                    //TransactionType = transactionType,
                                    Description = transaction.Element("description")?.Value
                                });
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

                                    /*if (position != null)
                                    {
                                        if (position.HasAttributes)
                                        {
                                            if (position.Attribute("latitude") != null)
                                                latitude = position.Attribute("latitude")?.Value;

                                            if (position.Attribute("longitude") != null)
                                                longitude = position.Attribute("longitude")?.Value;
                                        }
                                    }*/
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

                        activityList.Add(new IATIActivity()
                        {
                            Id = activityCounter,
                            Identifier = activity.Element("iati-identifier")?.Value,
                            Title = projectTitle,
                            Locations = locations,
                            Countries = countries,
                            Regions = regions,
                            Documents = documentsList,
                            Description = activity.Element("description")?.Value,
                            Sectors = sectors,
                            DefaultCurrency = currency,
                            Transactions = transactionsList,
                            ParticipatingOrganizations = organizationList
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
                        string startDate = "", endDate = "", projectTitle = "";
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
                                    if (date.Attribute("type").Value.Equals("start-actual"))
                                    {
                                        startDate = date.FirstAttribute?.Value;
                                    }
                                    else if (date.Attribute("type").Value.Equals("end-planned"))
                                    {
                                        endDate = date.FirstAttribute?.Value;
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(startDate))
                        {
                            startDate = "N/A";
                        }
                        projectsList.Add(new IATIProject()
                        {
                            Id = activityCounter,
                            DefaultCurrency = currency,
                            IATIIdentifier = activity.Element("iati-identifier")?.Value,
                            Title = projectTitle,
                            Description = activity.Element("description")?.Value,
                            StartDate = startDate,
                            EndDate = endDate
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

        /*public List<TransactionTypes> FillTransactionTypes(IEnumerable<IConfigurationSection> dataArray)
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
        }*/

    }
}
