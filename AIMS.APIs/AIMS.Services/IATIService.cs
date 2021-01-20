using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.IATILib.Parsers;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AIMS.Services
{
    public interface IIATIService
    {
        /// <summary>
        /// Adds IATIData for the specified date
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Add(IATIModel model);

        /// <summary>
        /// Saves iati settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse SaveIATISettings(IATISettings model);

        /// <summary>
        /// Gets IATI Settings
        /// </summary>
        /// <returns></returns>
        IATISettings GetIATISettings();

        /// <summary>
        /// Gets all available iati settings
        /// </summary>
        /// <returns></returns>
        IEnumerable<IATISettings> GetIATISettingsList();

        /// <summary>
        /// Downloads the latest IATI into a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<ActionResponse> DownloadIATIFromUrl(string url, string fileToWrite);

        /// <summary>
        /// Downloads json for transaction types from IATI and write to a file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileToWrite"></param>
        /// <returns></returns>
        Task<ActionResponse> DownloadTransactionTypesFromUrl(string url, string fileToWrite);

        /// <summary>
        /// Loads latest IATI
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        ICollection<IATIActivity> GetMatchingIATIActivities(string dataFilePath, string criteria);

        /// <summary>
        /// Gets small version of the projects list
        /// </summary>
        /// <returns></returns>
        ICollection<IATIProject> GetProjects(string dataFilePath);

        /// <summary>
        /// Extracts and save sectors
        /// </summary>
        /// <returns></returns>
        ActionResponse ExtractAndSaveIATISectors(string dataFilePath, string sectorVocabPath);

        /// <summary>
        /// Amends the sector names
        /// </summary>
        /// <param name="dataFilePaht"></param>
        /// <returns></returns>
        ActionResponse NameSectorsCorrectly(string filePath, int sectorTypeId);

        /// <summary>
        /// Saves transaction types
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        ActionResponse SaveTransactionTypes(string json);

        /// <summary>
        /// Get activities by provided Ids
        /// </summary>
        /// <param name="IdsModel"></param>
        /// <returns></returns>
        Task<ICollection<IATIActivity>> GetActivitiesByIds(string dataFilePath, List<IATIByIdModel> IdsModel, string tTypeFilePath, string fTypeFilePath);

        /// <summary>
        /// Gets all the activitiesGetActivitiesByIds
        /// </summary>
        /// <returns></returns>
        IEnumerable<IATIActivity> GetAll();

        /// <summary>
        /// Gets matching list of activities for the provided keywords agains titles and descriptions
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        ICollection<IATIActivity> GetMatchingTitleDescriptions(string keywords);

        /// <summary>
        /// Extracts json only for transaction types
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string ExtractTransactionTypesJson(string json);

        /// <summary>
        /// Extract finance type json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string ExtractFinanceTypesJson(string json);

        /// <summary>
        /// Extracts sector vocabulary json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string ExtractSectorsVocabJson(string json);

        /// <summary>
        /// Extract country and respective code from json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        List<IATICountryModel> ExtractCountriesList(string json);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string ExtractOrganizationTypesVocabJson(string json);

        /// <summary>
        /// Extracts organization vocabulary json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string ExtractOrganizationsVocabJson(string json);

        /// <summary>
        /// Gets all the organizations
        /// </summary>
        /// <returns></returns>
        ICollection<IATIOrganization> GetOrganizations();

        /// <summary>
        /// Extract and save organization types from IATI to DB
        /// </summary>
        /// <returns></returns>
        ActionResponse ExtractAndSaveOrganizationTypes(string json);

        /// <summary>
        /// Extracts and save organizations from IATI to DB
        /// </summary>
        /// <returns></returns>
        ActionResponse ExtractAndSaveOrganizations(string dataFilePath, string orgTypesJson);

        /// <summary>
        /// Extracts and save locations from IATI to DB
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        ActionResponse ExtractAndSaveLocations(string dataFilePath);

        /// <summary>
        /// Deletes all the data less than the specified date
        /// </summary>
        /// <param name="datedLessThan"></param>
        /// <returns></returns>
        ActionResponse Delete(DateTime datedLessThan);

        /// <summary>
        /// Deletes inactive organizations (Temporary API to fix data)
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        ActionResponse DeleteInActiveOrganizations(string dataFilePath);

        /// <summary>
        /// Downloads latest IATI
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        Task<ActionResponse> DownloadLatestIATIAsync(string dataFilePath);
    }

    public class IATIService : IIATIService
    {
        AIMSDbContext context;
        IATISourceType sourceType = IATISourceType.Old;

        public IATIService(AIMSDbContext cntxt)
        {
            context = cntxt;
            this.GetIATISettings();
        }

        public ICollection<IATIActivity> GetMatchingIATIActivities(string dataFilePath, string criteria)
        {
            string url = dataFilePath;
            XmlReader xReader = XmlReader.Create(url);
            XDocument xDoc = XDocument.Load(xReader);
            XAttribute activity = null;

            if (this.sourceType == IATISourceType.New)
            {
                activity = (from el in xDoc.Descendants("iati-activities")
                            where el.HasAttributes && el.Attribute("version") != null
                            select el.Attribute("version")).FirstOrDefault();
            }
            else if (this.sourceType == IATISourceType.Old)
            {
                activity = (from el in xDoc.Descendants("iati-activity")
                            where el.HasAttributes && el.FirstAttribute != null
                            select el.FirstAttribute).FirstOrDefault();
            }

            IParser parser;
            ICollection<IATIActivity> activityList = new List<IATIActivity>();
            ICollection<IATIOrganization> organizations = new List<IATIOrganization>();
            string version = "";
            version = activity.Value;
            switch (version)
            {
                case "1.03":
                    parser = new ParserIATIVersion13();
                    activityList = parser.ExtractAcitivities(xDoc, criteria);
                    break;

                case "2.01":
                case "2.02":
                case "2.03":
                    if (this.sourceType == IATISourceType.Old)
                    {
                        parser = new ParserIATIVersion21();
                    }
                    else
                    {
                        parser = new ParserIATIVersion21New();
                    }
                    activityList = parser.ExtractAcitivities(xDoc, criteria);
                    break;
            }

            //Extract organizations for future use
            if (activityList.Count > 0)
            {
                if (activityList != null)
                {
                    var fundersList = from a in activityList
                                           select a.Funders;
                    var implementersList = from i in activityList
                                           select i.Implementers;

                    var organizationList = fundersList.Union(implementersList);

                    if (organizationList.Count() > 0)
                    {
                        foreach (var orgCollection in organizationList)
                        {
                            var orgList = from list in orgCollection
                                          select list;

                            foreach (var org in orgList)
                            {
                                IATIOrganization orgExists = null;
                                if (organizations.Count > 0 && org != null && org.Name != null)
                                {
                                    orgExists = (from o in organizations
                                                 where o.Name.ToLower().Equals(org.Name.ToLower())
                                                 select o).FirstOrDefault();
                                }

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
            return activityList;
        }

        public async Task<ICollection<IATIActivity>> GetActivitiesByIds(string dataFilePath, List<IATIByIdModel> IdsModel, string tTypeFilePath, string fTypeFilePath)
        {
            string url = dataFilePath;
            XmlReader xReader = XmlReader.Create(url);
            XDocument xDoc = XDocument.Load(xReader);
            XAttribute activity = null;

            if (this.sourceType == IATISourceType.New)
            {
                activity = (from el in xDoc.Descendants("iati-activities")
                            where el.HasAttributes && el.Attribute("version") != null
                            select el.Attribute("version")).FirstOrDefault();
            }
            else if (this.sourceType == IATISourceType.Old)
            {
                activity = (from el in xDoc.Descendants("iati-activity")
                            where el.HasAttributes && el.FirstAttribute != null
                            select el.FirstAttribute).FirstOrDefault();
            }

            var transactionTypes = JsonConvert.DeserializeObject<List<IATITransactionTypes>>(File.ReadAllText(tTypeFilePath));
            var financeTypes = JsonConvert.DeserializeObject<List<IATIFinanceTypes>>(File.ReadAllText(fTypeFilePath));

            IEnumerable<string> ids = (from id in IdsModel
                                       select id.Identifier);
            IParser parser;
            ICollection<IATIActivity> activityList = new List<IATIActivity>();
            ICollection<IATIOrganization> organizations = new List<IATIOrganization>();
            string version = "";
            version = activity.Value;
            switch (version)
            {
                case "1.03":
                    parser = new ParserIATIVersion13();
                    activityList = parser.ExtractAcitivitiesForIds(xDoc, ids);
                    break;

                case "2.01":
                case "2.02":
                case "2.03":
                    if (this.sourceType == IATISourceType.Old)
                    {
                        parser = new ParserIATIVersion21();
                    }
                    else
                    {
                        parser = new ParserIATIVersion21New();
                    }
                    activityList = parser.ExtractAcitivitiesForIds(xDoc, ids, transactionTypes, financeTypes);
                    break;
            }

            //Extract organizations for future use
            if (activityList.Count > 0)
            {
                if (activityList != null)
                {
                    var fundersList = from a in activityList
                                           select a.Funders;
                    var implementersList = from i in activityList
                                           select i.Implementers;
                    var organizationList = fundersList.Union(implementersList);

                    if (organizationList.Count() > 0)
                    {
                        foreach (var orgCollection in organizationList)
                        {
                            var orgList = from list in orgCollection
                                          select list;

                            foreach (var org in orgList)
                            {
                                IATIOrganization orgExists = null;
                                if (organizations.Count > 0 && org != null && org.Name != null)
                                {
                                    orgExists = (from o in organizations
                                                 where o.Name.ToLower().Equals(org.Name.ToLower())
                                                 select o).FirstOrDefault();
                                }

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
            return await Task<ICollection<IATIActivity>>.Run(() => activityList).ConfigureAwait(false);
        }

        public string ExtractTransactionTypesJson(string json)
        {
            string tTypeJson = null;
            List<IATITransactionTypes> transactionTypes = new List<IATITransactionTypes>();
            JObject jObject = JObject.Parse(json);
            var tCodesArray = jObject["data"].ToArray();
            if (tCodesArray.Length > 0)
            {
                foreach (var tCode in tCodesArray)
                {
                    transactionTypes.Add(new IATITransactionTypes()
                    {
                        Code = (string)tCode["code"],
                        Name = (string)tCode["name"]
                    });
                }
                tTypeJson = JsonConvert.SerializeObject(transactionTypes);
            }
            return tTypeJson;
        }

        public List<IATICountryModel> ExtractCountriesList(string json)
        {
            List<IATICountryModel> countries = new List<IATICountryModel>();
            JObject jObject = JObject.Parse(json);
            var cCodesArray = jObject["data"].ToArray();
            if (cCodesArray.Length > 0)
            {
                foreach (var cCode in cCodesArray)
                {
                    countries.Add(new IATICountryModel()
                    {
                        Code = (string)cCode["code"],
                        Country = (string)cCode["name"],
                    });
                }
            }
            return countries;
        }

        public string ExtractFinanceTypesJson(string json)
        {
            string fTypeJson = null;
            List<IATIFinanceTypes> transactionTypes = new List<IATIFinanceTypes>();
            JObject jObject = JObject.Parse(json);
            var tCodesArray = jObject["data"].ToArray();
            if (tCodesArray.Length > 0)
            {
                foreach (var tCode in tCodesArray)
                {
                    transactionTypes.Add(new IATIFinanceTypes()
                    {
                        Code = (string)tCode["code"],
                        Name = (string)tCode["name"]
                    });
                }
                var unitWork = new UnitOfWork(context);
                fTypeJson = JsonConvert.SerializeObject(transactionTypes);
            }
            return fTypeJson;
        }

        public string ExtractSectorsVocabJson(string json)
        {
            string sVocabJson = null;
            List<IATISectorsVocabulary> sectorVocabs = new List<IATISectorsVocabulary>();
            JObject jObject = JObject.Parse(json);
            var tCodesArray = jObject["data"].ToArray();
            if (tCodesArray.Length > 0)
            {
                foreach (var tCode in tCodesArray)
                {
                    sectorVocabs.Add(new IATISectorsVocabulary()
                    {
                        Code = (string)tCode["code"],
                        Name = (string)tCode["name"]
                    });
                }
                var unitWork = new UnitOfWork(context);
                sVocabJson = JsonConvert.SerializeObject(sectorVocabs);
            }
            return sVocabJson;
        }

        public string ExtractOrganizationTypesVocabJson(string json)
        {
            string sVocabJson = null;
            List<IATIOrganizationTypeVocabulary> orgTypesVocabs = new List<IATIOrganizationTypeVocabulary>();
            JObject jObject = JObject.Parse(json);
            var tCodesArray = jObject["data"].ToArray();
            if (tCodesArray.Length > 0)
            {
                foreach (var tCode in tCodesArray)
                {
                    orgTypesVocabs.Add(new IATIOrganizationTypeVocabulary()
                    {
                        Code = (int)tCode["code"],
                        Name = (string)tCode["name"]
                    });
                }
                sVocabJson = JsonConvert.SerializeObject(orgTypesVocabs);
            }
            return sVocabJson;
        }

        public string ExtractOrganizationsVocabJson(string json)
        {
            string sVocabJson = null;
            List<IATISectorsVocabulary> sectorVocabs = new List<IATISectorsVocabulary>();
            JObject jObject = JObject.Parse(json);
            var tCodesArray = jObject["data"].ToArray();
            if (tCodesArray.Length > 0)
            {
                foreach (var tCode in tCodesArray)
                {
                    sectorVocabs.Add(new IATISectorsVocabulary()
                    {
                        Code = (string)tCode["code"],
                        Name = (string)tCode["name"]
                    });
                }
                var unitWork = new UnitOfWork(context);
                sVocabJson = JsonConvert.SerializeObject(sectorVocabs);
            }
            return sVocabJson;
        }

        public ICollection<IATIProject> GetProjects(string dataFilePath)
        {
            ICollection<IATIProject> iatiProjects = new List<IATIProject>();
            string url = dataFilePath;
            XmlReader xReader = XmlReader.Create(url);
            XDocument xDoc = XDocument.Load(xReader);
            XAttribute activity = null;

            if (this.sourceType == IATISourceType.New)
            {
                activity = (from el in xDoc.Descendants("iati-activities")
                            where el.HasAttributes && el.Attribute("version") != null
                            select el.Attribute("version")).FirstOrDefault();
            }
            else if (this.sourceType == IATISourceType.Old)
            {
                activity = (from el in xDoc.Descendants("iati-activity")
                            where el.HasAttributes && el.FirstAttribute != null
                            select el.FirstAttribute).FirstOrDefault();
            }

            IParser parser;
            string version = "";
            version = activity.Value;
            switch (version)
            {
                case "1.03":
                    parser = new ParserIATIVersion13();
                    iatiProjects = parser.ExtractProjects(xDoc);
                    break;

                case "2.01":
                case "2.02":
                case "2.03":
                    if (this.sourceType == IATISourceType.Old)
                    {
                        parser = new ParserIATIVersion21();
                    }
                    else
                    {
                        parser = new ParserIATIVersion21New();
                    }
                    iatiProjects = parser.ExtractProjects(xDoc);
                    break;
            }
            return iatiProjects;
        }

        public ActionResponse ExtractAndSaveOrganizationTypes(string json)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();

            try
            {
                List<IATIOrganizationTypeVocabulary> orgTypesList = new List<IATIOrganizationTypeVocabulary>();
                if (!string.IsNullOrEmpty(json))
                {
                    List<EFOrganizationTypes> newOrganizationTypes = new List<EFOrganizationTypes>();
                    orgTypesList = this.GetDeserializedOrgTypes(json);
                    if (orgTypesList.Any())
                    {
                        List<string> orgTypesDb = unitWork.OrganizationTypesRepository.GetProjection(o => o.Id != 0, o => o.TypeName).ToList<string>();
                        foreach(var type in orgTypesList)
                        {
                            if (!orgTypesDb.Contains(type.Name, StringComparer.OrdinalIgnoreCase))
                            {
                                newOrganizationTypes.Add(new EFOrganizationTypes()
                                {
                                    TypeName = type.Name
                                });
                            }
                        }

                        if (newOrganizationTypes.Count > 0)
                        {
                            unitWork.OrganizationTypesRepository.InsertMultiple(newOrganizationTypes);
                            unitWork.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public ActionResponse ExtractAndSaveOrganizations(string dataFilePath, string orgTypesJson)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            string url = dataFilePath;

            try
            {
                XmlReader xReader = XmlReader.Create(url);
                XDocument xDoc = XDocument.Load(xReader);
                XAttribute activity = null;

                if (this.sourceType == IATISourceType.New)
                {
                    activity = (from el in xDoc.Descendants("iati-activities")
                                where el.HasAttributes && el.Attribute("version") != null
                                select el.Attribute("version")).FirstOrDefault();
                }
                else if (this.sourceType == IATISourceType.Old)
                {
                    activity = (from el in xDoc.Descendants("iati-activity")
                                where el.HasAttributes && el.FirstAttribute != null
                                select el.FirstAttribute).FirstOrDefault();
                }

                IParser parser;
                ICollection<IATIActivity> activityList = new List<IATIActivity>();
                ICollection<IATIOrganizationModel> organizations = new List<IATIOrganizationModel>();
                string version = "";
                version = activity.Value;
                switch (version)
                {
                    case "1.03":
                        parser = new ParserIATIVersion13();
                        organizations = parser.ExtractOrganizations(xDoc);
                        break;

                    case "2.01":
                    case "2.02":
                    case "2.03":
                        if (this.sourceType == IATISourceType.Old)
                        {
                            parser = new ParserIATIVersion21();
                        }
                        else
                        {
                            parser = new ParserIATIVersion21New();
                        }
                        organizations = parser.ExtractOrganizations(xDoc);
                        break;
                }

                var organizationsList = unitWork.IATIOrganizationRepository.GetManyQueryable(o => o.Id != 0);
                var orgNames = (from o in organizationsList
                                select o.OrganizationName.Trim()).ToList<string>();

                List<EFIATIOrganization> newIATIOrganizations = new List<EFIATIOrganization>();
                int withOutTypeCount = 0;
                foreach (var org in organizations)
                {
                    if (!string.IsNullOrEmpty(org.Name) && !string.IsNullOrWhiteSpace(org.Name))
                    {
                        if (orgNames.Contains(org.Name.Trim(), StringComparer.OrdinalIgnoreCase) == false)
                        {
                            EFIATIOrganization isOrganizationInList = null;
                            EFIATIOrganization isOrganizationInDb = null;

                            if (newIATIOrganizations.Count > 0)
                            {
                                isOrganizationInList = (from s in newIATIOrganizations
                                                        where s.OrganizationName.ToLower() == org.Name.ToLower()
                                                        select s).FirstOrDefault();

                                isOrganizationInDb = (from o in organizationsList
                                                      where o.OrganizationName.ToLower() == org.Name.ToLower()
                                                      select o).FirstOrDefault();
                            }

                            if (isOrganizationInList == null && isOrganizationInDb == null)
                            {
                                int orgCode = org.Code;
                                newIATIOrganizations.Add(new EFIATIOrganization()
                                {
                                    OrganizationName = org.Name,
                                });
                            }
                        }
                    }
                }

                if (newIATIOrganizations.Count > 0)
                {
                    unitWork.IATIOrganizationRepository.InsertMultiple(newIATIOrganizations);
                    unitWork.Save();
                    response.Message = withOutTypeCount.ToString();
                    response.ReturnedId = newIATIOrganizations.Count;
                }
                else
                {
                    response.Message = "0"; 
                }

                //Temporary script to delete any irrelevent IATI Organizations
                List<EFIATIOrganization> iatiOrgsToDelete = new List<EFIATIOrganization>();
                organizationsList = unitWork.IATIOrganizationRepository.GetManyQueryable(o => o.Id != 0);
                foreach(var org in organizationsList)
                {
                    var isOrgInIATI = (from o in organizations
                                       where o.Name.Trim().Equals(org.OrganizationName.Trim(), StringComparison.OrdinalIgnoreCase)
                                       select o).FirstOrDefault();

                    if (isOrgInIATI == null)
                    {
                        iatiOrgsToDelete.Add(org);
                    }    
                }

                if (iatiOrgsToDelete.Count > 0)
                {
                    foreach(var org in iatiOrgsToDelete)
                    {
                        unitWork.IATIOrganizationRepository.Delete(org);
                    }
                    unitWork.Save();
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ActionResponse SaveTransactionTypes(string json)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                List<IATITransactionTypes> transactionTypes = new List<IATITransactionTypes>();
                JObject jObject = JObject.Parse(json);
                var tCodesArray = jObject["data"].ToArray();
                if (tCodesArray.Length > 0)
                {
                    foreach (var tCode in tCodesArray)
                    {
                        transactionTypes.Add(new IATITransactionTypes()
                        {
                            Code = (string)tCode["code"],
                            Name = (string)tCode["name"]
                        });
                    }
                    var unitWork = new UnitOfWork(context);
                    string tTypeJson = JsonConvert.SerializeObject(transactionTypes);
                    var iatiSettings = unitWork.IATISettingsRepository.GetOne(i => i.Id != 0);
                    if (iatiSettings != null)
                    {
                        iatiSettings.TransactionTypesJson = tTypeJson;
                    }
                    unitWork.Save();
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ActionResponse ExtractAndSaveLocations(string dataFilePath)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            string url = dataFilePath;

            try
            {
                XmlReader xReader = XmlReader.Create(url);
                XDocument xDoc = XDocument.Load(xReader);
                XAttribute activity = null;

                if (this.sourceType == IATISourceType.New)
                {
                    activity = (from el in xDoc.Descendants("iati-activities")
                                where el.HasAttributes && el.Attribute("version") != null
                                select el.Attribute("version")).FirstOrDefault();
                }
                else if (this.sourceType == IATISourceType.Old)
                {
                    activity = (from el in xDoc.Descendants("iati-activity")
                                where el.HasAttributes && el.FirstAttribute != null
                                select el.FirstAttribute).FirstOrDefault();
                }

                IParser parser;
                ICollection<IATIActivity> activityList = new List<IATIActivity>();
                ICollection<IATILocation> locations = new List<IATILocation>();
                string version = "";
                version = activity.Value;
                switch (version)
                {
                    case "1.03":
                        parser = new ParserIATIVersion13();
                        locations = parser.ExtractLocations(xDoc);
                        break;

                    case "2.01":
                    case "2.02":
                    case "2.03":
                        if (this.sourceType == IATISourceType.Old)
                        {
                            parser = new ParserIATIVersion21();
                        }
                        else
                        {
                            parser = new ParserIATIVersion21New();
                        }
                        locations = parser.ExtractLocations(xDoc);
                        break;
                }

                var locationsList = unitWork.LocationRepository.GetManyQueryable(o => o.Id != 0);
                var locationNames = (from l in locationsList
                                select l.Location.Trim()).ToList<string>();

                List<EFLocation> newIATILocations = new List<EFLocation>();
                foreach (var loc in locations)
                {
                    if (!string.IsNullOrEmpty(loc.Name) && !string.IsNullOrWhiteSpace(loc.Name))
                    {
                        if (locationNames.Contains(loc.Name.Trim(), StringComparer.OrdinalIgnoreCase) == false)
                        {
                            EFLocation isLocationInList = null;
                            EFLocation isLocationInDb = null;

                            if (newIATILocations.Count > 0)
                            {
                                isLocationInList = (from l in newIATILocations
                                                        where l.Location.ToLower() == loc.Name.ToLower()
                                                        select l).FirstOrDefault();

                                isLocationInDb = (from l in locationsList
                                                      where l.Location.ToLower() == loc.Name.ToLower()
                                                      select l).FirstOrDefault();
                            }

                            if (isLocationInList == null && isLocationInDb == null)
                            {
                                decimal latitude = 0;
                                decimal longitude = 0;

                                decimal.TryParse(loc.Latitude, out latitude);
                                decimal.TryParse(loc.Longitude, out longitude);

                                newIATILocations.Add(new EFLocation()
                                {
                                    Location = loc.Name,
                                    Latitude = latitude,
                                    Longitude = longitude
                                });
                            }
                        }
                    }
                }

                if (newIATILocations.Count > 0)
                {
                    unitWork.LocationRepository.InsertMultiple(newIATILocations);
                    unitWork.Save();
                    response.ReturnedId = newIATILocations.Count;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ActionResponse NameSectorsCorrectly(string filePath, int sectorTypeId)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            try
            {
                IParser parser = new ParserIATIVersion21();
                XmlReader xReader = XmlReader.Create(filePath);
                XDocument xDoc = XDocument.Load(xReader);
                var sectorsView = parser.ExtractSectorsFromSource(xDoc);
                var sectorType = unitWork.SectorTypesRepository.GetByID(sectorTypeId);
                if (sectorType != null)
                {
                    if (!sectorType.TypeName.Trim().Equals(sectorsView.SectorTypeName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        sectorType.TypeName = sectorsView.SectorTypeName;
                        unitWork.SectorTypesRepository.Update(sectorType);
                        unitWork.Save();
                    }
                }

                var sectorsList = sectorsView.SectorsList;
                var sectorsInDB = unitWork.SectorRepository.GetManyQueryable(s => s.SectorTypeId == sectorTypeId);
                bool isSaved = false;
                foreach (var sector in sectorsList)
                {
                    var isInDb = (from s in sectorsInDB
                                  where s.IATICode == sector.SectorCode
                                  select s).FirstOrDefault();
                    if (isInDb != null)
                    {
                        isInDb.SectorName = sector.SectorName;
                        unitWork.SectorRepository.Update(isInDb);
                        isSaved = true;
                    }
                    else
                    {
                        unitWork.SectorRepository.Insert(new EFSector()
                        {
                            SectorType = sectorType,
                            SectorName = sector.SectorName,
                            IATICode = sector.SectorCode,
                            IsUnAttributed = false,
                            TimeStamp = DateTime.Now,
                            ParentSector = null
                        });
                        unitWork.Save();
                    }
                }
                if (isSaved)
                {
                    unitWork.Save();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ActionResponse ExtractAndSaveIATISectors(string dataFilePath, string sectorVocabPath)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            ICollection<IATISectorModel> iatiSectors = new List<IATISectorModel>();
            string url = dataFilePath;
            var sectorVocabs = JsonConvert.DeserializeObject<List<IATISectorsVocabulary>>(File.ReadAllText(sectorVocabPath));

            try
            {
                XmlReader xReader = XmlReader.Create(url);
                XDocument xDoc = XDocument.Load(xReader);
                XAttribute activity = null;

                if (this.sourceType == IATISourceType.New)
                {
                    activity = (from el in xDoc.Descendants("iati-activities")
                                where el.HasAttributes && el.Attribute("version") != null
                                select el.Attribute("version")).FirstOrDefault();
                }
                else if (this.sourceType == IATISourceType.Old)
                {
                    activity = (from el in xDoc.Descendants("iati-activity")
                                where el.HasAttributes && el.FirstAttribute != null
                                select el.FirstAttribute).FirstOrDefault();
                }

                IParser parser;
                string version = "";
                version = activity.Value;
                switch (version)
                {
                    case "1.03":
                        parser = new ParserIATIVersion13();
                        iatiSectors = parser.ExtractSectors(xDoc);
                        break;

                    case "2.01":
                    case "2.02":
                    case "2.03":
                        if (this.sourceType == IATISourceType.Old)
                        {
                            parser = new ParserIATIVersion21();
                        }
                        else
                        {
                            parser = new ParserIATIVersion21New();
                        }
                        iatiSectors = parser.ExtractSectors(xDoc);
                        break;
                }

                var sectorTypes = unitWork.SectorTypesRepository.GetManyQueryable(s => s.IsSourceType == true);
                if (sectorTypes != null)
                {
                    var sectorTypeIds = (from st in sectorTypes
                                         select st.Id).ToList<int>();
                    var sectorsList = unitWork.SectorRepository.GetManyQueryable(s => sectorTypeIds.Contains(s.SectorTypeId));
                    List<string> sectorNames = (from s in sectorsList
                                                select s.SectorName).ToList<string>();

                    List<EFSector> newIATISectors = new List<EFSector>();
                    bool isUpdated = false;
                    foreach (var sector in iatiSectors)
                    {
                        //if (sectorNames.Contains(sector.SectorName, StringComparer.OrdinalIgnoreCase) == false)
                        //{
                        EFSector isSectorInList = null;
                        var sectorType = (from s in sectorTypes
                                          where (s.IATICode != null && s.IATICode == sector.SectorTypeCode)
                                          select s).FirstOrDefault();

                        if (sectorType == null)
                        {
                            var sectorVocab = (from v in sectorVocabs
                                               where v.Code == sector.SectorTypeCode
                                               select v).FirstOrDefault();

                            if (sectorVocab != null && (sectorVocab.Code != "99" && sectorVocab.Code != "98"))
                            {
                                sectorType = unitWork.SectorTypesRepository.Insert(new EFSectorTypes()
                                {
                                    IsSourceType = true,
                                    TypeName = sectorVocab.Name,
                                    IATICode = sectorVocab.Code
                                });
                                unitWork.Save();
                            }
                        }

                        EFSector isSectorInDb = null;
                        isSectorInDb = (from s in sectorsList
                                        where s.IATICode == sector.SectorCode
                                        select s).FirstOrDefault();

                        if (isSectorInDb == null)
                        {
                            isSectorInDb = (from s in sectorsList
                                            where s.SectorName.Trim().Equals(sector.SectorName.Trim(), StringComparison.OrdinalIgnoreCase)
                                            select s).FirstOrDefault();

                            if (isSectorInDb != null)
                            {
                                isSectorInDb.IATICode = sector.SectorCode;
                                unitWork.SectorRepository.Update(isSectorInDb);
                                isUpdated = true;
                            }
                        }
                        isSectorInList = (from s in newIATISectors
                                          where s.IATICode == sector.SectorCode
                                          select s).FirstOrDefault();

                        if ((isSectorInList == null && isSectorInDb == null) && sectorType != null)
                            {
                                newIATISectors.Add(new EFSector()
                                {
                                    SectorName = sector.SectorName,
                                    IATICode = sector.SectorCode,
                                    SectorType = sectorType,
                                    ParentSector = null
                                });
                            }
                    }

                    if (newIATISectors.Count > 0 || isUpdated)
                    {
                        unitWork.SectorRepository.InsertMultiple(newIATISectors);
                        unitWork.Save();
                        response.ReturnedId = newIATISectors.Count;
                    }
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ActionResponse> DownloadIATIFromUrl(string url, string fileToWrite)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                string xml;
                using (var client = new WebClient())
                {
                    xml = client.DownloadString(url);
                }
                File.WriteAllText(fileToWrite, xml);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }

        public async Task<ActionResponse> DownloadTransactionTypesFromUrl(string url, string fileToWrite)
        {
            ActionResponse response = new ActionResponse();
            HttpClient client = new HttpClient();
            List<IATITransactionTypes> list = new List<IATITransactionTypes>();
            try
            {
                var httpResponse = await client.GetAsync(url);
                string json = await httpResponse.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(json);
                string parsedJson = obj["data"].ToString();
                File.WriteAllText(fileToWrite, parsedJson);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }

        public IEnumerable<IATIActivity> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<IATIActivity> activityList = new List<IATIActivity>();
                DateTime dated = DateTime.Now;
                IATIActivity activityData = new IATIActivity();
                var iatiData = unitWork.IATIDataRepository.GetFirst(i => i.Id != 0);
                if (iatiData != null)
                {
                    activityList = JsonConvert.DeserializeObject<List<IATIActivity>>(iatiData.Data);
                }
                return activityList;
            }
        }

        public IATISettings GetIATISettings()
        {
            var unitWork = new UnitOfWork(context);
            IATISettings settings = new IATISettings();
            var iatiSettings = unitWork.IATISettingsRepository.GetOne(i => i.IsActive == true);
            if (iatiSettings != null)
            {
                settings.BaseUrl = iatiSettings.BaseUrl;
                settings.SourceType = iatiSettings.SourceType;
                this.sourceType = iatiSettings.SourceType;
            }
            return settings;
        }

        public IEnumerable<IATISettings> GetIATISettingsList()
        {
            var unitWork = new UnitOfWork(context);
            List<IATISettings> settingsList = new List<IATISettings>();
            var iatiSettingsList = unitWork.IATISettingsRepository.GetManyQueryable(i => i.Id != 0);
            foreach(var setting in iatiSettingsList)
            {
                settingsList.Add(new IATISettings()
                {
                    SettingId = setting.Id,
                    BaseUrl = setting.BaseUrl,
                    HelpText = setting.HelpText,
                    SourceType = setting.SourceType,
                    IsActive= setting.IsActive
                });
            }
            return settingsList;
        }

        public ICollection<IATIOrganization> GetOrganizations()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<IATIOrganization> organizations = new List<IATIOrganization>();
                DateTime dated = DateTime.Now;
                var iatiData = unitWork.IATIDataRepository.GetFirst(i => i.Id != 0);
                if (iatiData != null)
                {
                    var organizationStr = iatiData.Organizations;
                    if (!string.IsNullOrEmpty(organizationStr))
                    {
                        organizations = JsonConvert.DeserializeObject<List<IATIOrganization>>(organizationStr);
                    }
                }
                return organizations;
            }
        }

        public ICollection<IATIActivity> GetMatchingTitleDescriptions(string keywords)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<IATIActivity> iATIActivities = new List<IATIActivity>();
                var iatiDataList = unitWork.IATIDataRepository.GetMany(d => d.Dated != null);
                var iatiData = (from data in iatiDataList
                                orderby data.Dated descending
                                select data).FirstOrDefault();

                string activitiesStr = "";
                if (iatiData != null)
                {
                    var allActivities = JsonConvert.DeserializeObject<List<IATIActivity>>(activitiesStr);
                    iATIActivities = (from activity in allActivities
                                      where activity.Title.Contains(keywords) ||
                                      activity.Description.Contains(keywords)
                                      select activity).ToList();

                }
                return iATIActivities;
            }
        }

        public ActionResponse Add(IATIModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var iatiList = unitWork.IATIDataRepository.GetMany(i => i.Id != 0);
                foreach (var iati in iatiList)
                {
                    unitWork.IATIDataRepository.Delete(iati);
                }
                unitWork.Save();

                unitWork.IATIDataRepository.Insert(new EFIATIData()
                {
                    Data = model.Data,
                    Organizations = model.Organizations,
                    Dated = DateTime.Now
                });
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse SaveIATISettings(IATISettings model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    bool isActiveSourceUpdated = false;
                    var iatiSettingsList = unitWork.IATISettingsRepository.GetManyQueryable(i => i.Id != 0);
                    var isIatiSettingExists = (from i in iatiSettingsList
                                               where i.Id == model.SettingId
                                               select i).FirstOrDefault();

                    if (isIatiSettingExists != null)
                    {
                        if (model.IsActive != isIatiSettingExists.IsActive)
                        {
                            isActiveSourceUpdated = true;
                        }
                        isIatiSettingExists.BaseUrl = model.BaseUrl;
                        isIatiSettingExists.SourceType = model.SourceType;
                        isIatiSettingExists.HelpText = model.HelpText;
                        isIatiSettingExists.IsActive = model.IsActive;
                    }
                    else
                    {
                        unitWork.IATISettingsRepository.Insert(new EFIATISettings()
                        {
                            BaseUrl = model.BaseUrl,
                            HelpText = model.HelpText,
                            SourceType = model.SourceType,
                            IsActive = model.IsActive
                        });
                        if (model.IsActive)
                        {
                            isActiveSourceUpdated = true;
                        }
                    }
                    
                    var iatiSettingToUpdate = (from i in iatiSettingsList
                                               where i.Id != model.SettingId
                                               select i).FirstOrDefault();
                    
                    if (iatiSettingToUpdate != null)
                    {
                        iatiSettingToUpdate.IsActive = !model.IsActive;
                    }
                    unitWork.Save();
                    
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
            }
        }

        public async Task<ActionResponse> DownloadLatestIATIAsync(string dataFilePath)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var iatiSettings = unitWork.IATISettingsRepository.GetOne(i => i.IsActive == true);
                    if (iatiSettings != null)
                    {
                        string baseUrl = iatiSettings.BaseUrl;
                        string xml = "";
                        using (var client = new WebClient())
                        {
                            xml = client.DownloadString(baseUrl);
                        }
                        File.WriteAllText(dataFilePath, xml);
                    }
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse Delete(DateTime dated)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var iatiData = unitWork.IATIDataRepository.GetMany(d => d.Dated.Date < dated.Date);
                foreach (var data in iatiData)
                {
                    unitWork.IATIDataRepository.Delete(data);
                }
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse DeleteInActiveOrganizations(string dataFilePath)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    XmlReader xReader = XmlReader.Create(dataFilePath);
                    XDocument xDoc = XDocument.Load(xReader);
                    XAttribute activity = null;

                    if (this.sourceType == IATISourceType.New)
                    {
                        activity = (from el in xDoc.Descendants("iati-activities")
                                    where el.HasAttributes && el.Attribute("version") != null
                                    select el.Attribute("version")).FirstOrDefault();
                    }
                    else if (this.sourceType == IATISourceType.Old)
                    {
                        activity = (from el in xDoc.Descendants("iati-activity")
                                    where el.HasAttributes && el.FirstAttribute != null
                                    select el.FirstAttribute).FirstOrDefault();
                    }

                    IParser parser = new ParserIATIVersion21();
                    var organizations = parser.ExtractOrganizations(xDoc);
                    var organizationsToDelete = unitWork.IATIOrganizationRepository.GetManyQueryable(o => o.Id != 0);
                    foreach(var org in organizationsToDelete)
                    {
                        unitWork.IATIOrganizationRepository.Delete(org);
                    }
                    unitWork.Save();

                    foreach(var org in organizations)
                    {
                        unitWork.IATIOrganizationRepository.Insert(new EFIATIOrganization()
                        {
                            OrganizationName = org.Name.Trim(),
                        });
                    }
                    unitWork.Save();
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        private List<IATIOrganizationTypeVocabulary> GetDeserializedOrgTypes(string json)
        {
            List<IATIOrganizationTypeVocabulary> orgTypesList = new List<IATIOrganizationTypeVocabulary>();
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<List<IATIOrganizationTypeVocabulary>>(json);
            }
            return orgTypesList;
        }
        
    }
}
