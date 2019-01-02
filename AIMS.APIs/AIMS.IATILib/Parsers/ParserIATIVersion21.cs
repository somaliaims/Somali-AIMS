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
            var activities = from activity in xmlDoc.Descendants("iati-activity")
                             where activity.Element("title").Element("narrative") != null && 
                             activity.Element("title").Element("narrative").Value.Contains(criteria)
                             select activity;

            string message = "";
            try
            {
                string currency = "";
                if (activities != null)
                {
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
                                sectors.Add(new IATISector()
                                {
                                    Code = sector.Attribute("code")?.Value,
                                    FundPercentage = sector.Attribute("percentage")?.Value
                                });
                            }
                        }

                        var aLocations = activity.Elements("location");
                        List<IATILocation> locations = new List<IATILocation>();
                        if (aLocations != null)
                        {
                            foreach(var location in aLocations)
                            {
                                string locationName = "", latitude = "", longitude = "";
                                var nameElement = location.Element("name");
                                if (nameElement != null)
                                {
                                    if (nameElement.Element("narrative") != null)
                                    {
                                        locationName = nameElement.Element("narrative")?.Value;
                                    }
                                    else
                                    {
                                        locationName = nameElement?.Value;
                                    }
                                }

                                var locationPoint = location.Element("point");
                                var locationCoordinates = location.Element("coordinates");

                                if (locationPoint != null)
                                {
                                    var position = locationPoint.Element("pos");
                                    if (position != null)
                                    {
                                        if (position.HasAttributes)
                                        {
                                            if (position.Attribute("latitude") != null)
                                                latitude = position.Attribute("latitude")?.Value;

                                            if (position.Attribute("longitude") != null)
                                                longitude = position.Attribute("longitude")?.Value;
                                        }
                                    }
                                }
                                else if (locationCoordinates != null && locationCoordinates.HasAttributes)
                                {
                                    if (locationCoordinates.Attribute("latitude") != null)
                                        latitude = locationCoordinates.Attribute("latitude")?.Value;

                                    if (locationCoordinates.Attribute("longitude") != null)
                                        longitude = locationCoordinates.Attribute("longitude")?.Value;
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
                            Identifier = activity.Element("iati-identifier")?.Value,
                            Title = projectTitle,
                            Locations = locations,
                            Countries = countries,
                            Regions = regions,
                            Description = activity.Element("description")?.Value,
                            Sectors = sectors,
                            DefaultCurrency = currency,
                            Transactions = transactionsList,
                            ParticipatingOrganizations = organizationList
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
                Debug.WriteLine(ex.Message);
            }
            return activityList;
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
