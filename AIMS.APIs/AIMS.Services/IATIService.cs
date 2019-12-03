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
    }

    public class IATIService : IIATIService
    {
        AIMSDbContext context;

        public IATIService(AIMSDbContext cntxt)
        {
            this.context = cntxt;
        }

        public ICollection<IATIActivity> GetMatchingIATIActivities(string dataFilePath, string criteria)
        {
            string url = dataFilePath;
            XmlReader xReader = XmlReader.Create(url);
            XDocument xDoc = XDocument.Load(xReader);
            var activity = (from el in xDoc.Descendants("iati-activity")
                            select el.FirstAttribute).FirstOrDefault();

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
                    parser = new ParserIATIVersion21();
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
            var activity = (from el in xDoc.Descendants("iati-activity")
                            select el.FirstAttribute).FirstOrDefault();

            //var transactionTypes = await this.GetTransactionTypes(tTypeFilePath);
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
                    parser = new ParserIATIVersion21();
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
                var unitWork = new UnitOfWork(context);
                tTypeJson = JsonConvert.SerializeObject(transactionTypes);
            }
            return tTypeJson;
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
                        Code = (int)tCode["code"],
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
                        Code = (int)tCode["code"],
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
            var activity = (from el in xDoc.Descendants("iati-activity")
                            select el.FirstAttribute).FirstOrDefault();

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
                    parser = new ParserIATIVersion21();
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
                var activity = (from el in xDoc.Descendants("iati-activity")
                                select el.FirstAttribute).FirstOrDefault();

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
                        parser = new ParserIATIVersion21();
                        organizations = parser.ExtractOrganizations(xDoc);
                        break;
                }

                var organizationTypesVocabs = this.GetDeserializedOrgTypes(orgTypesJson);
                var organizationTypes = unitWork.OrganizationTypesRepository.GetManyQueryable(o => o.Id != 0);
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
                                EFOrganizationTypes orgType = null;
                                string orgTypeName = (from v in organizationTypesVocabs
                                                        where v.Code == org.Code
                                                        select v.Name).FirstOrDefault();
                                orgType = (from t in organizationTypes
                                                      where t.TypeName.Equals(orgTypeName, StringComparison.OrdinalIgnoreCase)
                                                      select t).FirstOrDefault();

                                withOutTypeCount = (orgType == null) ? withOutTypeCount : (withOutTypeCount + 1);
                                if (orgType != null)
                                {
                                    newIATIOrganizations.Add(new EFIATIOrganization()
                                    {
                                        OrganizationName = org.Name,
                                        OrganizationType = orgType
                                    });
                                }
                                else
                                {
                                    newIATIOrganizations.Add(new EFIATIOrganization()
                                    {
                                        OrganizationName = org.Name,
                                    });
                                }
                                
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
                var activity = (from el in xDoc.Descendants("iati-activity")
                                select el.FirstAttribute).FirstOrDefault();

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
                        parser = new ParserIATIVersion21();
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
                var activity = (from el in xDoc.Descendants("iati-activity")
                                select el.FirstAttribute).FirstOrDefault();

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
                        parser = new ParserIATIVersion21();
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
                    foreach (var sector in iatiSectors)
                    {
                        if (sectorNames.Contains(sector.SectorName, StringComparer.OrdinalIgnoreCase) == false)
                        {
                            EFSector isSectorInList = null;
                            var sectorType = (from s in sectorTypes
                                              where (s.IATICode != null && s.IATICode == sector.SectorTypeCode)
                                              select s).FirstOrDefault();

                            if (sectorType == null)
                            {
                                var sectorVocab = (from v in sectorVocabs
                                                   where v.Code == sector.SectorTypeCode
                                                   select v).FirstOrDefault();

                                if (sectorVocab != null && (sectorVocab.Code != 99 && sectorVocab.Code != 98))
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

                            if (newIATISectors.Count > 0)
                            {
                                isSectorInList = (from s in newIATISectors
                                                  where s.SectorName.ToLower() == sector.SectorName.ToLower()
                                                  select s).FirstOrDefault();
                            }

                            if (isSectorInList == null && sectorType != null)
                            {
                                newIATISectors.Add(new EFSector()
                                {
                                    SectorName = sector.SectorName,
                                    SectorType = sectorType,
                                    ParentSector = null
                                });
                            }
                        }
                    }

                    if (newIATISectors.Count > 0)
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
            var iatiSettings = unitWork.IATISettingsRepository.GetOne(i => i.Id != 0);
            if (iatiSettings != null)
            {
                settings.BaseUrl = iatiSettings.BaseUrl;
            }
            return settings;
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
                    var iatiSettings = unitWork.IATISettingsRepository.GetOne(i => i.Id != 0);
                    if (iatiSettings != null)
                    {
                        iatiSettings.BaseUrl = model.BaseUrl;
                        unitWork.Save();
                    }
                    else
                    {
                        unitWork.IATISettingsRepository.Insert(new EFIATISettings()
                        {
                            BaseUrl = model.BaseUrl,
                        });
                        unitWork.Save();
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
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

        private List<IATIOrganizationTypeVocabulary> GetDeserializedOrgTypes(string json)
        {
            List<IATIOrganizationTypeVocabulary> orgTypesList = new List<IATIOrganizationTypeVocabulary>();
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<List<IATIOrganizationTypeVocabulary>>(json);
            }
            return orgTypesList;
        }

        /*private async Task<List<IATITransactionTypes>> GetTransactionTypes(string url)
        {
            HttpClient client = new HttpClient();
            List<IATITransactionTypes> list = new List<IATITransactionTypes>();
            try
            {
                var httpResponse = await client.GetAsync(url);
                string json = await httpResponse.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<IATITransactionTypes>>(json);
            }
            catch (Exception)
            {
            }
            return list;
        }*/
    }
}
