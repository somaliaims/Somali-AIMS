using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AIMS.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using System.Net;

namespace AIMS.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Search matching projects by sector wise grouped for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search matching projects without sector for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<ProjectProfileReportBySector> GetProjectsWithoutSectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search matching projects by sector wise grouped for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportByLocation> GetProjectsByLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search projects having no assigned locations
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<ProjectProfileReportByLocation> GetProjectsWithoutLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search project by year in a time series manner
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<TimeSeriesReportByYear> GetProjectsByYear(SearchProjectsByYearModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Gets lighter version of projects budget report
        /// </summary>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<ProjectsBudgetReportSummary> GetProjectsBudgetReportSummary(string reportUrl, string defaultCurrency, decimal exchangeRate, int chartType = 0);

        /// <summary>
        /// Gets budget report
        /// </summary>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="chartType"></param>
        /// <returns></returns>
        Task<BudgetReport> GetBudgetReport(string reportUrl, string defaultCurrency, decimal exchangeRate, int chartType = 0);

        /// <summary>
        /// Gets envelope report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        Task<EnvelopeReport> GetEnvelopeReport(SearchEnvelopeModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Gets all projects report
        /// </summary>
        /// <returns></returns>
        Task<ProjectReportView> GetAllProjectsReport(SearchAllProjectsModel model);

        /// <summary>
        /// Gets project report for a single project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProjectDetailReport> GetProjectReport(int id, string reportUrl);

        /// <summary>
        /// Internal function for extracting rate of default currency
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="ratesList"></param>
        /// <returns></returns>
        decimal GetExchangeRateForCurrency(string currency, List<CurrencyWithRates> ratesList);

        /// <summary>
        /// Gets projects summary against the location
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectSummary> GetLatestProjectsSummary();

        /// <summary>
        /// Gets location names only for projects report
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetLocationNames();
    }

    public class ReportService : IReportService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ReportService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectSummary> GetLatestProjectsSummary()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectSummary> summaryList = new List<ProjectSummary>();
                var projects = unitWork.ProjectRepository.GetWithInclude(p => p.EndingFinancialYear.FinancialYear >= DateTime.Now.Year, new string[] { "EndingFinancialYear", "Disbursements" })
                    .OrderByDescending(p => p.DateUpdated).Take(10);

                foreach (var project in projects)
                {
                    var disbursements = (from p in project.Disbursements
                                         select (p.Amount * p.ExchangeRate)).Sum();
                    summaryList.Add(new ProjectSummary()
                    {
                        Project = project.Title,
                        Funding = project.ProjectValue,
                        Disbursement = disbursements
                    });
                }
                return summaryList;
            }
        }

        public ICollection<string> GetLocationNames()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => l.Location);
                if (locations.Any())
                {
                    locations = (from l in locations
                                 orderby l ascending
                                 select l);
                }
                return locations.ToList();
            }
        }

        public async Task<ProjectReportView> GetAllProjectsReport(SearchAllProjectsModel model)
        {
            var unitWork = new UnitOfWork(context);
            ProjectReportView projectsReport = new ProjectReportView();
            List<ProjectDetailView> projectsList = new List<ProjectDetailView>();
            List<ProjectDetailSectorView> sectorsList = new List<ProjectDetailSectorView>();
            IQueryable<EFProject> projects;
            string defaultCurrency = "", projectCurrency = "";
            decimal defaultCurrencyExchangeRate = 1, projectValue = 0;
            int startingFinancialYear = 0, endingFinancialYear = 0, currentActiveYear = DateTime.Now.Year;

            if (model.UseDefaultCurrency)
            {
                var currency = unitWork.CurrencyRepository.GetOne(c => c.IsDefault == true);
                if (currency != null)
                {
                    defaultCurrency = currency.Currency;
                }
                var exchangeRate = unitWork.ManualRatesRepository.GetOne(r => r.Year == DateTime.Now.Year);
                if (exchangeRate != null)
                {
                    defaultCurrencyExchangeRate = exchangeRate.ExchangeRate;
                }
            }

            var financialYearsList = unitWork.FinancialYearRepository.GetManyQueryable(y => y.FinancialYear > 0);
            financialYearsList = (from fy in financialYearsList
                                  orderby fy.FinancialYear
                                  select fy);

            var sectors = unitWork.SectorRepository.GetWithInclude(s => s.ParentSectorId != null && s.SectorType.IsPrimary == true, new string[] { "ParentSector" });
            if (sectors.Any())
            {
                var sectorDetailList = (from sec in sectors
                                        orderby sec.SectorName
                          select new { Id = sec.Id, SectorName = sec.SectorName, ParentSector = sec.ParentSector.SectorName });
                foreach(var sec in sectorDetailList)
                {
                    sectorsList.Add(new ProjectDetailSectorView() { Id = sec.Id, Sector = sec.SectorName, ParentSector = sec.ParentSector });
                }
            }
            
            var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => new { l.Id, l.Location });
            var markers = unitWork.MarkerRepository.GetProjection(m => m.Id != 0, m => new { m.Id, m.FieldTitle });
            var financialYearSettings = unitWork.FinancialYearSettingsRepository.GetOne(y => y.Id != 0);

            List<int> filterProjectIds = new List<int>();
            if (model.OrganizationId > 0)
            {
                var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationId == f.FunderId);
                var projectIdsFunders = (from pFunder in projectFunders
                                         select pFunder.ProjectId).ToList<int>().Distinct();

                var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationId == f.ImplementerId);
                var projectIdsImplementers = (from pImplementer in projectImplementers
                                              select pImplementer.ProjectId).ToList<int>().Distinct();

                filterProjectIds = projectIdsFunders.Union(projectIdsImplementers).ToList();
            }

            if (filterProjectIds.Count > 0)
            {
                projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => filterProjectIds.Contains(p.Id), new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers", "Markers.Marker" });
            }
            else
            {
                projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers", "Markers.Marker" });
            }
            
            List<ProjectDetailLocationView> locationsList = new List<ProjectDetailLocationView>();
            if (locations.Any())
            {
                locations = (from l in locations
                             orderby l.Location
                             select l);
                foreach(var location in locations)
                {
                    locationsList.Add(new ProjectDetailLocationView()
                    {
                        Id = location.Id,
                        Location = location.Location
                    });
                }
            }

            List<ProjectDetailMarkerView> markersList = new List<ProjectDetailMarkerView>();
            if (markers.Any())
            {
                markers = (from m in markers
                           orderby m.FieldTitle
                           select m);
                foreach(var marker in markers)
                {
                    markersList.Add(new ProjectDetailMarkerView()
                    {
                        Id = marker.Id,
                        Marker = marker.FieldTitle
                    });
                }
            }

            if (model.StartingYear > 0)
            {
                startingFinancialYear = model.StartingYear;
                projects = (from p in projects
                            where p.StartingFinancialYear.FinancialYear >= model.StartingYear ||
                            p.EndingFinancialYear.FinancialYear >= model.StartingYear
                            select p);
                financialYearsList = (from fy in financialYearsList
                                      where fy.FinancialYear >= startingFinancialYear
                                      select fy);
            }

            if (model.EndingYear > 0)
            {
                endingFinancialYear = model.EndingYear;
                projects = (from p in projects
                            where p.EndingFinancialYear.FinancialYear <= model.EndingYear ||
                            p.StartingFinancialYear.FinancialYear <= model.EndingYear
                            select p);
                financialYearsList = (from fy in financialYearsList
                                      where fy.FinancialYear <= endingFinancialYear
                                      select fy);
            }

            int fyDay = 1, fyMonth = 1, currentMonth = DateTime.Now.Month, currentDay = DateTime.Now.Day;
            if (financialYearSettings != null)
            {
                fyDay = financialYearSettings.Day;
                fyMonth = financialYearSettings.Month;

                if (fyDay != 1 && fyMonth != 1)
                {
                    if (currentMonth < fyMonth)
                    {
                        --currentActiveYear;
                    }
                    else if (currentMonth == fyMonth && currentDay < fyDay)
                    {
                        --currentActiveYear;
                    }
                }
            }

            var financialYears = (from y in financialYearsList
                                  select y.FinancialYear);
            startingFinancialYear = financialYears.Min();
            endingFinancialYear = financialYears.Max();
            List<FinancialYearMiniView> yearsView = new List<FinancialYearMiniView>();
            foreach (var yr in financialYearsList)
            {
                yearsView.Add(new FinancialYearMiniView()
                {
                    FinancialYear = yr.FinancialYear,
                    Label = yr.Label
                });
            }

            foreach (var project in projects)
            {
                IEnumerable<string> funderNames = (from f in project.Funders
                                                   select f.Funder.OrganizationName);
                IEnumerable<string> implementerNames = (from i in project.Implementers
                                                        select i.Implementer.OrganizationName);
                IEnumerable<string> organizations = funderNames.Union(implementerNames);
                List<OrganizationAbstractView> fundersList = new List<OrganizationAbstractView>();
                foreach (string org in funderNames)
                {
                    fundersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }

                List<OrganizationAbstractView> implementersList = new List<OrganizationAbstractView>();
                foreach (string org in implementerNames)
                {
                    implementersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }

                projectValue = project.ProjectValue;
                projectCurrency = project.ProjectCurrency;
                if (model.UseDefaultCurrency)
                {
                    projectCurrency = defaultCurrency;
                    var calculatedExchangeRate = (defaultCurrencyExchangeRate / project.ExchangeRate);
                    projectValue = (calculatedExchangeRate * projectValue);
                    
                    foreach(var disbursement in project.Disbursements)
                    {
                        disbursement.Amount = (defaultCurrencyExchangeRate * disbursement.Amount);
                    }
                }

                projectsList.Add(new ProjectDetailView()
                {
                    Id = project.Id,
                    Title = project.Title.Replace("\"", ""),
                    Description = project.Description,
                    ProjectCurrency = projectCurrency,
                    ProjectValue = projectValue,
                    ExchangeRate = project.ExchangeRate,
                    StartingFinancialYear = project.StartingFinancialYear.FinancialYear,
                    EndingFinancialYear = project.EndingFinancialYear.FinancialYear,
                    Funders = fundersList,
                    Implementers = implementersList,
                    Locations = mapper.Map<List<LocationAbstractView>>(project.Locations),
                    Sectors = mapper.Map<List<SectorAbstractView>>(project.Sectors),
                    Documents = mapper.Map<List<DocumentAbstractView>>(project.Documents),
                    Disbursements = mapper.Map<List<DisbursementAbstractView>>(project.Disbursements),
                    Markers = mapper.Map<List<MarkerAbstractView>>(project.Markers)
                });
            }

            if (sectorsList.Any())
            {
                sectorsList = (from s in sectorsList
                               orderby s.ParentSector
                               select s).ToList();
            }

            projectsReport.UseDefaultCurrency = model.UseDefaultCurrency;
            projectsReport.CurrentFinancialYear = currentActiveYear;
            projectsReport.FinancialYears = yearsView;
            projectsReport.StartingFinancialYear = startingFinancialYear;
            projectsReport.EndingFinancialYear = endingFinancialYear;
            projectsReport.Markers = markersList;
            projectsReport.Locations = locationsList;
            projectsReport.Sectors = sectorsList;
            projectsReport.Projects = projectsList;
            return await Task<ProjectReportView>.Run(() => projectsReport).ConfigureAwait(false);
        }

        public async Task<ProjectDetailReport> GetProjectReport(int id, string reportUrl)
        {
            var unitWork = new UnitOfWork(context);
            ProjectDetailReport report = new ProjectDetailReport();
            report.ReportSettings = new Report()
            {
                Title = ReportConstants.PROJECT_PROFILE_TITLE,
                SubTitle = ReportConstants.PROJECT_PROFILE_SUBTITLE,
                Footer = ReportConstants.PROJECTS_PROFILE_FOOTER,
                Dated = DateTime.Now.ToLongDateString(),
                ReportUrl = reportUrl + ReportConstants.PROJECT_PROFILE_URL + id
            };
            ProjectReportView projectsReport = new ProjectReportView();
            List<ProjectDetailView> projectsList = new List<ProjectDetailView>();
            List<ProjectDetailSectorView> sectorsList = new List<ProjectDetailSectorView>();
            List<OrganizationAbstractView> fundersList = new List<OrganizationAbstractView>();
            List<OrganizationAbstractView> implementersList = new List<OrganizationAbstractView>();
            IQueryable<EFProject> projects;
            int startingFinancialYear = 0, endingFinancialYear = 0, currentActiveYear = DateTime.Now.Year;
            var financialYearsList = unitWork.FinancialYearRepository.GetManyQueryable(y => y.FinancialYear > 0);
            financialYearsList = (from fy in financialYearsList
                                  orderby fy.FinancialYear
                                  select fy);

            var sectors = unitWork.SectorRepository.GetWithInclude(s => s.ParentSectorId != null && s.SectorType.IsPrimary == true, new string[] { "ParentSector" });
            if (sectors.Any())
            {
                var sectorDetailList = (from sec in sectors
                                        orderby sec.SectorName
                                        select new { Id = sec.Id, SectorName = sec.SectorName, ParentSector = sec.ParentSector.SectorName });
                foreach (var sec in sectorDetailList)
                {
                    sectorsList.Add(new ProjectDetailSectorView() { Id = sec.Id, Sector = sec.SectorName, ParentSector = sec.ParentSector });
                }
            }

            var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => new { l.Id, l.Location });
            var markers = unitWork.MarkerRepository.GetProjection(m => m.Id != 0, m => new { m.Id, m.FieldTitle });
            projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id == id, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers", "Markers.Marker" });

            List<ProjectDetailLocationView> locationsList = new List<ProjectDetailLocationView>();
            if (locations.Any())
            {
                locations = (from l in locations
                             orderby l.Location
                             select l);
                foreach (var location in locations)
                {
                    locationsList.Add(new ProjectDetailLocationView()
                    {
                        Id = location.Id,
                        Location = location.Location
                    });
                }
            }

            List<ProjectDetailMarkerView> markersList = new List<ProjectDetailMarkerView>();
            if (markers.Any())
            {
                markers = (from m in markers
                           orderby m.FieldTitle
                           select m);
                foreach (var marker in markers)
                {
                    markersList.Add(new ProjectDetailMarkerView()
                    {
                        Id = marker.Id,
                        Marker = marker.FieldTitle
                    });
                }
            }

            var firstProject = (from p in projects
                           select p).FirstOrDefault();
            if (firstProject != null)
            {
                startingFinancialYear = firstProject.StartingFinancialYear.FinancialYear;
                endingFinancialYear = firstProject.EndingFinancialYear.FinancialYear;
            }

            var financialYearSettings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
            int fyDay = 1, fyMonth = 1, currentMonth = DateTime.Now.Month, currentDay = DateTime.Now.Day;
            if (financialYearSettings != null)
            {
                fyDay = financialYearSettings.Day;
                fyMonth = financialYearSettings.Month;
                if (fyDay != 1 && fyMonth != 1)
                {
                    if (currentMonth < fyMonth)
                    {
                        --currentActiveYear;
                    }
                    else if (currentMonth == fyMonth && currentDay < fyDay)
                    {
                        --currentActiveYear;
                    }
                }
            }

            financialYearsList = (from fy in financialYearsList
                                  where fy.FinancialYear >= startingFinancialYear &&
                                  fy.FinancialYear <= endingFinancialYear
                                  orderby fy.FinancialYear ascending
                                  select fy);
            var financialYears = (from y in financialYearsList
                                  select y.FinancialYear);

            startingFinancialYear = financialYears.Min();
            endingFinancialYear = financialYears.Max();
            List<FinancialYearMiniView> yearsView = new List<FinancialYearMiniView>();
            foreach (var yr in financialYearsList)
            {
                yearsView.Add(new FinancialYearMiniView()
                {
                    FinancialYear = yr.FinancialYear,
                    Label = yr.Label
                });
            }

            foreach (var project in projects)
            {
                IEnumerable<string> funderNames = (from f in project.Funders
                                                   select f.Funder.OrganizationName);
                IEnumerable<string> implementerNames = (from i in project.Implementers
                                                        select i.Implementer.OrganizationName);
                IEnumerable<string> organizations = funderNames.Union(implementerNames);

                foreach (string org in funderNames)
                {
                    fundersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }

                foreach (string org in implementerNames)
                {
                    implementersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }
                projectsList.Add(new ProjectDetailView()
                {
                    Id = project.Id,
                    Title = project.Title.Replace("\"", ""),
                    Description = project.Description,
                    ProjectCurrency = project.ProjectCurrency,
                    ProjectValue = project.ProjectValue,
                    ExchangeRate = project.ExchangeRate,
                    StartDate = project.StartDate.Date.ToString(),
                    EndDate = project.EndDate.Date.ToString(),
                    DateUpdated = project.DateUpdated.Date.ToString(),
                    StartingFinancialYear = project.StartingFinancialYear.FinancialYear,
                    EndingFinancialYear = project.EndingFinancialYear.FinancialYear,
                    Funders = fundersList,
                    Implementers = implementersList,
                    Locations = mapper.Map<List<LocationAbstractView>>(project.Locations),
                    Sectors = mapper.Map<List<SectorAbstractView>>(project.Sectors),
                    Documents = mapper.Map<List<DocumentAbstractView>>(project.Documents),
                    Disbursements = mapper.Map<List<DisbursementAbstractView>>(project.Disbursements),
                    Markers = mapper.Map<List<MarkerAbstractView>>(project.Markers)
                });
            }
            projectsReport.FinancialYears = yearsView;
            projectsReport.CurrentFinancialYear = currentActiveYear;
            projectsReport.StartingFinancialYear = startingFinancialYear;
            projectsReport.EndingFinancialYear = endingFinancialYear;
            projectsReport.Markers = markersList;
            projectsReport.Locations = locationsList;
            projectsReport.Sectors = sectorsList;
            projectsReport.Projects = projectsList;
            report.ProjectProfile = projectsReport;
            return await Task<ProjectDetailReport>.Run(() => report).ConfigureAwait(false);
        }

        public async Task<ProjectProfileReportByLocation> GetProjectsWithoutLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate == 0) ? 1 : exchangeRate;
                ProjectProfileReportByLocation locationProjectsReport = new ProjectProfileReportByLocation();
                try
                {
                    model.LocationOption = NoLocationOptions.ProjectsWithoutLocations;
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForLocationsReport(model);
                    reportUrl += ReportConstants.LOCATION_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    locationProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_LOCATION_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_LOCATION_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_LOCATION_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectLocations> projectLocationsQueryable = null;

                    var projectIds = unitWork.ProjectLocationsRepository.GetProjection(p => p.ProjectId != 0, p => p.ProjectId).Distinct();
                    if (model.ProjectIds.Count > 0)
                    {
                        model.ProjectIds = model.ProjectIds.Except(projectIds).ToList();
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 1970 && model.EndingYear >= 1970)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (!projectIds.Contains(p.Id) && (p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where (!projectIds.Contains(project.Id) && (project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear))
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = (projectIdsFunders.Union(projectIdsImplementers)).Except(projectIds);
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.LocationIds.Count > 0)
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(l => !projectIds.Contains(l.ProjectId) && model.LocationIds.Contains(l.LocationId), new string[] { "Location" });
                    }
                    else
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(p => !projectIds.Contains(p.ProjectId), new string[] { "Location" });
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (!projectIds.Contains(p.Id)) && (p.StartingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.Description = project.Description;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    decimal totalFunding = 0, totalDisbursements = 0, actualDisbursements = 0,
                    plannedDisbursements = 0, totalLocationDisbursements = 0, totalLocationActualDisbursements = 0,
                    totalLocationPlannedDisbursements = 0;

                    List<ProjectsByLocation> locationProjectsList = new List<ProjectsByLocation>();
                    List<ProjectViewForLocation> projectsListForLocation = new List<ProjectViewForLocation>();
                    ProjectsByLocation noLocation = new ProjectsByLocation();
                    noLocation.LocationName = "No Locations Projects";

                    foreach (var project in projectsList)
                    {
                        var projectExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                        totalFunding += Math.Round((project.ProjectValue * (exchangeRate / projectExchangeRate)), MidpointRounding.AwayFromZero);
                        actualDisbursements = (from d in project.Disbursements
                                               where d.DisbursementType == DisbursementTypes.Actual && d.Amount > 0
                                               let disbursements = (d.Amount * (exchangeRate / projectExchangeRate))
                                               select disbursements).Sum();
                        plannedDisbursements = (from d in project.Disbursements
                                                where d.DisbursementType == DisbursementTypes.Planned && d.Amount > 0
                                                let disbursements = (d.Amount * (exchangeRate / projectExchangeRate))
                                                select disbursements).Sum();

                        totalDisbursements = (actualDisbursements + plannedDisbursements);
                        totalLocationActualDisbursements = Math.Round((totalLocationActualDisbursements + actualDisbursements), MidpointRounding.AwayFromZero);
                        totalLocationPlannedDisbursements = Math.Round((totalLocationPlannedDisbursements + plannedDisbursements), MidpointRounding.AwayFromZero);
                        totalLocationDisbursements = Math.Round((totalLocationDisbursements + totalDisbursements), MidpointRounding.AwayFromZero);

                        decimal projectValue = (project.ProjectValue > 0) ? Math.Round((project.ProjectValue * (exchangeRate / projectExchangeRate)), MidpointRounding.AwayFromZero) : 0;
                        projectsListForLocation.Add(new ProjectViewForLocation()
                        {
                            Title = project.Title.Replace("\"", ""),
                            StartingFinancialYear = project.StartingFinancialYear,
                            EndingFinancialYear = project.EndingFinancialYear,
                            Funders = string.Join(", ", project.Funders.Select(f => f.Funder)),
                            Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                            //ProjectValue = projectValue,
                            ProjectValue = (project.ActualDisbursements + project.PlannedDisbursements),
                            ProjectPercentValue = (project.ActualDisbursements + project.PlannedDisbursements),
                            ActualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero),
                            PlannedDisbursements = Math.Round(plannedDisbursements, MidpointRounding.AwayFromZero),
                        });
                    }
                    noLocation.TotalFunding = totalFunding;
                    noLocation.TotalDisbursements = totalLocationDisbursements;
                    noLocation.ActualDisbursements = totalLocationActualDisbursements;
                    noLocation.PlannedDisbursements = totalLocationPlannedDisbursements;
                    noLocation.Projects = projectsListForLocation;

                    locationProjectsList.Add(noLocation);
                    locationProjectsReport.LocationProjectsList = locationProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportByLocation>.Run(() => locationProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportByLocation> GetProjectsByLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate == 0) ? 1 : exchangeRate;
                ProjectProfileReportByLocation locationProjectsReport = new ProjectProfileReportByLocation();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForLocationsReport(model);
                    reportUrl += ReportConstants.LOCATION_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    locationProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_LOCATION_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_LOCATION_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_LOCATION_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    List<int> sectorProjectIds = new List<int>();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectLocations> projectLocationsQueryable = null;
                    List<EFProjectLocations> projectLocations = new List<EFProjectLocations>();
                    IQueryable<EFProjectSectors> projectsInSector = null;
                    List<PercentInProjectView> sectorPercentageInProjects = new List<PercentInProjectView>();

                    if (model.SectorId > 0)
                    {
                        var sectorIds = unitWork.SectorRepository.GetProjection(s => s.ParentSectorId == model.SectorId, s => s.Id);
                        var sectorProjects = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorId.Equals(p.SectorId) || sectorIds.Contains(p.SectorId), new string[] { "Sector" });
                        sectorProjectIds = (from p in sectorProjects
                                            select p.ProjectId).Distinct().ToList<int>();
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => sectorProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                        if (sectorProjectIds.Count > 0)
                        {
                            if (sectorIds.Count() > 0)
                            {
                                projectsInSector = unitWork.ProjectSectorsRepository.GetManyQueryable(s => sectorIds.Contains(s.SectorId) && sectorProjectIds.Contains(s.ProjectId));
                            }
                            else
                            {
                                projectsInSector = unitWork.ProjectSectorsRepository.GetManyQueryable(s => s.SectorId == model.SectorId && sectorProjectIds.Contains(s.ProjectId));
                            }
                        }

                        sectorPercentageInProjects = (from s in projectsInSector
                                                      group s by s.ProjectId into g
                                                      select new PercentInProjectView
                                                      {
                                                          ProjectId = g.Key,
                                                          Percentage = g.Sum(s => s.FundsPercentage)
                                                      }).ToList();
                    }

                    if (model.ProjectIds.Count > 0)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where model.ProjectIds.Contains(p.Id)
                                                  select p);
                        }
                    }

                    if (model.StartingYear > 0 || model.EndingYear > 0)
                    {
                        if (projectProfileList == null)
                        {
                            if (model.StartingYear > 0 && model.EndingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear || (p.EndingFinancialYear.FinancialYear >= model.StartingYear))),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                            }

                            else if (model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                            }

                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                            }
                        }
                        else
                        {
                            if (model.StartingYear > 0 && model.EndingYear <= 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where (p.StartingFinancialYear.FinancialYear >= model.StartingYear || p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                     select p;
                            }
                            else if (model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where (p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)
                                                     select p;
                            }
                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear))
                                                     select p;
                            }
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.MarkerId != 0 || model.MarkerId2 != 0)
                    {
                        List<int> projectIds = new List<int>();
                        List<int> markerIds = new List<int>();
                        var values = model.MarkerValues;
                        var values2 = model.MarkerValues2;

                        for (int v = 0; v < values.Count; v++)
                        {
                            values[v] = WebUtility.UrlDecode(values[v]);
                        }
                        for (int v = 0; v < values2.Count; v++)
                        {
                            values2[v] = WebUtility.UrlDecode(values2[v]);
                        }

                        if (model.MarkerId > 0)
                        {
                            markerIds.Add(model.MarkerId);
                        }
                        if (model.MarkerId2 > 0)
                        {
                            markerIds.Add(model.MarkerId2);
                        }

                        List<int> filterProjectIds = new List<int>();
                        if (projectProfileList != null)
                        {
                            filterProjectIds = (from p in projectProfileList
                                                select p.Id).ToList<int>();
                        }

                        IQueryable<EFProjectMarkers> projectMarkers = null;
                        if (filterProjectIds.Count > 0)
                        {
                            projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(m => filterProjectIds.Contains(m.ProjectId) && markerIds.Contains(m.MarkerId));
                        }
                        else
                        {
                            projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(m => markerIds.Contains(m.MarkerId));
                        }

                        var projectIdsForMarkers = (from p in projectMarkers
                                                    select p.ProjectId).ToList();
                        projectMarkers = (from p in projectMarkers
                                          orderby p.ProjectId
                                          select p);

                        bool isMatch1 = (model.MarkerId == 0) ? true : false; 
                        bool isMatch2 = (model.MarkerId2 == 0) ? true : false;
                        UtilityHelper utilityHelper = new UtilityHelper();
                        int currentProjectId = 0;
                        foreach (var projectMarker in projectMarkers)
                        {
                            if (currentProjectId != 0 && currentProjectId != projectMarker.ProjectId)
                            {
                                if (isMatch1 && isMatch2)
                                {
                                    projectIds.Add(currentProjectId);
                                }
                                isMatch1 = (model.MarkerId == 0) ? true : false;
                                isMatch2 = (model.MarkerId2 == 0) ? true : false;
                            }

                            if (projectMarker.MarkerId == model.MarkerId)
                            {
                                if (values.Count() > 0)
                                {
                                    var markerValues = utilityHelper.ParseAndExtractIfJson(projectMarker.Values);
                                    if (markerValues != null && markerValues.Any())
                                    {
                                        MarkerValues valueMatch = (from v in markerValues
                                                                   where values.Contains(v.Value, StringComparer.OrdinalIgnoreCase)
                                                                   select v).FirstOrDefault();
                                        if (valueMatch != null)
                                        {
                                            isMatch1 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (values.Contains(projectMarker.Values, StringComparer.OrdinalIgnoreCase))
                                        {
                                            isMatch1 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isMatch1 = true;
                                }
                            }
                            else
                            {
                                isMatch1 = true;
                            }

                            if (projectMarker.MarkerId == model.MarkerId2)
                            {
                                if (values2.Count() > 0)
                                {
                                    var markerValues = utilityHelper.ParseAndExtractIfJson(projectMarker.Values);
                                    if (markerValues != null && markerValues.Any())
                                    {
                                        MarkerValues valueMatch = (from v in markerValues
                                                                   where values2.Contains(v.Value, StringComparer.OrdinalIgnoreCase)
                                                                   select v).FirstOrDefault();
                                        if (valueMatch != null)
                                        {
                                            isMatch2 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (values2.Contains(projectMarker.Values, StringComparer.OrdinalIgnoreCase))
                                        {
                                            isMatch2 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isMatch2 = true;
                                }
                            }
                            currentProjectId = projectMarker.ProjectId;
                        }

                        if (isMatch1 && isMatch2 && currentProjectId != 0)
                        {
                            projectIds.Add(currentProjectId);
                            projectIds = projectIds.Distinct().ToList();
                        }

                        if (values.Count == 0 && values2.Count == 0)
                        {
                            projectIds = (from p in projectMarkers
                                          select p.ProjectId).Distinct().ToList();
                        }

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                        }
                        else
                        {
                            if (projectProfileList.Any())
                            {
                                projectProfileList = (from p in projectProfileList
                                                      where projectIds.Contains(p.Id)
                                                      select p);
                            }
                        }
                    }

                    if (model.LocationIds.Count > 0)
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(l => model.LocationIds.Contains(l.LocationId), new string[] { "Location" });
                    }
                    else
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(p => p.ProjectId != 0, new string[] { "Location" });
                    }

                    //List<EFProjectLocations> projectLocationsList = new List<EFProjectLocations>();
                    if (model.SubLocationIds.Count > 0)
                    {
                        foreach(var projectLocation in projectLocationsQueryable)
                        {
                            int[] ids = (!string.IsNullOrEmpty(projectLocation.SubLocationIds)) ? projectLocation.SubLocationIds.Split("-").Select(int.Parse).ToArray() : new int[0];
                            if (model.SubLocationIds.Intersect(ids).Count() > 0)
                            {
                                projectLocations.Add(projectLocation);
                            }
                        }
                        projectLocations = (from pLocation in projectLocations
                                            orderby pLocation.Location.Location
                                            select pLocation).ToList();

                    }
                    else
                    {
                        projectLocations = (from pLocation in projectLocationsQueryable
                                            orderby pLocation.Location.Location
                                            select pLocation).ToList();
                    }

                    

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.Id != 0),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    decimal sectorPercentage = 0;
                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectValue = 0;
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        sectorPercentage = (model.SectorId == 0) ? 1 : 0;
                        if (model.SectorId > 0)
                        {
                            sectorPercentage = (from p in sectorPercentageInProjects
                                                where p.ProjectId == project.Id
                                                select p.Percentage).FirstOrDefault();
                            projectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                            projectValue = (sectorPercentage > 0) ? ((sectorPercentage / 100) * projectValue) : 0;
                        }
                        else
                        {
                            projectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                        }

                        List<ProjectLocationDetailView> projectLocationsList = new List<ProjectLocationDetailView>();
                        foreach(var pLocation in project.Locations)
                        {
                            List<SubLocationMiniView> sublocationsList = new List<SubLocationMiniView>();
                            if (!string.IsNullOrEmpty(pLocation.SubLocations))
                            {
                                sublocationsList = JsonConvert.DeserializeObject<List<SubLocationMiniView>>(pLocation.SubLocations);
                            }
                            projectLocationsList.Add(new ProjectLocationDetailView()
                            {
                                LocationId = pLocation.LocationId,
                                FundsPercentage = pLocation.FundsPercentage,
                                Latitude = (pLocation.Location.Latitude != null) ? (decimal)(pLocation.Location.Latitude) : 0,
                                Longitude = (pLocation.Location.Longitude != null) ? (decimal)(pLocation.Location.Longitude) : 0,
                                SubLocations = sublocationsList
                            });
                        }
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.ProjectValue = projectValue;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ExchangeRate = project.ExchangeRate;
                        profileView.Description = project.Description;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Locations = projectLocationsList;
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    List<ProjectsByLocation> locationProjectsList = new List<ProjectsByLocation>();
                    ProjectsByLocation projectsByLocation = null;

                    int totalLocations = projectLocations.Count();
                    List<ProjectViewForLocation> projectsListForLocation = null;
                    ICollection<LocationProjects> locationsByProjects = new List<LocationProjects>();

                    foreach (var loc in projectLocations)
                    {
                        var isLocationIdsExist = (from locIds in locationsByProjects
                                                  where locIds.LocationId.Equals(loc.LocationId)
                                                  select locIds).FirstOrDefault();

                        if (isLocationIdsExist == null)
                        {
                            locationsByProjects.Add(new LocationProjects()
                            {
                                LocationId = loc.LocationId,
                                Location = loc.Location.Location,
                                Projects = (from locProject in projectLocations
                                            where locProject.LocationId.Equals(loc.LocationId)
                                            select new LocationProject
                                            {
                                                ProjectId = locProject.ProjectId,
                                                FundsPercentage = locProject.FundsPercentage
                                            }).ToList<LocationProject>()
                            });
                        }
                    }

                    if (model.LocationIds.Count == 0)
                    {
                        var currentLocationIds = (from l in projectLocations
                                                  select l.LocationId);
                        var locationsWithNoProjects = unitWork.LocationRepository.GetManyQueryable(l => !currentLocationIds.Contains(l.Id));
                        if (locationsWithNoProjects.Any())
                        {
                            foreach (var location in locationsWithNoProjects)
                            {
                                locationsByProjects.Add(new LocationProjects()
                                {
                                    LocationId = location.Id,
                                    Location = location.Location,
                                    Projects = new List<LocationProject>()
                                });
                            }
                        }
                    }

                    locationsByProjects = (from loc in locationsByProjects
                                           orderby loc.Location
                                           select loc).ToList();
                    foreach (var locationByProject in locationsByProjects)
                    {
                        projectsByLocation = new ProjectsByLocation();
                        projectsByLocation.LocationName = locationByProject.Location;
                        int currentLocationId = locationByProject.LocationId;

                        projectsListForLocation = new List<ProjectViewForLocation>();
                        var projectIds = (from p in locationByProject.Projects
                                          select p.ProjectId);

                        var projectsForLocation = (from project in projectsList
                                                where projectIds.Contains(project.Id)
                                                select project).ToList<ProjectProfileView>();
                        var projectsJson = JsonConvert.SerializeObject(projectsForLocation);
                        var locationProjects = JsonConvert.DeserializeObject<List<ProjectProfileView>>(projectsJson);

                        int projectTotalPercentage = 0;
                        decimal totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, locationPercentage = 0,
                            locationActualDisbursements = 0, locationPlannedDisbursements = 0;
                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.LocationId == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();
                                decimal projectValue = 0;
                                if (locationPercentage > 0)
                                {
                                    projectValue = (project.ProjectValue > 0) ? Math.Round(((locationPercentage / 100) * project.ProjectValue), MidpointRounding.AwayFromZero) : 0;
                                }
                                project.ProjectValue = projectValue;
                                projectTotalPercentage += Convert.ToInt32(locationPercentage);
                                project.ProjectPercentValue = projectValue;
                                //totalFundingPercentage += project.ProjectPercentValue;
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            project.ExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.LocationId == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();
                                if (locationPercentage > 0)
                                {
                                   locationPercentage = (locationPercentage / 100);
                                }
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear > 0 && model.EndingYear > 0)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 && d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }
                                    else if (model.StartingYear > 0 && model.EndingYear <= 0)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }
                                    if (model.StartingYear <= 0 && model.EndingYear > 0)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }
                                    
                                    decimal actualDisbursements = 0, plannedDisbursements = 0;
                                    if (locationPercentage > 0)
                                    {
                                        actualDisbursements = ((((from d in project.Disbursements
                                                                  where d.DisbursementType == DisbursementTypes.Actual && d.Amount > 0
                                                                  select (d.Amount * (exchangeRate / project.ExchangeRate))).Sum())));
                                        plannedDisbursements = ((((from d in project.Disbursements
                                                                   where d.DisbursementType == DisbursementTypes.Planned && d.Amount > 0
                                                                   select (d.Amount * (exchangeRate / project.ExchangeRate))).Sum())));
                                        actualDisbursements = (locationPercentage * actualDisbursements);
                                        plannedDisbursements = (locationPercentage * plannedDisbursements);
                                    }

                                    sectorPercentage = 1;
                                    if (model.SectorId > 0)
                                    {
                                        sectorPercentage = (from s in sectorPercentageInProjects
                                                            where s.ProjectId == project.Id
                                                            select s.Percentage).FirstOrDefault();
                                        sectorPercentage = (sectorPercentage > 0) ? (sectorPercentage / 100) : sectorPercentage;
                                        actualDisbursements = (sectorPercentage * actualDisbursements);
                                        plannedDisbursements = (sectorPercentage * plannedDisbursements);
                                    }

                                    totalDisbursements = (actualDisbursements + plannedDisbursements);
                                    UtilityHelper helper = new UtilityHelper();
                                    project.ActualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero);
                                    project.PlannedDisbursements = Math.Round(plannedDisbursements, MidpointRounding.AwayFromZero);
                                    project.ProjectValue = Math.Round(totalDisbursements, MidpointRounding.AwayFromZero);
                                    totalDisbursementsPercentage += totalDisbursements;
                                    locationActualDisbursements += project.ActualDisbursements;
                                    locationPlannedDisbursements += project.PlannedDisbursements;
                                    totalFundingPercentage += totalDisbursements;
                                }
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            projectsListForLocation.Add(new ProjectViewForLocation()
                            {
                                Title = project.Title.Replace("\"", ""),
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(", ", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = project.ProjectValue,
                                ProjectPercentValue = project.ProjectValue,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }
                        projectsListForLocation = (from pl in projectsListForLocation
                                                   orderby pl.Title ascending
                                                   select pl).ToList();
                        projectsByLocation.TotalFunding = Math.Round(totalFundingPercentage, MidpointRounding.AwayFromZero);
                        projectsByLocation.TotalDisbursements = Math.Round(totalDisbursementsPercentage, MidpointRounding.AwayFromZero);
                        projectsByLocation.ActualDisbursements = Math.Round(locationActualDisbursements, MidpointRounding.AwayFromZero);
                        projectsByLocation.PlannedDisbursements = Math.Round(locationPlannedDisbursements, MidpointRounding.AwayFromZero);
                        projectsByLocation.Projects = projectsListForLocation;
                        locationProjectsList.Add(projectsByLocation);
                    }
                    locationProjectsReport.LocationProjectsList = locationProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportByLocation>.Run(() => locationProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<BudgetReport> GetBudgetReport(string reportUrl, string defaultCurrency, decimal exchangeRate, int chartType = 0)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                BudgetReport budgetReport = new BudgetReport();
                IQueryable<EFProject> projectProfileList = null;
                try
                {
                    string queryString = "?load=true";
                    if (chartType != 0)
                    {
                        queryString += "&ctype=" + chartType;
                    }
                    budgetReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BUDGET_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BUDGET_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BUDGET_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl + ReportConstants.BUDGET_REPORT_URL + queryString
                    };

                    int currentYear = DateTime.Now.Year;
                    int previousYear = (currentYear - 1);
                    int futureYearsLimit = (currentYear + 3);

                    List<ProjectBudgetSummaryView> projectBudgetsList = new List<ProjectBudgetSummaryView>();
                    projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndingFinancialYear.FinancialYear >= currentYear,
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Disbursements", "Disbursements.Year" });

                    projectProfileList = (from p in projectProfileList
                                          orderby p.DateUpdated descending
                                          select p);
                    List<int> projectIds = (from p in projectProfileList
                                            select p.Id).ToList();

                    UtilityHelper utilityHelper = new UtilityHelper();
                    List<YearlyTotalDisbursementsSummary> totalDisbursementsSummaryList = new List<YearlyTotalDisbursementsSummary>();
                    var allSectors = unitWork.SectorRepository.GetWithInclude(s => s.SectorType.IsPrimary == true, new string[] { "SectorType" });
                    var parentSectors = (from sec in allSectors
                                         where sec.ParentSector == null
                                         select sec);
                    var sectors = (from sec in allSectors
                                   where sec.ParentSector != null
                                   select sec);

                    if (sectors.Any())
                    {
                        sectors = (from sector in sectors
                                   orderby sector.SectorName
                                   select sector);
                    }
                    var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => new { l.Id, l.Location });
                    if (locations.Any())
                    {
                        locations = (from location in locations
                                     orderby location.Location
                                     select location);
                    }

                    int fyMonth = 1, fyDay = 1;
                    var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(s => s.Id != 0);
                    if (fySettings != null)
                    {
                        fyMonth = fySettings.Month;
                        fyDay = fySettings.Day;
                    }
                    bool isSimilarToCalendarYear = (fyMonth == 1 && fyDay == 1) ? true : false;
                    List<BudgetFinancialYears> yearsList = new List<BudgetFinancialYears>();
                    var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(y => y.FinancialYear >= previousYear && y.FinancialYear <= futureYearsLimit).ToList();
                    for(int yr=previousYear; yr <= futureYearsLimit; yr++)
                    {
                        var financialYear = (from f in financialYears
                                             where f.FinancialYear == yr
                                             select f).FirstOrDefault();
                        if (financialYear == null)
                        {
                            string label = (isSimilarToCalendarYear) ? ("FY " + yr) : ("FY " + yr + "/" + (yr + 1)); 
                            financialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                            {
                                Label = label,
                                FinancialYear = yr
                            });
                            unitWork.Save();
                            financialYears.Add(financialYear);
                        }
                        yearsList.Add(new BudgetFinancialYears() { Year = yr, Label = financialYear.Label });
                    }
                    var projectDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => projectIds.Contains(d.ProjectId), "FinancialYear");
                    var projectSectors = unitWork.ProjectSectorsRepository.GetManyQueryable(s => projectIds.Contains(s.ProjectId));
                    var projectLocations = unitWork.ProjectLocationsRepository.GetManyQueryable(l => projectIds.Contains(l.ProjectId));
                    List<BudgetSectorDisbursements> yearlySectorDisbursements = new List<BudgetSectorDisbursements>();
                    List<BudgetSectorDisbursements> yearlyParentSectorDisbursements = new List<BudgetSectorDisbursements>();
                    List<BudgetLocationDisbursements> yearlyLocationDisbursements = new List<BudgetLocationDisbursements>();
                    
                    foreach (var sector in sectors)
                    {
                        var sectorProjectIds = (from s in projectSectors
                                                where s.SectorId == sector.Id
                                                select s.ProjectId);

                        BudgetSectorDisbursements sectorDisbursements = new BudgetSectorDisbursements()
                        {
                            ParentSectorId = (int)sector.ParentSectorId,
                            SectorId = sector.Id,
                            SectorName = sector.SectorName,
                        };
                        List<YearlyDisbursements> disbursements = new List<YearlyDisbursements>();
                        for(int yr = previousYear; yr <= futureYearsLimit; yr++)
                        {
                            decimal yearlyDisbursementsTotal = 0;
                            foreach(var project in projectProfileList)
                            {
                                if (project.Sectors.Any())
                                {
                                    var sectorPercentage = (from s in project.Sectors
                                                            where s.SectorId == sector.Id
                                                            select s.FundsPercentage).FirstOrDefault();

                                    if (sectorPercentage > 0)
                                    {
                                        decimal amountPercentage = 0;
                                        if (project.Disbursements.Any())
                                        {
                                            amountPercentage = ((sectorPercentage / 100) * (from d in project.Disbursements
                                                                             where d.Year.FinancialYear == yr
                                                                             select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum());
                                        }
                                        yearlyDisbursementsTotal += amountPercentage;
                                    }
                                }
                            }
                            disbursements.Add(new YearlyDisbursements()
                            {
                                Year = yr,
                                TotalValue = Math.Round(yearlyDisbursementsTotal, MidpointRounding.AwayFromZero)
                            });
                        }
                        sectorDisbursements.Disbursements = disbursements;
                        yearlySectorDisbursements.Add(sectorDisbursements);
                    }

                    foreach(var sector in parentSectors)
                    {
                        int parentSectorId = (sector.ParentSectorId == null) ? 0 : (int)sector.ParentSectorId;
                        BudgetSectorDisbursements sectorDisbursements = new BudgetSectorDisbursements()
                        {
                            ParentSectorId = parentSectorId,
                            SectorId = sector.Id,
                            SectorName = sector.SectorName,
                        };

                        var foundSectors = (from sec in yearlySectorDisbursements
                                            where sec.ParentSectorId == sector.Id
                                            select sec);

                        decimal totalValue = 0;
                        List<YearlyDisbursements> disbursements = new List<YearlyDisbursements>();
                        for (int yr = previousYear; yr <= futureYearsLimit; yr++)
                        {
                            totalValue = 0;
                            foreach(var foundSector in foundSectors)
                            {
                                totalValue += (from d in foundSector.Disbursements
                                               where d.Year == yr
                                               select d.TotalValue).FirstOrDefault();
                            }
                            disbursements.Add(new YearlyDisbursements()
                            {
                                Year = yr,
                                TotalValue = totalValue
                            });
                        }
                        sectorDisbursements.Disbursements = disbursements;
                        yearlyParentSectorDisbursements.Add(sectorDisbursements);
                    }

                    foreach (var location in locations)
                    {
                        var sectorProjectIds = (from l in projectLocations
                                                where l.LocationId == location.Id
                                                select l.ProjectId);

                        BudgetLocationDisbursements locationDisbursements = new BudgetLocationDisbursements()
                        {
                            LocationId = location.Id,
                            LocationName = location.Location,
                        };
                        List<YearlyDisbursements> disbursements = new List<YearlyDisbursements>();
                        for (int yr = previousYear; yr <= futureYearsLimit; yr++)
                        {
                            decimal yearlyDisbursementsTotal = 0;
                            foreach (var project in projectProfileList)
                            {
                                if (project.Locations.Any())
                                {
                                    var locationPercentage = (from l in project.Locations
                                                            where l.LocationId == location.Id
                                                            select l.FundsPercentage).FirstOrDefault();

                                    if (locationPercentage > 0)
                                    {
                                        decimal amountPercentage = 0;
                                        if (project.Disbursements.Any())
                                        {
                                            amountPercentage = ((locationPercentage / 100) * (from d in project.Disbursements
                                                                                            where d.Year.FinancialYear == yr
                                                                                            select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum());
                                        }
                                        yearlyDisbursementsTotal += amountPercentage;
                                    }
                                }
                            }
                            disbursements.Add(new YearlyDisbursements()
                            {
                                Year = yr,
                                TotalValue = Math.Round(yearlyDisbursementsTotal, MidpointRounding.AwayFromZero)
                            });
                        }
                        locationDisbursements.Disbursements = disbursements;
                        yearlyLocationDisbursements.Add(locationDisbursements);
                    }
                    budgetReport.Years = yearsList;
                    budgetReport.ParentSectorDisbursements = yearlyParentSectorDisbursements;
                    budgetReport.SectorDisbursements = yearlySectorDisbursements;
                    budgetReport.LocationDisbursements = yearlyLocationDisbursements;
                }
                catch(Exception ex)
                {
                    string message = ex.Message; 
                }
                return await Task<BudgetReport>.Run(() => budgetReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectsBudgetReportSummary> GetProjectsBudgetReportSummary(string reportUrl, string defaultCurrency, decimal exchangeRate, int chartType = 0)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                if (exchangeRate <= 0)
                {
                    exchangeRate = 1;
                }
                ProjectsBudgetReportSummary budgetReport = new ProjectsBudgetReportSummary();
                IQueryable<EFProject> projectProfileList = null;
                try
                {
                    string queryString = "?load=true";
                    if (chartType != 0)
                    {
                        queryString += "&ctype=" + chartType;
                    }
                    budgetReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BUDGET_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BUDGET_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BUDGET_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl + ReportConstants.BUDGET_REPORT_URL + queryString
                    };

                    int currentYear = DateTime.Now.Year;
                    int previousYear = (currentYear - 1);
                    int futureYearsLimit = (currentYear + 3);
                    List<ProjectBudgetSummaryView> projectBudgetsList = new List<ProjectBudgetSummaryView>();

                    projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndingFinancialYear.FinancialYear >= currentYear,
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Disbursements", "Disbursements.Year" });

                    projectProfileList = (from p in projectProfileList
                                          orderby p.DateUpdated descending
                                          select p);

                    UtilityHelper utilityHelper = new UtilityHelper();
                    List<YearlyTotalDisbursementsSummary> totalDisbursementsSummaryList = new List<YearlyTotalDisbursementsSummary>();
                    foreach (var project in projectProfileList)
                    {
                        ProjectBudgetSummaryView projectBudget = new ProjectBudgetSummaryView();
                        int upperYearLimit = currentYear + 3;
                        int yearsLeft = upperYearLimit - currentYear;
                        int projectStartYear = project.StartingFinancialYear.FinancialYear;
                        projectBudget.Id = project.Id;
                        projectBudget.Title = project.Title.Replace("\"", "");

                        List<ProjectDisbursements> disbursementsList = new List<ProjectDisbursements>();
                        decimal projectCost = 0;

                        List<ProjectYearlyDisbursementsSummary> yearlyDisbursements = new List<ProjectYearlyDisbursementsSummary>();
                        List<ProjectYearlyDisbursementsBreakup> disbursementsBreakup = new List<ProjectYearlyDisbursementsBreakup>();
                        decimal actualDisbursements = 0, expectedDisbursements = 0, disbursements = 0;
                        ++upperYearLimit;
                        for (int year = (currentYear - 1); year < upperYearLimit; year++)
                        {
                            project.ExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                            projectCost = (project.ProjectValue * (exchangeRate / project.ExchangeRate));
                            yearsLeft = upperYearLimit - year;
                            decimal yearDisbursements = Math.Round((from d in project.Disbursements
                                                                    where d.Year.FinancialYear == year && d.DisbursementType == DisbursementTypes.Actual && d.Amount > 0
                                                                    select (d.Amount * (exchangeRate / project.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);

                            actualDisbursements += yearDisbursements;
                            expectedDisbursements = Math.Round((from d in project.Disbursements
                                                                where d.Year.FinancialYear == year && d.DisbursementType == DisbursementTypes.Planned && d.Amount > 0
                                                                select (d.Amount * (exchangeRate / project.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);

                            disbursements = yearDisbursements + expectedDisbursements;
                            actualDisbursements += expectedDisbursements;
                            yearlyDisbursements.Add(new ProjectYearlyDisbursementsSummary()
                            {
                                Year = year,
                                Disbursements = disbursements,
                            });

                            disbursementsBreakup.Add(new ProjectYearlyDisbursementsBreakup()
                            {
                                Year = year,
                                ActualDisbursements = yearDisbursements,
                                ExpectedDisbursements = expectedDisbursements
                            });

                            var yearExists = (from s in totalDisbursementsSummaryList
                                              where s.Year == year
                                              select s).FirstOrDefault();

                            if (yearExists == null)
                            {
                                totalDisbursementsSummaryList.Add(new YearlyTotalDisbursementsSummary()
                                {
                                    Year = year,
                                    TotalDisbursements = yearDisbursements,
                                    TotalExpectedDisbursements = expectedDisbursements
                                });
                            }
                            else
                            {
                                yearExists.TotalDisbursements += yearDisbursements;
                                yearExists.TotalExpectedDisbursements += expectedDisbursements;
                            }
                        }
                        projectBudget.YearlyDisbursements = yearlyDisbursements;
                        projectBudget.DisbursementsBreakup = disbursementsBreakup;
                        projectBudgetsList.Add(projectBudget);
                    }
                    foreach (var total in totalDisbursementsSummaryList)
                    {
                        total.TotalDisbursements = Math.Round(total.TotalDisbursements, MidpointRounding.AwayFromZero);
                        total.TotalExpectedDisbursements = Math.Round(total.TotalExpectedDisbursements, MidpointRounding.AwayFromZero);
                    }
                    budgetReport.TotalYearlyDisbursements = totalDisbursementsSummaryList;
                    projectBudgetsList = (from pl in projectBudgetsList
                                          orderby pl.Title ascending
                                          select pl).ToList();
                    budgetReport.Projects = projectBudgetsList;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<ProjectsBudgetReportSummary>.Run(() => budgetReport).ConfigureAwait(false);
            }
        }

        public async Task<EnvelopeReport> GetEnvelopeReport(SearchEnvelopeModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                if (exchangeRate <= 0)
                {
                    exchangeRate = 1;
                }
                EnvelopeReport envelopeReport = new EnvelopeReport();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForEnvelopeReport(model);
                    reportUrl += ReportConstants.ENVELOPE_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    envelopeReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_ENVELOPE_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_ENVELOPE_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_ENVELOPE_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    int currentYear = DateTime.Now.Year;
                    //int previousYear = currentYear - 1;
                    //int upperYearLimit = currentYear + 1;
                    int previousYear = model.StartingYear;
                    int upperYearLimit = model.EndingYear;
                    if (previousYear == 0 && upperYearLimit == 0)
                    {
                        previousYear = (currentYear - 1);
                        model.StartingYear = previousYear;
                        upperYearLimit = (currentYear + 1);
                        model.EndingYear = upperYearLimit;
                    }

                    var envelopeYears = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(y => y.EnvelopeId != 0, new string[] { "Year" });
                    var financialYears = (from y in envelopeYears
                                         select y.Year.FinancialYear).Distinct();
                    int minYear = (from y in financialYears
                                   select y).Min();
                    int maxYear = (from y in financialYears
                                   select y).Max();

                    if (previousYear == 0 && upperYearLimit != 0)
                    {
                        previousYear = minYear;
                        model.StartingYear = minYear;
                    }

                    if (previousYear != 0 && upperYearLimit == 0)
                    {
                        upperYearLimit = maxYear;
                        model.EndingYear = maxYear;
                    }

                    List<EnvelopeYearlyView> envelopeViewList = new List<EnvelopeYearlyView>();
                    IQueryable<EFEnvelopeYearlyBreakup> envelopeList = null;
                    IQueryable<EFEnvelope> envelopes = null;

                    var envelopeTypes = unitWork.EnvelopeTypesRepository.GetManyQueryable(e => e.Id != 0);
                    envelopeReport.EnvelopeTypes = mapper.Map<List<EnvelopeTypeView>>(envelopeTypes);
                    if (model.FunderIds.Count > 0)
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => model.FunderIds.Contains(e.FunderId), new string[] { "Funder" });
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => model.FunderIds.Contains(e.Envelope.FunderId), new string[] { "Envelope", "Year" });
                    }
                    else if (model.FunderTypeIds.Count > 0)
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => model.FunderTypeIds.Contains((int)e.Funder.OrganizationTypeId), new string[] { "Funder", "Funder.OrganizationType" });
                        var funderIds = (from e in envelopes
                                         select e.FunderId);
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => funderIds.Contains((int)e.Envelope.FunderId), new string[] { "Envelope", "Year" });
                    }
                    else
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => e.FunderId != 0, new string[] { "Funder" });
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => e.Envelope.FunderId != 0, new string[] { "Envelope", "Year" });
                    }

                    if (model.StartingYear > 0)
                    {
                        previousYear = model.StartingYear;
                        envelopeList = (from e in envelopeList
                                        where e.Year.FinancialYear >= model.StartingYear
                                        select e);
                    }

                    if (model.EndingYear > 0)
                    {
                        upperYearLimit = model.EndingYear;
                        envelopeList = (from e in envelopeList
                                        where e.Year.FinancialYear <= model.EndingYear
                                        select e);
                    }

                    if (model.EnvelopeTypeIds.Count > 0)
                    {
                        envelopeList = (from e in envelopeList
                                        where model.EnvelopeTypeIds.Contains(e.EnvelopeTypeId)
                                        select e);

                        envelopeTypes = (from e in envelopeTypes
                                         where model.EnvelopeTypeIds.Contains(e.Id)
                                         select e);
                    }

                    List<int> envelopeYearsList = new List<int>();
                    for (int yr = previousYear; yr <= upperYearLimit; yr++)
                    {
                        envelopeYearsList.Add(yr);
                    }
                    envelopeReport.EnvelopeYears = envelopeYearsList;
                    foreach (var envelope in envelopes)
                    {
                        EnvelopeYearlyView envelopeView = new EnvelopeYearlyView();
                        envelopeView.EnvelopeBreakupsByType = new List<EnvelopeBreakupView>();
                        IQueryable<EFEnvelopeYearlyBreakup> yearlyBreakup = null;
                        envelopeView.Currency = envelope.Currency;
                        envelopeView.ExchangeRate = envelope.ExchangeRate;
                        envelopeView.FunderId = envelope.FunderId;
                        envelopeView.Funder = envelope.Funder.OrganizationName;
                        int envelopeId = envelope.Id;

                        yearlyBreakup = (from yb in envelopeList
                                         where yb.EnvelopeId == envelope.Id
                                         orderby yb.Year.FinancialYear ascending
                                         select yb);

                        IQueryable<EFEnvelopeYearlyBreakup> yearBreakup = null;
                        decimal envelopeTypeTotal = 0;
                        foreach (var type in envelopeTypes)
                        {
                            EnvelopeBreakupView breakupView = new EnvelopeBreakupView();
                            breakupView.EnvelopeType = type.TypeName;
                            breakupView.EnvelopeTypeId = type.Id;
                            envelopeTypeTotal = 0;
                            List<EnvelopeYearlyBreakUp> yearlyBreakupList = new List<EnvelopeYearlyBreakUp>();
                            for (int year = previousYear; year <= upperYearLimit; year++)
                            {
                                if (yearlyBreakup != null)
                                {
                                    yearBreakup = (from yb in yearlyBreakup
                                                   where yb.Year.FinancialYear == year
                                                   select yb);
                                }

                                EFEnvelopeYearlyBreakup isBreakupExists = null;
                                if (yearBreakup != null)
                                {
                                    isBreakupExists = (from typeBreakup in yearBreakup
                                                       where typeBreakup.EnvelopeTypeId == type.Id
                                                       select typeBreakup).FirstOrDefault();
                                }

                                if (isBreakupExists != null)
                                {
                                    envelope.ExchangeRate = (envelope.ExchangeRate <= 0) ? 1 : envelope.ExchangeRate;
                                    var amount = Math.Round(isBreakupExists.Amount * (exchangeRate / envelope.ExchangeRate), MidpointRounding.AwayFromZero);
                                    yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                                    {
                                        Amount = amount,
                                        Year = year
                                    });
                                    envelopeTypeTotal += amount;
                                }
                                else
                                {
                                    yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                                    {
                                        Amount = 0,
                                        Year = year,
                                    });
                                }
                            }

                            //if (envelopeTypeTotal > 0)
                            //{
                                breakupView.YearlyBreakup = yearlyBreakupList;
                                envelopeView.EnvelopeBreakupsByType.Add(breakupView);
                            //}
                        }

                        decimal envelopeValue = 0;
                        foreach(var eType in envelopeView.EnvelopeBreakupsByType)
                        {
                            envelopeValue += (from y in eType.YearlyBreakup
                                              select y.Amount).Sum();
                        }
                        if (envelopeValue > 0)
                        {
                            envelopeViewList.Add(envelopeView);
                        }
                    }
                    envelopeViewList = (from e in envelopeViewList
                                        orderby e.Funder.Trim() ascending
                                        select e).ToList();
                    envelopeReport.Envelope = envelopeViewList;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<EnvelopeReport>.Run(() => envelopeReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportBySector> GetProjectsWithoutSectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
                try
                {
                    model.SectorOption = NoSectorOptions.ProjectsWithoutSectors;
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForSectorsReport(model);
                    reportUrl += ReportConstants.SECTOR_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    sectorProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_SECTOR_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_SECTOR_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_SECTOR_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    List<int> locationProjectIds = new List<int>();

                    var projectIds = unitWork.ProjectSectorsRepository.GetProjection(p => p.ProjectId != 0, p => p.ProjectId).Distinct();
                    if (model.LocationId != 0)
                    {
                        locationProjectIds = unitWork.ProjectLocationsRepository.GetProjection(p => !projectIds.Contains(p.ProjectId) && p.LocationId == model.LocationId, p => p.ProjectId).ToList();
                    }

                    if (locationProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => !projectIds.Contains(p.Id) && locationProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.ProjectIds.Count > 0)
                    {
                        model.ProjectIds = model.ProjectIds.Except(projectIds).ToList();
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where model.ProjectIds.Contains(p.Id)
                                                  select p);
                        }
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => !projectIds.Contains(p.Id) && ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where !projectIds.Contains(project.Id) && project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = (projectIdsFunders.Union(projectIdsImplementers)).Except(projectIds);
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => !projectIds.Contains(p.Id) && (p.EndingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }
                    
                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    decimal totalFunding = 0, totalDisbursements = 0, actualDisbursements = 0,
                    plannedDisbursements = 0, totalSectorDisbursements = 0, totalSectorActualDisbursements = 0,
                    totalSectorPlannedDisbursements = 0;

                    List<ProjectsBySector> sectorProjectsList = new List<ProjectsBySector>();
                    List<ProjectViewForSector> projectsListForSector = new List<ProjectViewForSector>();
                    ProjectsBySector noSector = new ProjectsBySector();
                    noSector.ParentSector = null;
                    noSector.ParentSectorId = 0;
                    noSector.SectorName = "No Sector Projects";

                    foreach(var project in projectsList)
                    {
                        var projectExchangeRate = project.ExchangeRate;
                        totalFunding += Math.Round((project.ProjectValue * (exchangeRate / projectExchangeRate)), MidpointRounding.AwayFromZero);
                        actualDisbursements = (from d in project.Disbursements
                                               where d.DisbursementType == DisbursementTypes.Actual && d.Amount > 0
                                               let disbursements = (d.Amount * (exchangeRate / projectExchangeRate))
                                               select disbursements).Sum();
                        plannedDisbursements = (from d in project.Disbursements
                                               where d.DisbursementType == DisbursementTypes.Planned && d.Amount > 0
                                               let disbursements = (d.Amount * (exchangeRate / projectExchangeRate))
                                               select disbursements).Sum();
                        
                        totalDisbursements = (actualDisbursements + plannedDisbursements);
                        totalSectorActualDisbursements = Math.Round((totalSectorActualDisbursements + actualDisbursements), MidpointRounding.AwayFromZero);
                        totalSectorPlannedDisbursements = Math.Round((totalSectorPlannedDisbursements + plannedDisbursements), MidpointRounding.AwayFromZero);
                        totalSectorDisbursements = Math.Round((totalSectorDisbursements + totalDisbursements), MidpointRounding.AwayFromZero);

                        decimal projectValue = (project.ProjectValue > 0) ? Math.Round((project.ProjectValue * (exchangeRate / projectExchangeRate)), MidpointRounding.AwayFromZero) : project.ProjectValue;
                        projectsListForSector.Add(new ProjectViewForSector()
                        {
                            ProjectId = project.Id,
                            Title = project.Title.Replace("\"", ""),
                            StartingFinancialYear = project.StartingFinancialYear,
                            EndingFinancialYear = project.EndingFinancialYear,
                            Funders = string.Join(", ", project.Funders.Select(f => f.Funder)),
                            Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                            ProjectValue = projectValue,
                            ProjectPercentValue = projectValue,
                            ActualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero),
                            PlannedDisbursements = Math.Round(plannedDisbursements, MidpointRounding.AwayFromZero),
                        });
                    }
                    noSector.TotalFunding = totalFunding;
                    noSector.TotalDisbursements = totalSectorDisbursements;
                    noSector.ActualDisbursements = totalSectorActualDisbursements;
                    noSector.PlannedDisbursements = totalSectorPlannedDisbursements;
                    noSector.Projects = projectsListForSector;
                    
                    sectorProjectsList.Add(noSector);
                    sectorProjectsReport.SectorProjectsList = sectorProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
                sectorProjectsReport.SectorLevel = model.SectorLevel;
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForSectorsReport(model);
                    reportUrl += ReportConstants.SECTOR_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    sectorProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_SECTOR_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_SECTOR_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_SECTOR_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    int defaultSectorTypeId = 0;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectSectors> projectSectors = null;
                    List<int> locationProjectIds = new List<int>();
                    IQueryable<EFProjectLocations> projectsInLocation = null;

                    if (model.LocationId > 0)
                    {
                        List<int> locationIds = new List<int>();
                        if (model.SubLocationIds.Count > 0)
                        {
                            var projectsForLocation = unitWork.ProjectLocationsRepository.GetManyQueryable(p => (p.SubLocationIds != null && p.LocationId == model.LocationId));
                            foreach(var locationProject in projectsForLocation)
                            {
                                int[] subLocationIds = locationProject.SubLocationIds.Split("-").Select(int.Parse).ToArray();
                                if (subLocationIds.Intersect(model.SubLocationIds).Count() > 0)
                                {
                                    if (!locationProjectIds.Contains(locationProject.ProjectId))
                                    {
                                        locationProjectIds.Add(locationProject.ProjectId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            locationProjectIds = unitWork.ProjectLocationsRepository.GetProjection(p => p.LocationId == model.LocationId, p => p.ProjectId).ToList();
                        }
                        
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => locationProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        if (locationProjectIds.Count > 0)
                        {
                            projectsInLocation = unitWork.ProjectLocationsRepository.GetManyQueryable(l => l.LocationId == model.LocationId && locationProjectIds.Contains(l.ProjectId));
                        }
                    }

                    if (model.ProjectIds.Count > 0)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where model.ProjectIds.Contains(p.Id)
                                                  select p);
                        }
                    }

                    if (model.StartingYear > 0 || model.EndingYear > 0)
                    {
                        if (projectProfileList == null)
                        {
                            if (model.StartingYear > 0 && model.EndingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear || (p.EndingFinancialYear.FinancialYear >= model.StartingYear))),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                            }

                            else if(model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                            }

                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                            }
                        }
                        else
                        {
                            if (model.StartingYear > 0 && model.EndingYear <= 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where (p.StartingFinancialYear.FinancialYear >= model.StartingYear || p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                     select p;
                            }
                            else if (model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where (p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)
                                                     select p;
                            }
                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear))
                                                        select p;
                            }
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }
                    
                    List<int> sectorIds = new List<int>();
                    if (model.SectorIds.Count > 0)
                    {
                        if (model.SectorLevel == SectorLevels.Parent)
                        {
                            if (model.SectorIds.Count == 1)
                            {
                                var parentSectorId = model.SectorIds[0];
                                var requiredIds = unitWork.SectorRepository.GetProjection(s => s.ParentSectorId == parentSectorId, s => new { Id = s.Id });
                                var ids = (from id in requiredIds
                                           select id.Id).ToList<int>();
                                projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => ids.Contains(p.SectorId), new string[] { "Sector" });
                            }
                            else
                            {
                                projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains((int)p.Sector.ParentSectorId), new string[] { "Sector" });
                            }
                        }
                        else
                        {
                            projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains(p.SectorId), new string[] { "Sector" });
                        }
                        
                        List<int> projectIdsList = (from s in projectSectors
                                         select s.ProjectId).ToList<int>();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }
                    else
                    {
                        var defaultSectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsPrimary == true);
                        if (defaultSectorType != null)
                        {
                            defaultSectorTypeId = defaultSectorType.Id;
                        }
                        var parentSectorIds = unitWork.SectorRepository.GetProjection(s => (s.IsUnAttributed == true || s.ParentSectorId != null) && s.SectorTypeId == defaultSectorTypeId, s => s.Id);
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => parentSectorIds.Contains(p.SectorId), new string[] { "Sector" });
                        List<int> projectIdsList = new List<int>();
                        if (projectSectors.Any())
                        {
                            projectIdsList = (from s in projectSectors
                                                  select s.ProjectId).ToList();
                        }

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.MarkerId != 0 || model.MarkerId2 != 0)
                    {
                        List<int> projectIds = new List<int>();
                        List<int> markerIds = new List<int>();
                        var values = model.MarkerValues;
                        var values2 = model.MarkerValues2;

                        for(int v = 0; v < values.Count; v++)
                        {
                            values[v] = WebUtility.UrlDecode(values[v]);
                        }
                        for (int v = 0; v < values2.Count; v++)
                        {
                            values2[v] = WebUtility.UrlDecode(values2[v]);
                        }

                        if (model.MarkerId > 0)
                        {
                            markerIds.Add(model.MarkerId);
                        }
                        if (model.MarkerId2 > 0)
                        {
                            markerIds.Add(model.MarkerId2);
                        }

                        List<int> filterProjectIds = new List<int>();
                        if (projectProfileList != null)
                        {
                            filterProjectIds = (from p in projectProfileList
                                                select p.Id).ToList<int>(); 
                        }

                        IQueryable<EFProjectMarkers> projectMarkers = null;
                        if (filterProjectIds.Count > 0)
                        {
                            projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(m => filterProjectIds.Contains(m.ProjectId) && markerIds.Contains(m.MarkerId));
                        }
                        else
                        {
                            projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(m => markerIds.Contains(m.MarkerId));
                        }
                        
                        var projectIdsForMarkers = (from p in projectMarkers
                                          select p.ProjectId).ToList();
                        projectMarkers = (from p in projectMarkers
                                          orderby p.ProjectId
                                          select p);

                        bool isMatch1 = false, isMatch2 = false;
                        UtilityHelper utilityHelper = new UtilityHelper();
                        int currentProjectId = 0;
                        foreach (var projectMarker in projectMarkers)
                        {
                            if (currentProjectId != 0 && currentProjectId != projectMarker.ProjectId)
                            {
                                if (isMatch1 && isMatch2)
                                {
                                    projectIds.Add(currentProjectId);
                                }    
                                isMatch1 = (model.MarkerId == 0) ? true : false;
                                isMatch2 = (model.MarkerId2 == 0) ? true : false;
                            }

                            if (projectMarker.MarkerId == model.MarkerId)
                            {
                                if (values.Count() > 0)
                                {
                                    var markerValues = utilityHelper.ParseAndExtractIfJson(projectMarker.Values);
                                    if (markerValues != null && markerValues.Any())
                                    {
                                        MarkerValues valueMatch = (from v in markerValues
                                                                   where values.Contains(v.Value, StringComparer.OrdinalIgnoreCase)
                                                                   select v).FirstOrDefault();
                                        if (valueMatch != null)
                                        {
                                            isMatch1 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (values.Contains(projectMarker.Values, StringComparer.OrdinalIgnoreCase))
                                        {
                                            isMatch1 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isMatch1 = true;
                                }
                            }
                            else
                            {
                                isMatch1 = true;
                            }

                            if (projectMarker.MarkerId == model.MarkerId2)
                            {
                                if (values2.Count() > 0)
                                {
                                    var markerValues = utilityHelper.ParseAndExtractIfJson(projectMarker.Values);
                                    if (markerValues != null && markerValues.Any())
                                    {
                                        MarkerValues valueMatch = (from v in markerValues
                                                                   where values2.Contains(v.Value, StringComparer.OrdinalIgnoreCase)
                                                                   select v).FirstOrDefault();
                                        if (valueMatch != null)
                                        {
                                            isMatch2 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (values2.Contains(projectMarker.Values, StringComparer.OrdinalIgnoreCase))
                                        {
                                            isMatch2 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isMatch2 = true;
                                }
                            }
                            currentProjectId = projectMarker.ProjectId;
                        }

                        if (isMatch1 && isMatch2 && currentProjectId != 0)
                        {
                            projectIds.Add(currentProjectId);
                            projectIds = projectIds.Distinct().ToList();
                        }

                        if (values.Count == 0 && values2.Count == 0)
                        {
                                projectIds = (from p in projectMarkers
                                              select p.ProjectId).Distinct().ToList();
                        }

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                        }
                        else
                        {
                            if (projectProfileList.Any())
                            {
                                projectProfileList = (from p in projectProfileList
                                                      where projectIds.Contains(p.Id)
                                                      select p);
                            }
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    projectSectors = from pSector in projectSectors
                                     orderby pSector.Sector.SectorName
                                     select pSector;

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    decimal locationPercentage = 0;
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title.Replace("\"", "");
                        profileView.Description = project.Description;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    List<ProjectsBySector> sectorProjectsList = new List<ProjectsBySector>();
                    ProjectsBySector projectsBySector = null;
                    int totalSectors = projectSectors.Count();
                    List<ProjectViewForSector> projectsListForSector = null;
                    ICollection<SectorWithProjects> sectorsByProjects = new List<SectorWithProjects>();

                    var sectors = unitWork.SectorRepository.GetProjection(s => s.Id != 0, s => new { s.Id, s.SectorName, s.IsUnAttributed });
                    sectorProjectsReport.UnAttributedSectorId = (from s in sectors
                                                                 where s.IsUnAttributed == true
                                                                 select s.Id).FirstOrDefault();
                    projectSectors = (from ps in projectSectors
                                      orderby ps.Sector.SectorName
                                      select ps);

                    foreach (var sec in projectSectors)
                    {
                        var isSectorIdsExist = (from secIds in sectorsByProjects
                                                where secIds.SectorId.Equals(sec.SectorId)
                                                select secIds).FirstOrDefault();

                        if (isSectorIdsExist == null)
                        {
                            sectorsByProjects.Add(new SectorWithProjects()
                            {
                                SectorId = sec.SectorId,
                                ParentSectorId = (sec.Sector.ParentSectorId == null) ? 0 : (int)sec.Sector.ParentSectorId,
                                Sector = sec.Sector.SectorName,
                                Projects = (from secProject in projectSectors
                                            where secProject.SectorId.Equals(sec.SectorId)
                                            select new SectorProject
                                            {
                                                ProjectId = secProject.ProjectId,
                                                FundsPercentage = secProject.FundsPercentage
                                            }).ToList<SectorProject>()
                            });
                        }
                    }

                    if (model.SectorIds.Count == 0)
                    {
                        var projectSectorIds = (from s in sectorsByProjects
                                                   select s.SectorId);
                        var sectorsWithoutProjects = unitWork.SectorRepository.GetManyQueryable(s => s.SectorTypeId == defaultSectorTypeId &&
                            !projectSectorIds.Contains(s.Id) && s.ParentSectorId != null);

                        if (sectorsWithoutProjects.Any())
                        {
                            foreach(var sec in sectorsWithoutProjects)
                            {
                                sectorsByProjects.Add(new SectorWithProjects()
                                {
                                    SectorId = sec.Id,
                                    ParentSectorId = (sec.ParentSectorId == null) ? 0 : (int)sec.ParentSectorId,
                                    Sector = sec.SectorName,
                                    Projects = new List<SectorProject>()
                                });
                            }
                        }
                    }

                    sectorsByProjects = (from sec in sectorsByProjects
                                         orderby sec.Sector
                                         select sec).ToList();
                    foreach (var sectorByProject in sectorsByProjects)
                    {
                        projectsBySector = new ProjectsBySector();
                        projectsBySector.SectorId = sectorByProject.SectorId;
                        projectsBySector.SectorName = sectorByProject.Sector;
                        projectsBySector.ParentSectorId = sectorByProject.ParentSectorId;
                        if (projectsBySector.ParentSectorId > 0)
                        {
                            projectsBySector.ParentSector = (from s in sectors
                                                             where s.Id.Equals(projectsBySector.ParentSectorId)
                                                             select s.SectorName).FirstOrDefault();
                        }
                        int currentSectorId = sectorByProject.SectorId;
                        projectsListForSector = new List<ProjectViewForSector>();
                        var projectIds = (from p in sectorByProject.Projects
                                          select p.ProjectId);

                        var projectsForSector = (from project in projectsList
                                              where projectIds.Contains(project.Id)
                                              select project).ToList<ProjectProfileView>();
                        var sectorProjectsJson = JsonConvert.SerializeObject(projectsForSector);
                        var sectorProjects = JsonConvert.DeserializeObject<List<ProjectProfileView>>(sectorProjectsJson);

                        decimal totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, sectorPercentage = 0,
                            sectorActualDisbursements = 0, sectorPlannedDisbursements = 0;
                        
                        foreach (var project in sectorProjects)
                        {
                            locationPercentage = 1;
                            if (model.LocationId > 0)
                            {
                                locationPercentage = (from p in projectsInLocation
                                                      where p.ProjectId == project.Id
                                                      select p.FundsPercentage).FirstOrDefault();
                                locationPercentage = (locationPercentage > 0) ? (locationPercentage / 100) : 0;
                            }
                            if (project.Sectors != null)
                            {
                                sectorPercentage = (from s in project.Sectors
                                                    where s.SectorId == currentSectorId
                                                    select s.FundsPercentage).FirstOrDefault();

                                decimal projectValue = 0;
                                //project.ExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                                if (sectorPercentage > 0)
                                {
                                    project.ProjectPercentValue = (project.ProjectValue > 0) ? Math.Round(( (sectorPercentage / 100) * (locationPercentage * (project.ProjectValue * (exchangeRate/project.ExchangeRate)))), MidpointRounding.AwayFromZero) : 0;
                                    project.ProjectValue = project.ProjectPercentValue;
                                }
                                else
                                {
                                    project.ProjectPercentValue = projectValue;
                                    project.ProjectValue = projectValue;
                                }
                                //totalFundingPercentage += project.ProjectPercentValue;
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            locationPercentage = 1;
                            if (model.LocationId > 0)
                            {
                                locationPercentage = (from p in projectsInLocation
                                                      where p.ProjectId == project.Id
                                                      select p.FundsPercentage).FirstOrDefault();
                                locationPercentage = (locationPercentage > 0) ? (locationPercentage / 100) : 0;
                            }
                            if (project.Sectors != null)
                            {
                                sectorPercentage = (from s in project.Sectors
                                                    where s.SectorId == currentSectorId
                                                    select s.FundsPercentage).FirstOrDefault();
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear > 0 && model.EndingYear > 0)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 && d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }
                                    else if (model.StartingYear > 0 && model.EndingYear <= 0)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }
                                    if (model.StartingYear <= 0 && model.EndingYear > 0)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }

                                    decimal actualDisbursements = 0, plannedDisbursements = 0;
                                    actualDisbursements = Math.Round((locationPercentage * ((from d in project.Disbursements
                                                                                             where d.DisbursementType == DisbursementTypes.Actual && d.Amount > 0
                                                                                             select (d.Amount * (exchangeRate / project.ExchangeRate))).Sum())), MidpointRounding.AwayFromZero);
                                    plannedDisbursements = Math.Round((locationPercentage * (from d in project.Disbursements
                                                                                             where d.DisbursementType == DisbursementTypes.Planned && d.Amount > 0
                                                                                             select (d.Amount * (exchangeRate / project.ExchangeRate))).Sum()), MidpointRounding.AwayFromZero);
                                    UtilityHelper helper = new UtilityHelper();
                                    if (sectorPercentage > 0)
                                    {
                                        project.ActualDisbursements = Math.Round(((sectorPercentage / 100) * actualDisbursements), MidpointRounding.AwayFromZero);
                                        project.PlannedDisbursements = Math.Round(((sectorPercentage / 100) * plannedDisbursements), MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        project.ActualDisbursements = 0;
                                        project.PlannedDisbursements = 0;
                                    }
                                    
                                    if (project.PlannedDisbursements < 0)
                                    {
                                        project.PlannedDisbursements = 0;
                                    }
                                    totalDisbursements = Math.Round((project.ActualDisbursements + project.PlannedDisbursements), MidpointRounding.AwayFromZero);
                                    //This is where project value is calculated using disbursements
                                    project.ProjectValue = totalDisbursements;
                                    totalDisbursementsPercentage += totalDisbursements;
                                    sectorActualDisbursements += project.ActualDisbursements;
                                    sectorPlannedDisbursements += project.PlannedDisbursements;
                                    totalFundingPercentage += totalDisbursements;
                                }
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            string projectTitle = project.Title.Replace("“", "");
                            projectTitle = projectTitle.Replace("”", "");
                            projectTitle = projectTitle.Replace("\"", "");
                            projectsListForSector.Add(new ProjectViewForSector()
                            {
                                ProjectId = project.Id,
                                Title = projectTitle,
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(", ", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = project.ProjectValue,
                                ProjectPercentValue = (project.ActualDisbursements + project.PlannedDisbursements),
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        projectsListForSector = (from pl in projectsListForSector
                                                 orderby pl.Title ascending
                                                 select pl).ToList();

                        projectsBySector.TotalFunding = totalFundingPercentage;
                        projectsBySector.TotalDisbursements = totalDisbursementsPercentage;
                        projectsBySector.ActualDisbursements = sectorActualDisbursements;
                        projectsBySector.PlannedDisbursements = sectorPlannedDisbursements;
                        projectsBySector.Projects = projectsListForSector;
                        sectorProjectsList.Add(projectsBySector);
                    }
                    sectorProjectsReport.SectorProjectsList = sectorProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<TimeSeriesReportByYear> GetProjectsByYear(SearchProjectsByYearModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                TimeSeriesReportByYear timeSeriesReportByYear = new TimeSeriesReportByYear();
                try
                {
                    List<int> sectorIdsForPercentage = new List<int>();
                    exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForTimeSeriesReport(model);
                    reportUrl += ReportConstants.YEARLY_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    timeSeriesReportByYear.ReportSettings = new Report()
                    {
                        Title = ReportConstants.TIME_SERIES_REPORT_TITLE,
                        SubTitle = ReportConstants.TIME_SERIES_REPORT_SUBTITLE,
                        Footer = ReportConstants.TIME_SERIES_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IEnumerable<int> financialYears = null;

                    if (model.LocationId > 0)
                    {
                        List<int> locationProjectIds = new List<int>();
                        if (model.SubLocationIds.Count > 0)
                        {
                            var projectsForLocation = unitWork.ProjectLocationsRepository.GetManyQueryable(p => (p.SubLocationIds != null && p.LocationId == model.LocationId));
                            foreach (var locationProject in projectsForLocation)
                            {
                                int[] subLocationIds = locationProject.SubLocationIds.Split("-").Select(int.Parse).ToArray();
                                if (subLocationIds.Intersect(model.SubLocationIds).Count() > 0)
                                {
                                    if (!locationProjectIds.Contains(locationProject.ProjectId))
                                    {
                                        locationProjectIds.Add(locationProject.ProjectId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            locationProjectIds = unitWork.ProjectLocationsRepository.GetProjection(p => p.LocationId == model.LocationId, p => p.ProjectId).ToList();
                        }
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => locationProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.ProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear > 0 || model.EndingYear > 0)
                    {
                        if (projectProfileList == null)
                        {
                            if (model.StartingYear > 0 && model.EndingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear || (p.EndingFinancialYear.FinancialYear >= model.StartingYear))),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                            }

                            else if (model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                            }

                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                            }
                        }
                        else
                        {
                            if (model.StartingYear > 0 && model.EndingYear <= 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where (p.StartingFinancialYear.FinancialYear >= model.StartingYear || p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                     select p;
                            }
                            else if (model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where (p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)
                                                     select p;
                            }
                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = from p in projectProfileList
                                                     where ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear))
                                                     select p;
                            }
                        }
                    }
                    
                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectIdsFunders = unitWork.ProjectFundersRepository.GetProjection(f => model.OrganizationIds.Contains(f.FunderId), f => f.ProjectId);
                        var projectIdsImplementers = unitWork.ProjectImplementersRepository.GetProjection(f => model.OrganizationIds.Contains(f.ImplementerId), i => i.ProjectId);
                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        var projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains(p.SectorId) || model.SectorIds.Contains((int)p.Sector.ParentSectorId), new string[] { "Sector" });
                        List<int> projectIdsList = (from s in projectSectors
                                                    select s.ProjectId).ToList<int>();
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                        var sectorIds = unitWork.SectorRepository.GetProjection(s => model.SectorIds.Contains(s.Id) || model.SectorIds.Contains((int)s.ParentSectorId), s => new { s.Id });
                        sectorIdsForPercentage = (from s in sectorIds
                                                  select s.Id).ToList();
                    }

                    if (model.MarkerId != 0 || model.MarkerId2 != 0)
                    {
                        List<int> projectIds = new List<int>();
                        List<int> markerIds = new List<int>();
                        var values = model.MarkerValues;
                        var values2 = model.MarkerValues2;

                        for (int v = 0; v < values.Count; v++)
                        {
                            values[v] = WebUtility.UrlDecode(values[v]);
                        }
                        for (int v = 0; v < values2.Count; v++)
                        {
                            values2[v] = WebUtility.UrlDecode(values2[v]);
                        }

                        if (model.MarkerId > 0)
                        {
                            markerIds.Add(model.MarkerId);
                        }
                        if (model.MarkerId2 > 0)
                        {
                            markerIds.Add(model.MarkerId2);
                        }

                        List<int> filterProjectIds = new List<int>();
                        if (projectProfileList != null)
                        {
                            filterProjectIds = (from p in projectProfileList
                                                select p.Id).ToList<int>();
                        }

                        IQueryable<EFProjectMarkers> projectMarkers = null;
                        if (filterProjectIds.Count > 0)
                        {
                            projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(m => filterProjectIds.Contains(m.ProjectId) && markerIds.Contains(m.MarkerId));
                        }
                        else
                        {
                            projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(m => markerIds.Contains(m.MarkerId));
                        }

                        var projectIdsForMarkers = (from p in projectMarkers
                                                    select p.ProjectId).ToList();
                        projectMarkers = (from p in projectMarkers
                                          orderby p.ProjectId
                                          select p);

                        bool isMatch1 = (model.MarkerId == 0) ? true : false;
                        bool isMatch2 = (model.MarkerId2 == 0) ? true : false;
                        UtilityHelper utilityHelper = new UtilityHelper();
                        int currentProjectId = 0;
                        foreach (var projectMarker in projectMarkers)
                        {
                            if (currentProjectId != 0 && currentProjectId != projectMarker.ProjectId)
                            {
                                if (isMatch1 && isMatch2)
                                {
                                    projectIds.Add(currentProjectId);
                                }
                                isMatch1 = (model.MarkerId == 0) ? true : false;
                                isMatch2 = (model.MarkerId2 == 0) ? true : false;
                            }

                            if (projectMarker.MarkerId == model.MarkerId)
                            {
                                if (values.Count() > 0)
                                {
                                    var markerValues = utilityHelper.ParseAndExtractIfJson(projectMarker.Values);
                                    if (markerValues != null && markerValues.Any())
                                    {
                                        MarkerValues valueMatch = (from v in markerValues
                                                                   where values.Contains(v.Value, StringComparer.OrdinalIgnoreCase)
                                                                   select v).FirstOrDefault();
                                        if (valueMatch != null)
                                        {
                                            isMatch1 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (values.Contains(projectMarker.Values, StringComparer.OrdinalIgnoreCase))
                                        {
                                            isMatch1 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isMatch1 = true;
                                }
                            }
                            else
                            {
                                isMatch1 = true;
                            }

                            if (projectMarker.MarkerId == model.MarkerId2)
                            {
                                if (values2.Count() > 0)
                                {
                                    var markerValues = utilityHelper.ParseAndExtractIfJson(projectMarker.Values);
                                    if (markerValues != null && markerValues.Any())
                                    {
                                        MarkerValues valueMatch = (from v in markerValues
                                                                   where values2.Contains(v.Value, StringComparer.OrdinalIgnoreCase)
                                                                   select v).FirstOrDefault();
                                        if (valueMatch != null)
                                        {
                                            isMatch2 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (values2.Contains(projectMarker.Values, StringComparer.OrdinalIgnoreCase))
                                        {
                                            isMatch2 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    isMatch2 = true;
                                }
                            }
                            currentProjectId = projectMarker.ProjectId;
                        }

                        if (isMatch1 && isMatch2 && currentProjectId != 0)
                        {
                            projectIds.Add(currentProjectId);
                            projectIds = projectIds.Distinct().ToList();
                        }

                        if (values.Count == 0 && values2.Count == 0)
                        {
                            projectIds = (from p in projectMarkers
                                          select p.ProjectId).Distinct().ToList();
                        }

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers", "Markers.Marker" });
                        }
                        else
                        {
                            if (projectProfileList.Any())
                            {
                                projectProfileList = (from p in projectProfileList
                                                      where projectIds.Contains(p.Id)
                                                      select p);
                            }
                        }
                    }

                    if (model.StartingYear > 0 && model.EndingYear > 0)
                    {
                        financialYears = unitWork.FinancialYearRepository.GetProjection(y => (y.FinancialYear >= model.StartingYear && y.FinancialYear <= model.EndingYear), y => y.FinancialYear);
                    }
                    else if (model.StartingYear > 0 && model.EndingYear == 0)
                    {
                        financialYears = unitWork.FinancialYearRepository.GetProjection(y => (y.FinancialYear >= model.StartingYear), y => y.FinancialYear);
                    }
                    else
                    {
                        financialYears = unitWork.FinancialYearRepository.GetProjection(y => (y.FinancialYear != 0), y => y.FinancialYear);
                    }

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    List<int> reportProjectIds = new List<int>();
                    foreach (var project in projectProfileList)
                    {
                        reportProjectIds.Add(project.Id);
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        //profileView.ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    ProjectsByYear projectsByYear = null;
                    int totalYears = financialYears.Count();
                    List<ProjectsByYear> projectsListForYear = new List<ProjectsByYear>();
                    List<ProjectViewForYear> projectsViewForYear = new List<ProjectViewForYear>();
                    ICollection<YearWithProjects> yearProjects = new List<YearWithProjects>();

                    foreach (var yr in financialYears)
                    {
                        var isYearExists = (from y in yearProjects
                                            where y.Equals(yr)
                                            select y).FirstOrDefault();

                        if (isYearExists == null)
                        {
                            yearProjects.Add(new YearWithProjects()
                            {
                                Year = yr,
                                Projects = (from p in projectProfileList
                                            where (yr >= p.StartingFinancialYear.FinancialYear && yr <= p.EndingFinancialYear.FinancialYear)
                                            select p.Id).ToList<int>()
                            });
                        }
                    }

                    IEnumerable<PercentInProjectView> locationPercentageInProjects = new List<PercentInProjectView>();
                    if (model.LocationId > 0)
                    {
                        locationPercentageInProjects = unitWork.ProjectLocationsRepository.GetProjection(p => reportProjectIds.Contains(p.ProjectId), p => new PercentInProjectView() { ProjectId = p.ProjectId, Percentage = p.FundsPercentage });
                    }

                    List<PercentInProjectView> sectorPercentageInProjects = new List<PercentInProjectView>();
                    if (model.SectorIds.Count > 0)
                    {
                        var projectsWithPercentage = unitWork.ProjectSectorsRepository.GetProjection(s => sectorIdsForPercentage.Contains(s.SectorId), s => new PercentInProjectView { ProjectId = s.ProjectId, Percentage = s.FundsPercentage });
                        sectorPercentageInProjects = (from s in projectsWithPercentage
                                                      group s by s.ProjectId into g
                                                      select new PercentInProjectView
                                                      {
                                                          ProjectId = g.Key,
                                                          Percentage = g.Sum(s => s.Percentage)
                                                      }).ToList();
                    }

                    foreach (var yearProject in yearProjects)
                    {
                        projectsByYear = new ProjectsByYear();
                        int currentYearId = yearProject.Year;

                        projectsViewForYear = new List<ProjectViewForYear>();
                        var projectIds = yearProject.Projects;
                        var projectsForYear = (from project in projectsList
                                                     where projectIds.Contains(project.Id)
                                                     select project).ToList<ProjectProfileView>();
                        var projectsJson = JsonConvert.SerializeObject(projectsForYear);
                        var yearlyProjectsProfile = JsonConvert.DeserializeObject<List<ProjectProfileView>>(projectsJson);

                        decimal totalFunding = 0, totalDisbursements = 0, totalProjectValue = 0, totalPlannedDisbursements = 0,
                            totalActualDisbursements = 0;
                        foreach (var project in yearlyProjectsProfile)
                        {
                            project.ExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                            totalProjectValue += (project.ProjectValue * (exchangeRate / project.ExchangeRate));

                            if (project.Disbursements.Count() > 0)
                            {
                                if (model.StartingYear > 0 && model.EndingYear > 0)
                                {
                                    project.Disbursements = (from d in project.Disbursements
                                                             where d.Year >= model.StartingYear
                                                             && d.Year <= model.EndingYear
                                                             select d).ToList();
                                }
                                else if (model.StartingYear > 0 && model.EndingYear <= 0)
                                {
                                    project.Disbursements = (from d in project.Disbursements
                                                             where d.Year >= model.StartingYear
                                                             select d).ToList();
                                }
                                if (model.StartingYear <= 0 && model.EndingYear > 0)
                                {
                                    project.Disbursements = (from d in project.Disbursements
                                                             where d.Year <= model.EndingYear
                                                             select d).ToList();
                                }
                                var disbursements = (from d in project.Disbursements
                                                     select d).ToList();

                                decimal locationPercentage = 1;
                                decimal sectorPercentage = 1;
                                if (model.LocationId > 0)
                                {
                                    locationPercentage = (from p in locationPercentageInProjects
                                                          where p.ProjectId == project.Id
                                                          select p.Percentage).FirstOrDefault();
                                    if (locationPercentage > 0)
                                    {
                                        locationPercentage = (locationPercentage / 100);
                                    }
                                }
                                
                                if (model.SectorIds.Count > 1)
                                {
                                    sectorPercentage = (from p in sectorPercentageInProjects
                                                        where p.ProjectId == project.Id
                                                        select p.Percentage).FirstOrDefault();
                                    if (sectorPercentage > 0)
                                    {
                                        sectorPercentage = (sectorPercentage / 100);
                                    }
                                }

                                decimal projectDisbursements = Math.Round((from d in disbursements
                                                                           where d.DisbursementType == DisbursementTypes.Actual && d.Year == currentYearId && d.Amount > 0
                                                                           select (((d.Amount * locationPercentage) * sectorPercentage) * (exchangeRate / project.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                decimal plannedDisbursements = Math.Round((from d in disbursements
                                                                           where d.DisbursementType == DisbursementTypes.Planned && d.Year == currentYearId && d.Amount > 0
                                                                           select (((d.Amount * locationPercentage) * sectorPercentage) * (exchangeRate / project.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                totalDisbursements += projectDisbursements;
                                UtilityHelper helper = new UtilityHelper();
                                project.ActualDisbursements = projectDisbursements;
                                project.PlannedDisbursements = plannedDisbursements;
                                if (project.PlannedDisbursements < 0)
                                {
                                    project.PlannedDisbursements = 0;
                                }
                                totalFunding += (project.ActualDisbursements + project.PlannedDisbursements);
                                totalPlannedDisbursements += project.PlannedDisbursements;
                                totalActualDisbursements += project.ActualDisbursements;
                            }

                            projectsViewForYear.Add(new ProjectViewForYear()
                            {
                                Title = project.Title.Replace("\"", ""),
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(", ", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = project.ProjectValue,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        if (projectsByYear != null)
                        {
                            projectsViewForYear = (from pv in projectsViewForYear
                                                   orderby pv.Title ascending
                                                   select pv).ToList();
                            projectsByYear.Year = currentYearId;
                            projectsByYear.TotalFunding = totalFunding;
                            projectsByYear.TotalDisbursements = totalDisbursements;
                            projectsByYear.TotalProjectValue = totalProjectValue;
                            projectsByYear.TotalActualDisbursements = totalActualDisbursements;
                            projectsByYear.TotalPlannedDisbursements = totalPlannedDisbursements;
                            projectsByYear.Projects = projectsViewForYear;
                            projectsListForYear.Add(projectsByYear);
                        }
                        else
                        {
                            projectsListForYear.Add(new ProjectsByYear()
                            {
                                Year = currentYearId,
                                Projects = new List<ProjectViewForYear>(),
                                TotalFunding = 0,
                                TotalDisbursements = 0,
                            });
                        }
                    }
                    timeSeriesReportByYear.YearlyProjectsList = projectsListForYear;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<TimeSeriesReportByYear>.Run(() => timeSeriesReportByYear).ConfigureAwait(false);
            }
        }

        public decimal GetExchangeRateForCurrency(string currency, List<CurrencyWithRates> ratesList)
        {
            return (from rate in ratesList
                    where rate.Currency.Equals(currency)
                    select rate.Rate).FirstOrDefault();
        }

        /*private IEnumerable<MarkerValues> ParseAndExtractIfJson(String json)
        {
            List<MarkerValues> markerValues = new List<MarkerValues>();
            try
            {
                markerValues = JsonConvert.DeserializeObject<List<MarkerValues>>(json);
            }
            catch (Exception)
            {
            }
            return markerValues;
        }*/

    }
}
