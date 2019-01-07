﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.IATILib.Parsers;
using AIMS.Models;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        /// Gets all the activities
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
        /// Gets all the organizations
        /// </summary>
        /// <returns></returns>
        ICollection<IATIOrganization> GetOrganizations();

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
            //string url = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=" + countryCode + "&stream=true";
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
                    var organizationList = from a in activityList
                                           select a.ParticipatingOrganizations;

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
                    parser = new ParserIATIVersion21();
                    iatiProjects = parser.ExtractProjects(xDoc);
                    break;
            }
            return iatiProjects;
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
            using (var unitWork = new UnitOfWork(context))
            {
                IATISettings settings = new IATISettings();
                var iatiSettings = unitWork.IATISettingsRepository.GetFirst(i => i.Id != 0);
                if (iatiSettings != null)
                {
                    settings.BaseUrl = iatiSettings.BaseUrl;
                    settings.CountryCode = iatiSettings.CountryCode;
                }
                return settings;
            }
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
                foreach(var iati in iatiList)
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
                    var iatiSettings = unitWork.IATISettingsRepository.GetFirst(i => i.Id != 0);
                    if (iatiSettings != null)
                    {
                        iatiSettings.BaseUrl = model.BaseUrl;
                        iatiSettings.CountryCode = model.CountryCode;
                        unitWork.Save();
                    }
                    else
                    {
                        unitWork.IATISettingsRepository.Insert(new EFIATISettings()
                        {
                            BaseUrl = model.BaseUrl,
                            CountryCode = model.CountryCode
                        });
                        unitWork.Save();
                    }
                }
                catch(Exception ex)
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
                foreach(var data in iatiData)
                {
                    unitWork.IATIDataRepository.Delete(data);
                }
                unitWork.Save();
                return response;
            }
        }
    }
}
