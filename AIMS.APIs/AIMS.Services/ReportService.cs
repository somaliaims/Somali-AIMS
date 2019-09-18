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

namespace AIMS.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Get projects report by sectors and title
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Task<ProjectProfileReportBySector> GetProjectsBySector(ReportModelForProjectSectors model);

        /// <summary>
        /// Search matching projects by sector wise grouped for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search matching projects by sector wise grouped for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportByLocation> GetProjectsByLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

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
        Task<ProjectsBudgetReportSummary> GetProjectsBudgetReportSummary(string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Gets envelope report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        Task<EnvelopeReport> GetEnvelopeReport(SearchEnvelopeModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

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
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectLocations> projectLocationsQueryable = null;
                    List<EFProjectLocations> projectLocations = null;

                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 1970 && model.EndingYear >= 1970)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartingFinancialYear.FinancialYear >= model.StartingYear
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


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

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
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(l => model.LocationIds.Contains(l.LocationId), new string[] { "Location" });
                    }
                    else
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(p => p.ProjectId != 0, new string[] { "Location" });
                    }

                    projectLocations = (from pLocation in projectLocationsQueryable
                                        orderby pLocation.Location.Location
                                        select pLocation).ToList();

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.StartingFinancialYear.FinancialYear >= year),
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
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
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

                    foreach (var locationByProject in locationsByProjects)
                    {
                        projectsByLocation = new ProjectsByLocation();
                        projectsByLocation.LocationName = locationByProject.Location;
                        int currentLocationId = locationByProject.LocationId;

                        projectsListForLocation = new List<ProjectViewForLocation>();
                        var projectIds = (from p in locationByProject.Projects
                                          select p.ProjectId);

                        var locationProjects = (from project in projectsList
                                                where projectIds.Contains(project.Id)
                                                select project).ToList<ProjectProfileView>();

                        int projectTotalPercentage = 0;
                        decimal totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, locationPercentage = 0;
                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.LocationId == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();

                                projectTotalPercentage += Convert.ToInt32(locationPercentage);
                                project.ProjectPercentValue = Math.Round(((project.ProjectValue / 100) * locationPercentage), MidpointRounding.AwayFromZero);
                                totalFundingPercentage += project.ProjectPercentValue;
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.LocationId == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }


                                    decimal actualDisbursements = ((((from d in project.Disbursements
                                                                      where d.DisbursementType == DisbursementTypes.Actual
                                                                      select d.Amount).Sum()) / 100) * locationPercentage);
                                    decimal plannedDisbursements = ((((from d in project.Disbursements
                                                                       where d.DisbursementType == DisbursementTypes.Planned
                                                                       select d.Amount).Sum()) / 100) * locationPercentage);

                                    totalDisbursements += actualDisbursements;

                                    UtilityHelper helper = new UtilityHelper();
                                    project.ActualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero);
                                    project.PlannedDisbursements = Math.Round(plannedDisbursements, MidpointRounding.AwayFromZero);
                                    totalDisbursementsPercentage += project.ActualDisbursements;
                                }
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            projectsListForLocation.Add(new ProjectViewForLocation()
                            {
                                Title = project.Title,
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = project.ProjectValue,
                                ProjectPercentValue = project.ProjectPercentValue,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }
                        projectsByLocation.TotalFunding = Math.Round(totalFundingPercentage, MidpointRounding.AwayFromZero);
                        projectsByLocation.TotalDisbursements = Math.Round(totalDisbursementsPercentage, MidpointRounding.AwayFromZero);
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

        public async Task<ProjectsBudgetReportSummary> GetProjectsBudgetReportSummary(string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                if (exchangeRate == 0)
                {
                    exchangeRate = 1;
                }
                ProjectsBudgetReportSummary budgetReport = new ProjectsBudgetReportSummary();
                IQueryable<EFProject> projectProfileList = null;
                try
                {
                    budgetReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BUDGET_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BUDGET_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BUDGET_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl + ReportConstants.BUDGET_REPORT_URL + "?load=true"
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
                        projectBudget.Title = project.Title;

                        List<ProjectDisbursements> disbursementsList = new List<ProjectDisbursements>();
                        decimal projectCost = 0;

                        List<ProjectYearlyDisbursementsSummary> yearlyDisbursements = new List<ProjectYearlyDisbursementsSummary>();
                        List<ProjectYearlyDisbursementsBreakup> disbursementsBreakup = new List<ProjectYearlyDisbursementsBreakup>();
                        decimal actualDisbursements = 0, expectedDisbursements = 0, disbursements = 0;
                        ++upperYearLimit;
                        for (int year = (currentYear - 1); year < upperYearLimit; year++)
                        {
                            decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                            projectCost = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                            yearsLeft = upperYearLimit - year;
                            decimal yearDisbursements = Math.Round((from d in project.Disbursements
                                                                    where d.Year.FinancialYear == year && d.DisbursementType == DisbursementTypes.Actual
                                                                    select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);

                            actualDisbursements += yearDisbursements;
                            expectedDisbursements = Math.Round((from d in project.Disbursements
                                                                where d.Year.FinancialYear == year && d.DisbursementType == DisbursementTypes.Planned
                                                                select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);

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
                EnvelopeReport envelopeReport = new EnvelopeReport();
                try
                {
                    envelopeReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_ENVELOPE_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_ENVELOPE_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_ENVELOPE_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl + ReportConstants.ENVELOPE_REPORT_URL + "?load=true"
                    };

                    int currentYear = DateTime.Now.Year;
                    int previousYear = currentYear - 1;
                    int upperYearLimit = currentYear + 1;
                    List<EnvelopeYearlyView> envelopeViewList = new List<EnvelopeYearlyView>();
                    IQueryable<EFEnvelopeYearlyBreakup> envelopeList = null;
                    IQueryable<EFEnvelope> envelopes = null;



                    var envelopeTypes = unitWork.EnvelopeTypesRepository.GetManyQueryable(e => e.Id != 0);
                    if (model.FunderId != 0)
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => e.FunderId == model.FunderId, new string[] { "Funder" });
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => e.Envelope.FunderId == model.FunderId, new string[] { "Envelope", "Year" });
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

                    if (model.EnvelopeTypeId > 0)
                    {
                        envelopeList = (from e in envelopeList
                                        where e.EnvelopeId == model.EnvelopeTypeId
                                        select e);
                    }

                    foreach (var envelope in envelopes)
                    {
                        EnvelopeYearlyView envelopeView = new EnvelopeYearlyView();
                        envelopeView.EnvelopeBreakupsByType = new List<EnvelopeBreakupView>();
                        IQueryable<EFEnvelopeYearlyBreakup> yearlyBreakup = null;
                        envelopeView.Currency = envelope.Currency;
                        envelopeView.FunderId = envelope.FunderId;
                        envelopeView.Funder = envelope.Funder.OrganizationName;
                        int envelopeId = envelope.Id;

                        if (yearlyBreakup != null)
                        {
                            yearlyBreakup = (from yb in yearlyBreakup
                                             orderby yb.Year.FinancialYear ascending
                                             select yb);
                        }

                        IQueryable<EFEnvelopeYearlyBreakup> yearBreakup = null;
                        foreach (var type in envelopeTypes)
                        {
                            EnvelopeBreakupView breakupView = new EnvelopeBreakupView();
                            breakupView.EnvelopeType = type.TypeName;
                            breakupView.EnvelopeTypeId = type.Id;

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
                                    yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                                    {
                                        Amount = isBreakupExists.Amount,
                                        Year = year
                                    });
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
                            breakupView.YearlyBreakup = yearlyBreakupList;
                            envelopeView.EnvelopeBreakupsByType.Add(breakupView);
                            envelopeViewList.Add(envelopeView);
                        }
                    }
                    envelopeReport.Envelope = envelopeViewList;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<EnvelopeReport>.Run(() => envelopeReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate == 0) ? 1 : exchangeRate;
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
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
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectSectors> projectSectors = null;

                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartingFinancialYear.FinancialYear >= model.StartingYear
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


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains(p.SectorId), new string[] { "Sector" });
                    }
                    else
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => p.ProjectId != 0, new string[] { "Sector" });
                    }

                    projectSectors = from pSector in projectSectors
                                     orderby pSector.Sector.SectorName
                                     select pSector;

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
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

                    foreach (var sectorByProject in sectorsByProjects)
                    {
                        projectsBySector = new ProjectsBySector();
                        projectsBySector.SectorName = sectorByProject.Sector;
                        int currentSectorId = sectorByProject.SectorId;

                        projectsListForSector = new List<ProjectViewForSector>();
                        var projectIds = (from p in sectorByProject.Projects
                                          select p.ProjectId);

                        var sectorProjects = (from project in projectsList
                                              where projectIds.Contains(project.Id)
                                              select project).ToList<ProjectProfileView>();

                        decimal totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, sectorPercentage = 0;
                        foreach (var project in sectorProjects)
                        {
                            if (project.Sectors != null)
                            {
                                sectorPercentage = (from s in project.Sectors
                                                    where s.SectorId == currentSectorId
                                                    select s.FundsPercentage).FirstOrDefault();

                                if (project.Funders.Count() > 0)
                                {
                                    project.ProjectPercentValue = Math.Round(((project.ProjectValue / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    totalFundingPercentage += project.ProjectPercentValue;
                                }
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            if (project.Sectors != null)
                            {
                                sectorPercentage = (from s in project.Sectors
                                                    where s.SectorId == currentSectorId
                                                    select s.FundsPercentage).FirstOrDefault();
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }

                                    decimal actualDisbursements = (from d in project.Disbursements
                                                                   where d.DisbursementType == DisbursementTypes.Actual
                                                                   select (d.Amount * d.ExchangeRate)).FirstOrDefault();
                                    decimal plannedDisbursements = (from d in project.Disbursements
                                                                    where d.DisbursementType == DisbursementTypes.Planned
                                                                    select (d.Amount * d.ExchangeRate)).FirstOrDefault();
                                    totalDisbursements = (actualDisbursements + plannedDisbursements);

                                    UtilityHelper helper = new UtilityHelper();
                                    project.ActualDisbursements = Math.Round(((actualDisbursements / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    project.PlannedDisbursements = Math.Round(((plannedDisbursements / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    if (project.PlannedDisbursements < 0)
                                    {
                                        project.PlannedDisbursements = 0;
                                    }
                                    totalDisbursementsPercentage += project.ActualDisbursements;
                                }
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                            projectsListForSector.Add(new ProjectViewForSector()
                            {
                                Title = project.Title,
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate)),
                                ProjectPercentValue = project.ProjectPercentValue,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        projectsBySector.TotalFunding = totalFundingPercentage;
                        projectsBySector.TotalDisbursements = totalDisbursementsPercentage;
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

                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndingFinancialYear.FinancialYear >= model.StartingYear || p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                 select project;
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
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        var projectIds = unitWork.ProjectSectorsRepository.GetProjection(p => model.SectorIds.Contains(p.SectorId), p => p.ProjectId);
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
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
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
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
                                            where p.StartingFinancialYear.FinancialYear >= yr || p.EndingFinancialYear.FinancialYear >= yr
                                            select p.Id).ToList<int>()
                            });
                        }
                    }

                    foreach (var yearProject in yearProjects)
                    {
                        projectsByYear = new ProjectsByYear();
                        int currentYearId = yearProject.Year;

                        projectsViewForYear = new List<ProjectViewForYear>();
                        var projectIds = yearProject.Projects;
                        var yearlyProjectsProfile = (from project in projectsList
                                                     where projectIds.Contains(project.Id)
                                                     select project).ToList<ProjectProfileView>();

                        decimal totalFunding = 0, totalDisbursements = 0, totalProjectValue = 0, totalPlannedDisbursements = 0;
                        foreach (var project in yearlyProjectsProfile)
                        {
                            totalProjectValue += (project.ProjectValue * (exchangeRate / project.ExchangeRate));
                            //Disbursements
                            if (project.Disbursements.Count() > 0)
                            {
                                var disbursements = (from d in project.Disbursements
                                                     select d).ToList();

                                decimal projectDisbursements = Math.Round((from d in disbursements
                                                                           where d.DisbursementType == DisbursementTypes.Actual && d.Year == currentYearId
                                                                           select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                decimal plannedDisbursements = Math.Round((from d in disbursements
                                                                           where d.DisbursementType == DisbursementTypes.Planned && d.Year == currentYearId
                                                                           select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
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
                            }

                            projectsViewForYear.Add(new ProjectViewForYear()
                            {
                                Title = project.Title,
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = (project.ProjectValue * (exchangeRate / project.ExchangeRate)),
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        if (projectsByYear != null)
                        {
                            projectsByYear.Year = currentYearId;
                            projectsByYear.TotalFunding = totalFunding;
                            projectsByYear.TotalDisbursements = totalDisbursements;
                            projectsByYear.TotalProjectValue = totalProjectValue;
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

    }
}
