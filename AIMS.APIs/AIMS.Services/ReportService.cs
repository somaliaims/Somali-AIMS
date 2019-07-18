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
        /// Gets projects budget report
        /// </summary>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<ProjectsBudgetReport> GetProjectsBudgetReport(string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Internal function for extracting rate of default currency
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="ratesList"></param>
        /// <returns></returns>
        decimal GetExchangeRateForCurrency(string currency, List<CurrencyWithRates> ratesList);
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


        public async Task<ProjectProfileReportByLocation> GetProjectsByLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
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
                            new string[] { "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 1970 && model.EndingYear >= 1970)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndDate.Year >= model.StartingYear && p.EndDate.Year <= model.EndingYear)),
                            new string[] { "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartDate.Year >= model.StartingYear
                                                 && project.EndDate.Year <= model.EndingYear
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
                            , new string[] { "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndDate.Year >= year && p.EndDate.Month >= month),
                            new string[] { "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
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
                        decimal totalFunding = 0, totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, locationPercentage = 0;
                        totalFunding = Math.Round(totalFunding, MidpointRounding.AwayFromZero);
                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.Id == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();

                                projectTotalPercentage += Convert.ToInt32(locationPercentage);
                                if (project.Funders.Count() > 0)
                                {
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Funders = (from f in project.Funders
                                                                 where f.Dated.Year >= model.StartingYear
                                                                 select f).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Funders = (from f in project.Funders
                                                                 where f.Dated.Year <= model.EndingYear
                                                                 select f).ToList();
                                    }
                                    var fundingTotal = project.Funders.Select(f => (f.Amount * (exchangeRate / f.ExchangeRate))).Sum();
                                    fundingTotal = Math.Round(fundingTotal, MidpointRounding.AwayFromZero);
                                    project.ProjectCost = Math.Round(((fundingTotal / 100) * locationPercentage), MidpointRounding.AwayFromZero);
                                    totalFundingPercentage += project.ProjectCost;
                                }
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.Id == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Dated.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Dated.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }

                                    decimal projectDisbursements = project.Disbursements.Select(d => (d.Amount * (exchangeRate / d.ExchangeRate))).Sum();
                                    totalDisbursements += projectDisbursements;

                                    UtilityHelper helper = new UtilityHelper();
                                    var endDate = Convert.ToDateTime(project.EndDate);
                                    var startDate = DateTime.Now;
                                    int months = helper.GetMonthDifference(startDate, endDate);

                                    project.ActualDisbursements = Math.Round(((projectDisbursements / 100) * locationPercentage), MidpointRounding.AwayFromZero);
                                    if (months > 0)
                                    {
                                        project.PlannedDisbursements = Math.Round(((project.ProjectCost - project.ActualDisbursements) / months), MidpointRounding.AwayFromZero);
                                        if (project.PlannedDisbursements < 0)
                                        {
                                            project.PlannedDisbursements = 0;
                                        }
                                    }
                                    totalDisbursementsPercentage += project.ActualDisbursements;
                                }
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            projectsListForLocation.Add(new ProjectViewForLocation()
                            {
                                Title = project.Title,
                                StartDate = project.StartDate,
                                EndDate = project.EndDate,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectCost = project.ProjectCost,
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

        public async Task<ProjectsBudgetReport> GetProjectsBudgetReport(string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectsBudgetReport budgetReport = new ProjectsBudgetReport();
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
                    List<ProjectBudgetView> projectBudgetsList = new List<ProjectBudgetView>();

                    projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndDate.Year >= currentYear,
                            new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Funders.FundingType" });


                    UtilityHelper utilityHelper = new UtilityHelper();
                    foreach (var project in projectProfileList)
                    {
                        ProjectBudgetView projectBudget = new ProjectBudgetView();
                        int upperYearLimit = project.EndDate.Year;
                        int yearsLeft = upperYearLimit - currentYear;
                        int monthsCurrentYear = 0;
                        int projectStartYear = project.StartDate.Year;
                        int projectEndYear = project.EndDate.Year;

                        if (projectEndYear > currentYear && projectStartYear < currentYear)
                        {
                            monthsCurrentYear = ReportConstants.YEAR_MONTHS;
                        }
                        else if (projectStartYear < currentYear && projectEndYear == currentYear)
                        {
                            monthsCurrentYear = (ReportConstants.YEAR_MONTHS - project.EndDate.Month);
                        }
                        else if (projectStartYear == currentYear && projectEndYear > currentYear)
                        {
                            monthsCurrentYear = (ReportConstants.YEAR_MONTHS - project.StartDate.Month);
                        }
                        else if (projectStartYear == currentYear && projectEndYear == currentYear)
                        {
                            monthsCurrentYear = utilityHelper.GetMonthDifference(project.StartDate, project.EndDate);
                        }
                        projectBudget.Id = project.Id;
                        projectBudget.Title = project.Title;
                        projectBudget.StartDate = project.StartDate;
                        projectBudget.EndDate = project.EndDate;
                        projectBudget.StartDateString = project.StartDate.ToString("yyyy MMMM");
                        projectBudget.EndDateString = project.EndDate.ToString("yyyy MMMM");
                        projectBudget.TotalMonths = utilityHelper.GetMonthDifference(project.StartDate, project.EndDate);
                        projectBudget.MonthsLeft = (project.EndDate <= DateTime.Now) ? 0 : (projectBudget.TotalMonths - (utilityHelper.GetMonthDifference(project.StartDate, DateTime.Now)));
                        
                        List<ProjectFunding> funderList = new List<ProjectFunding>();
                        List<ProjectDisbursements> disbursementsList = new List<ProjectDisbursements>();
                        List<BudgetFundShare> fundShares = new List<BudgetFundShare>();
                        decimal projectCost = 0;
                        if (project.Funders.Count > 0)
                        {
                            foreach(var funder in project.Funders)
                            {
                                decimal calculatedExRate = Math.Round((exchangeRate / funder.ExchangeRate), 3);
                                funderList.Add(new ProjectFunding()
                                {
                                    Funder = funder.Funder.OrganizationName,
                                    Amount = funder.Amount,
                                    AmountInDefault = Math.Round((funder.Amount * calculatedExRate), MidpointRounding.AwayFromZero),
                                    Currency = funder.Currency,
                                    ExchangeRateToDefault = calculatedExRate,
                                    FundType = funder.FundingType.FundingType,
                                });
                                projectCost += ((funder.Amount) * calculatedExRate);
                            }
                            fundShares = (from f in project.Funders
                                                        group (f.Amount * (exchangeRate / f.ExchangeRate)) by f.FundingType.FundingType into g
                                                        select new BudgetFundShare()
                                                        {
                                                            FundingType = g.Key,
                                                            Amount = Math.Round(g.Sum(), MidpointRounding.AwayFromZero),
                                                            Percentage = Math.Round((((g.Sum()) / projectCost) * 100), 2)
                                                        }).ToList();
                        }
                        projectBudget.ProjectValue = Math.Round(projectCost, MidpointRounding.AwayFromZero);

                        decimal actualDisbursements = 0;
                        if (project.Disbursements.Count > 0)
                        {
                            foreach(var disbursement in project.Disbursements)
                            {
                                decimal calculatedExRate = Math.Round((exchangeRate / disbursement.ExchangeRate), 3);
                                disbursementsList.Add(new ProjectDisbursements()
                                {
                                    Amount = disbursement.Amount,
                                    AmountInDefault = Math.Round((disbursement.Amount * calculatedExRate), MidpointRounding.AwayFromZero),
                                    Currency = disbursement.Currency,
                                    Dated = disbursement.Dated.ToShortDateString(),
                                    ExchangeRateToDefault = calculatedExRate
                                });
                                actualDisbursements = (disbursement.Amount * calculatedExRate);
                            }
                        }
                        actualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero);

                        List<ProjectYearlyDisbursements> yearlyDisbursements = new List<ProjectYearlyDisbursements>();
                        int activeMonths = 0;
                        for (int year = (currentYear - 1); year <= upperYearLimit; year++)
                        {
                            activeMonths = 0;
                            decimal yearDisbursements = (from d in disbursementsList
                                                         select d.AmountInDefault).Sum();
                            decimal expectedDisbursements = yearsLeft > 0 ? (Math.Round((projectCost - actualDisbursements) / yearsLeft)) : actualDisbursements;
                            if (year > projectStartYear && year < projectEndYear)
                            {
                                activeMonths = ReportConstants.YEAR_MONTHS;
                            }
                            else if (year < projectStartYear)
                            {
                                activeMonths = 0;
                            }
                            else if (year > projectStartYear && year == projectEndYear)
                            {
                                activeMonths = projectBudget.EndDate.Month;
                            }
                            else if(year == projectStartYear && year < projectEndYear)
                            {
                                activeMonths = (ReportConstants.YEAR_MONTHS - projectBudget.StartDate.Month);
                            }
                            yearlyDisbursements.Add(new ProjectYearlyDisbursements()
                            {
                                Year = year,
                                Disbursements = yearDisbursements,
                                ExpectedDisbursements = expectedDisbursements,
                                ActiveMonths = activeMonths
                            });
                        }
                        projectBudget.MoneyPreviousTwoYears = (from d in yearlyDisbursements
                                                               where (d.Year == (currentYear - 2) ||
                                                               d.Year == (currentYear - 1))
                                                               select d.Disbursements).Sum();
                        projectBudget.MoneyLeftForYears = (from d in yearlyDisbursements
                                                           where d.Year >= currentYear
                                                           select d.Disbursements).Sum();
                        var yearlyDisbursement = (from d in yearlyDisbursements
                                                  where d.Year == currentYear
                                                  select d).FirstOrDefault();
                        if (yearlyDisbursement != null)
                        {
                            projectBudget.ExpectedMinusActual = (yearlyDisbursement.ExpectedDisbursements - yearlyDisbursement.Disbursements);
                            projectBudget.ExpectedMinusActual = (projectBudget.ExpectedMinusActual > 0) ? projectBudget.ExpectedMinusActual : 0;
                        }

                        projectBudget.Funding = funderList;
                        projectBudget.Disbursements = disbursementsList;
                        projectBudget.YearlyDisbursements = yearlyDisbursements;
                        projectBudget.FundShares = fundShares;
                        projectBudgetsList.Add(projectBudget);
                    }
                    budgetReport.Projects = projectBudgetsList;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<ProjectsBudgetReport>.Run(() => budgetReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
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
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndDate.Year >= model.StartingYear && p.EndDate.Year <= model.EndingYear)),
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartDate.Year >= model.StartingYear
                                                 && project.EndDate.Year <= model.EndingYear
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
                            , new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndDate.Year >= year && p.EndDate.Month >= month),
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
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
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Funders = (from f in project.Funders
                                                           where f.Dated.Year >= model.StartingYear
                                                           select f).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Funders = (from f in project.Funders
                                                           where f.Dated.Year <= model.EndingYear
                                                           select f).ToList();
                                    }
                                    var fundingTotal = Math.Round(project.Funders.Select(f => (f.Amount * (exchangeRate / f.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                    fundingTotal = Math.Round(fundingTotal, MidpointRounding.AwayFromZero);
                                    project.ProjectCost = Math.Round(((fundingTotal / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    totalFundingPercentage += project.ProjectCost;
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
                                                           where d.Dated.Year >= model.StartingYear
                                                           select d).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                           where d.Dated.Year <= model.EndingYear
                                                           select d).ToList();
                                    }

                                    decimal projectDisbursements = Math.Round(project.Disbursements.Select(d => (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                    totalDisbursements += projectDisbursements;
                                    totalDisbursements = Math.Round(totalDisbursements, MidpointRounding.AwayFromZero);
                                    UtilityHelper helper = new UtilityHelper();
                                    var endDate = Convert.ToDateTime(project.EndDate);
                                    var startDate = DateTime.Now;
                                    int months = helper.GetMonthDifference(startDate, endDate);

                                    project.ActualDisbursements = Math.Round(((projectDisbursements / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    if (months > 0)
                                    {
                                        project.PlannedDisbursements = Math.Round(((project.ProjectCost - project.ActualDisbursements) / months), MidpointRounding.AwayFromZero);
                                        if (project.PlannedDisbursements < 0)
                                        {
                                            project.PlannedDisbursements = 0;
                                        }
                                    }
                                    totalDisbursementsPercentage += project.ActualDisbursements; 
                                }
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            projectsListForSector.Add(new ProjectViewForSector()
                            {
                                Title = project.Title,
                                StartDate = project.StartDate,
                                EndDate = project.EndDate,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectCost = project.ProjectCost,
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
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndDate.Year >= model.StartingYear && p.EndDate.Year <= model.EndingYear)),
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartDate.Year >= model.StartingYear
                                                 && project.EndDate.Year <= model.EndingYear
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
                            , new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndDate.Year >= year && p.EndDate.Month >= month),
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        var projectIds = unitWork.ProjectSectorsRepository.GetProjection(p => model.SectorIds.Contains(p.SectorId), p => p.ProjectId);
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
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
                                            where p.StartDate.Year == yr
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

                        decimal totalFunding = 0, totalDisbursements = 0;
                        foreach (var project in yearlyProjectsProfile)
                        {
                            if (project.Funders.Count() > 0)
                            {
                                if (model.StartingYear >= 2000)
                                {
                                    project.Funders = (from f in project.Funders
                                                       where f.Dated.Year >= model.StartingYear
                                                       select f).ToList();
                                }

                                if (model.EndingYear >= 2000)
                                {
                                    project.Funders = (from f in project.Funders
                                                       where f.Dated.Year <= model.EndingYear
                                                       select f).ToList();
                                }
                                var fundingTotal = Math.Round(project.Funders.Select(f => (f.Amount * (exchangeRate / f.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                fundingTotal = Math.Round(fundingTotal, MidpointRounding.AwayFromZero);
                                project.ProjectCost = fundingTotal;
                                totalFunding += fundingTotal;
                            }

                            //Disbursements
                            if (project.Disbursements.Count() > 0)
                            {
                                if (model.StartingYear >= 2000)
                                {
                                    project.Disbursements = (from d in project.Disbursements
                                                       where d.Dated.Year >= model.StartingYear
                                                       select d).ToList();
                                }

                                if (model.EndingYear >= 2000)
                                {
                                    project.Disbursements = (from d in project.Disbursements
                                                       where d.Dated.Year <= model.EndingYear
                                                       select d).ToList();
                                }
                                decimal projectDisbursements = Math.Round(project.Disbursements.Select(d => (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                totalDisbursements += projectDisbursements;
                                totalDisbursements = Math.Round(totalDisbursements, MidpointRounding.AwayFromZero);
                                UtilityHelper helper = new UtilityHelper();
                                var endDate = Convert.ToDateTime(project.EndDate);
                                var startDate = DateTime.Now;
                                int months = helper.GetMonthDifference(startDate, endDate);

                                project.ActualDisbursements = projectDisbursements;
                                if (months > 0)
                                {
                                    project.PlannedDisbursements = Math.Round(((project.ProjectCost - project.ActualDisbursements) / months), MidpointRounding.AwayFromZero);
                                    if (project.PlannedDisbursements < 0)
                                    {
                                        project.PlannedDisbursements = 0;
                                    }
                                }
                            }

                            projectsViewForYear.Add(new ProjectViewForYear()
                            {
                                Title = project.Title,
                                StartDate = project.StartDate,
                                EndDate = project.EndDate,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectCost = project.ProjectCost,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        if (projectsByYear != null)
                        {
                            projectsByYear.Year = currentYearId;
                            projectsByYear.TotalFunding = totalFunding;
                            projectsByYear.TotalDisbursements = totalDisbursements;
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
