using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Gets all projects
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectView> GetAll(decimal exchangeRate);

        /// <summary>
        /// Gets list of project titles
        /// </summary>
        /// <returns></returns>
        ICollection<ProjectTitle> GetProjectTitles();

        /// <summary>
        /// Gets all with abstract level details
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectAbstractView>> GetAllDetailAsync();

        /// <summary>
        /// Gets project details for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProjectModelView Get(int id);

        /// <summary>
        /// Gets project report
        /// </summary>
        /// <returns></returns>
        ProjectReport GetProjectsReport();

        /// <summary>
        /// Get project full profile report for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProjectProfileReport> GetProjectProfileReportAsync(int id);

        /// <summary>
        /// Gets latest list of projects
        /// </summary>
        /// <returns></returns>
        IEnumerable<LatestProjectView> GetLatest();

        /// <summary>
        /// Get project reports filtered through sector id
        /// </summary>
        /// <param name="sectorId"></param>
        /// <returns></returns>
        Task<FilteredProjectProfileReport> GetProjectsReportForSectorAsync(int sectorId);

        /// <summary>
        /// Gets report for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProjectReport GetProjectReport(int id);

        /// <summary>
        /// Gets project title
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProjectInfo GetTitle(int id);

        /// <summary>
        /// Gets matching project titles list
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<ProjectView> GetMatching(string criteria);

        /// <summary>
        /// Search matching projects for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectProfileView>> SearchProjectsByCriteria(SearchProjectModel model, decimal exchangeRate);

        /// <summary>
        /// Gets lighter version of projects for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectView>> SearchProjectsViewByCriteria(SearchProjectModel model, decimal exchangeRate);

        /// <summary>
        /// Gets project profiles for the provided project ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectProfileView>> GetProjectsByIdsAsync(List<int> ids);

        /// <summary>
        /// Gets all projects async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        Task<ActionResponse> AddAsync(ProjectModel project, int userId);

        /// <summary>
        /// Updates a project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<ActionResponse> UpdateAsync(int id, ProjectModel model);

        /// <summary>
        /// Gets locations for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<LocationView> GetProjectLocations(int id);

        /// <summary>
        /// Gets sectors for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectSectorView> GetProjectSectors(int id);

        /// <summary>
        /// Gets funds for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectFundsView> GetProjectFunds(int id);

        /// <summary>
        /// Adds location to a project
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<ActionResponse> AddProjectLocation(ProjectLocationModel model);

        /// <summary>
        /// Adds sector to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddProjectSector(ProjectSectorModel model);

        /// <summary>
        /// Adds funder to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectFunder(ProjectFunderModel model, int userOrganizationId);

        /// <summary>
        /// Adds funder to a project with the logic from source
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userOrganizationId"></param>
        /// <returns></returns>
        ActionResponse AddProjectFunderFromSource(ProjectFunderSourceModel model, int userOrganizationId);

        /// <summary>
        /// Adds implementer to a project with the logic from souce
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userOrganizationId"></param>
        /// <returns></returns>
        ActionResponse AddProjectImplementerFromSource(ProjectImplementerSourceModel model, int userOrganizationId);

        /// <summary>
        /// Adds implementer to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectImplementer(ProjectImplementerModel model);

        /// <summary>
        /// Adds disbursement to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddProjectDisbursement(ProjectDisbursementModel model);

        /// <summary>
        /// Adds document to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectDocument(ProjectDocumentModel model);

        /// <summary>
        /// Adds custom field to project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddUpdateProjectMarker(ProjectMarkerModel model);

        /// <summary>
        /// Gets funders for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectFunderView> GetProjectFunders(int id);

        /// <summary>
        /// Gets implementers for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectImplementerView> GetProjectImplementers(int id);

        /// <summary>
        /// Gets list of projects for the provided funder
        /// </summary>
        /// <param name="funderId"></param>
        /// <returns></returns>
        IEnumerable<ProjectView> GetOrganizationProjects(int funderId);

        /// <summary>
        /// Gets list of projects for the provided location
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        IEnumerable<ProjectView> GetLocationProjects(int locationId);

        /// <summary>
        /// Gets list of projects for the provided sector
        /// </summary>
        /// <param name="sectorId"></param>
        /// <returns></returns>
        IEnumerable<ProjectView> GetSectorProjects(int sectorId);

        /// <summary>
        /// Gets list of projects for the provided field id
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        IEnumerable<ProjectView> GetMarkerProjects(int fieldId);

        /// <summary>
        /// Gets project disbursements
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectDisbursementView>> GetProjectDisbursementsAsync(int id);

        /// <summary>
        /// Adjusts project disbursements after changing the settings for FYs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectDisbursementView>> AdjustProjectDisbursementsAsync(int id);

        /// <summary>
        /// Creates project disbursements after project creation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectDisbursementView>> CreateProjectDisbursementsAsync(int id);

        /// <summary>
        /// Gets project documents
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectDocumentView> GetProjectDocuments(int id);

        /// <summary>
        /// Gets users projects
        /// </summary>
        /// <param name="funderId"></param>
        /// <returns></returns>
        IEnumerable<UserProjectsView> GetUserProjects(int userId, int funderId);

        /// <summary>
        /// Imports projects from Excel extracted template
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        Task<ActionResponse> ImportProjects(List<NewImportedAidData> projects);

        /// <summary>
        /// Deletes project location
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteProjectLocationAsync(int projectId, int locationId);

        /// <summary>
        /// Deletes project sector
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteProjectSectorAsync(int projectId, int locationId);

        /// <summary>
        /// Deletes project funder
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="funderId"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectFunder(int projectId, int funderId);

        /// <summary>
        /// Deletes project implementer
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="implementerId"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectImplementer(int projectId, int implementerId);

        /// <summary>
        /// Deletes project custom field id
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="customFieldId"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectMarker(int projectId, int customFieldId);

        /// <summary>
        /// Deletes the disbursement with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectDisbursement(int id);

        /// <summary>
        /// Deletes a document for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectDocument(int id);

        /// <summary>
        /// Merges the provided projects and their relevant data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> MergeProjectsAsync(MergeProjectsModel model, int userId);

        /// <summary>
        /// Gets count for active projects
        /// </summary>
        /// <returns></returns>
        int GetActiveProjectsCount();

        /// <summary>
        /// Gets current year disbursements
        /// </summary>
        /// <returns></returns>
        CurrentYearDisbursementView GetCurrentYearDisbursements();

        /// <summary>
        /// Rectify financial years for projects, temporary Api
        /// </summary>
        /// <returns></returns>
        ActionResponse UpdateFinancialYearsForProjects();

        /// <summary>
        /// Adjust planned disbursements for active projects
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> AdjustDisbursementsForProjectsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="ratesList"></param>
        /// <returns></returns>
        decimal GetExchangeRateForCurrency(string currency, List<CurrencyWithRates> ratesList);
    }

    public class ProjectService : IProjectService
    {
        AIMSDbContext context;
        IMapper mapper;
        readonly string UNATTRIBUTED = "UNATTRIBUTED";

        public ProjectService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectView> GetAll(decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                var financialYearSettings = unitWork.FinancialYearSettingsRepository.GetOne(fy => fy.Id != 0);
                int fyDay = 1, fyMonth = 1;
                if (financialYearSettings != null)
                {
                    fyDay = financialYearSettings.Day;
                    fyMonth = financialYearSettings.Month;
                }

                int currentFinancialYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                int currentDay = DateTime.Now.Day;

                if (fyDay != 1 && fyMonth != 1)
                {
                    if (currentMonth < fyMonth)
                    {
                        --currentFinancialYear;
                    }
                    else if (currentMonth == fyMonth && currentDay < fyDay)
                    {
                        --currentFinancialYear;
                    }
                }

                var projects = unitWork.ProjectRepository.GetWithInclude(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                projects = (from p in projects
                            orderby p.DateUpdated descending
                            select p);

                var projectDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.Year.FinancialYear == currentFinancialYear && d.DisbursementType == DisbursementTypes.Planned, new string[] { "Year" });
                List<ProjectView> projectsList = new List<ProjectView>();
                string yearLabel = "";
                if (projects.Any())
                {
                    yearLabel = (from d in projectDisbursements
                                 where d.Year.FinancialYear == currentFinancialYear
                                 select d.Year.Label).FirstOrDefault();
                }
                foreach(var project in projects)
                {
                    decimal currentFYPlannedDisbursements = (from d in projectDisbursements
                                                             where d.ProjectId == project.Id && d.Year.FinancialYear == currentFinancialYear
                                                             select ((exchangeRate / d.ExchangeRate) * d.Amount)).FirstOrDefault();
                    project.ExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                    projectsList.Add(new ProjectView()
                    {
                        Id = project.Id,
                        Title = project.Title,
                        Description = project.Description,
                        ProjectValueInDefaultCurrency = Math.Round(((exchangeRate / project.ExchangeRate) * project.ProjectValue), MidpointRounding.AwayFromZero),
                        CurrentYearPlannedDisbursements = currentFYPlannedDisbursements,
                        CurrentYearLabel = yearLabel
                    });
                }
                return projectsList;
            }
        }

        public async Task<IEnumerable<ProjectView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                projects = (from p in projects
                            orderby p.DateUpdated descending
                            select p);
                return await Task<IEnumerable<ProjectView>>.Run(() => mapper.Map<List<ProjectView>>(projects)).ConfigureAwait(false);
            }
        }

        public ICollection<ProjectTitle> GetProjectTitles()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectTitle> projectTitles = new List<ProjectTitle>();
                var projects = unitWork.ProjectRepository.GetProjection(p => p.Id != 0, p => new { p.Id, p.Title });
                if (projects.Any())
                {
                    foreach (var project in projects)
                    {
                        projectTitles.Add(new ProjectTitle()
                        {
                            Id = project.Id,
                            Title = project.Title
                        });
                    }
                }
                return projectTitles;
            }
        }

        public IEnumerable<LatestProjectView> GetLatest()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<LatestProjectView> projects = new List<LatestProjectView>();
                var latestProjects = unitWork.ProjectRepository.GetWithIncludeOrderByDescending(p => p.Id != 0, p => p.DateUpdated, 5, new string[] { "Funders", "Funders.Funder", "StartingFinancialYear", "EndingFinancialYear" });
                foreach (var project in latestProjects)
                {
                    projects.Add(new LatestProjectView()
                    {
                        Id = project.Id,
                        Title = project.Title,
                        StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString(),
                        EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString(),
                        ProjectCost = project.ProjectValue
                    });
                }
                return projects;
            }
        }

        public CurrentYearDisbursementView GetCurrentYearDisbursements()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                CurrentYearDisbursementView disbursementView = new CurrentYearDisbursementView();
                Decimal disbursementValue = 0;
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                int currentDay = DateTime.Now.Day;
                var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(s => s.Id != 0);
                int settingsMonth = 1, settingsDay = 1;
                if (fySettings != null)
                {
                    settingsMonth = fySettings.Month;
                    settingsDay = fySettings.Day;

                    if (currentMonth < settingsMonth)
                    {
                        --currentYear;
                    }
                    else if (currentMonth == settingsMonth && currentDay < settingsDay)
                    {
                        --currentYear;
                    }
                }
                var financialYearExists = unitWork.FinancialYearRepository.GetOne(y => y.FinancialYear == currentYear);
                if (financialYearExists == null)
                {
                    string label = (fySettings.Month == 1 && fySettings.Day == 1) ? "FY " + currentYear : "FY " + (currentYear) + "/" + (currentYear + 1);
                    unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                    {
                        FinancialYear = currentYear,
                        Label = label
                    });
                    unitWork.Save();
                }
                else
                {
                    var disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.Year.FinancialYear == currentYear && d.DisbursementType == DisbursementTypes.Actual && d.Amount > 0, new string[] { "Year" });
                    if (disbursements.Any())
                    {
                        disbursementValue = (from d in disbursements
                                             select (d.Amount * (1 / d.ExchangeRate))).Sum();
                    }
                }
                disbursementView.Year = currentYear;
                disbursementView.FinancialYear = financialYearExists.Label;
                disbursementView.Disbursements = disbursementValue;
                return disbursementView;
            }
        }

        public async Task<IEnumerable<ProjectAbstractView>> GetAllDetailAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectAbstractView> projectsList = new List<ProjectAbstractView>();
                var projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                foreach (var project in projects)
                {
                    IEnumerable<string> funderNames = (from f in project.Funders
                                                       select f.Funder.OrganizationName);
                    IEnumerable<string> implementerNames = (from i in project.Implementers
                                                            select i.Implementer.OrganizationName);
                    IEnumerable<string> organizations = funderNames.Union(implementerNames);

                    List<OrganizationAbstractView> organizationsList = new List<OrganizationAbstractView>();
                    foreach (string org in organizations)
                    {
                        organizationsList.Add(new OrganizationAbstractView()
                        {
                            Name = org
                        });
                    }
                    projectsList.Add(new ProjectAbstractView()
                    {
                        Id = project.Id,
                        Title = project.Title,
                        Description = project.Description,
                        ProjectValue = project.ProjectValue,
                        ProjectCurrency = project.ProjectCurrency,
                        StartDate = project.StartDate.ToShortDateString(),
                        EndDate = project.EndDate.ToShortDateString(),
                        DateUpdated = project.DateUpdated.ToShortDateString(),
                        StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString(),
                        EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString(),
                        Organizations = organizationsList,
                        Locations = mapper.Map<List<LocationAbstractView>>(project.Locations),
                        Sectors = mapper.Map<List<SectorAbstractView>>(project.Sectors)
                    });
                }
                return await Task<IEnumerable<ProjectAbstractView>>.Run(() => projectsList).ConfigureAwait(false);
            }
        }

        public IEnumerable<ProjectView> GetOrganizationProjects(int organizatioinId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var funderProjects = unitWork.ProjectFundersRepository.GetManyQueryable(p => p.FunderId == organizatioinId);
                var implementerProjects = unitWork.ProjectImplementersRepository.GetManyQueryable(p => p.ImplementerId == organizatioinId);

                var projectForFunders = (from f in funderProjects
                                         select f.ProjectId).Distinct().ToList<int>();
                var projectForImplementers = (from i in implementerProjects
                                              select i.ProjectId).Distinct().ToList<int>();

                List<int> projectIds = projectForFunders.Union(projectForImplementers).ToList<int>();
                var projects = unitWork.ProjectRepository.GetManyQueryable(p => projectIds.Contains(p.Id));
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public IEnumerable<ProjectView> GetLocationProjects(int locationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var locationProjects = unitWork.ProjectLocationsRepository.GetManyQueryable(p => p.LocationId == locationId);

                var projectIds = (from l in locationProjects
                                  select l.ProjectId).Distinct().ToList<int>();

                var projects = unitWork.ProjectRepository.GetManyQueryable(p => projectIds.Contains(p.Id));
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public IEnumerable<ProjectView> GetSectorProjects(int sectorId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorProjects = unitWork.ProjectSectorsRepository.GetManyQueryable(p => p.SectorId == sectorId);

                var projectIds = (from s in sectorProjects
                                  select s.ProjectId).Distinct().ToList<int>();

                var projects = unitWork.ProjectRepository.GetManyQueryable(p => projectIds.Contains(p.Id));
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public IEnumerable<ProjectView> GetMarkerProjects(int fieldId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var fieldProjects = unitWork.ProjectMarkersRepository.GetManyQueryable(p => p.MarkerId == fieldId);

                var projectIds = (from f in fieldProjects
                                  select f.ProjectId).Distinct().ToList<int>();

                var projects = unitWork.ProjectRepository.GetManyQueryable(p => projectIds.Contains(p.Id));
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public ProjectModelView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var project = unitWork.ProjectRepository.GetWithInclude(p => p.Id == id, new string[] { "StartingFinancialYear", "EndingFinancialYear" }).FirstOrDefault();
                return mapper.Map<ProjectModelView>(project);
            }
        }

        public int GetActiveProjectsCount()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return unitWork.ProjectRepository.GetProjection(p => p.Id != 0, p => p.Id).Count();
            }
        }

        public ProjectInfo GetTitle(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectInfo info = new ProjectInfo();
                var project = unitWork.ProjectRepository.GetByID(id);
                if (project != null)
                {
                    info.ProjectTitle = project.Title;
                }
                return info;
            }
        }

        public ProjectReport GetProjectsReport()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectReport report = new ProjectReport();
                var projects = unitWork.ProjectRepository.GetAll();
                var projectsList = mapper.Map<List<ProjectView>>(projects);
                report.ReportSettings = new Report()
                {
                    Title = ReportConstants.PROJECTS_LIST_TITLE,
                    Dated = DateTime.Now.ToLongDateString(),
                    Footer = ReportConstants.PROJECTS_LIST_FOOTER
                };
                report.Projects = projectsList;
                return report;
            }
        }

        public async Task<ProjectProfileReport> GetProjectProfileReportAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id.Equals(id), new string[] { "StartingFinancialYear", "EndingFinancialYear", "FundingType", "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers.Marker" });
                ProjectProfileView profileView = new ProjectProfileView();

                if (projectProfileList != null)
                {
                    IOrderedEnumerable<EFProjectDisbursements> projectDisbursements = null;
                    foreach (var project in projectProfileList)
                    {
                        List<ProjectDisbursementView> disbursementsList = new List<ProjectDisbursementView>();
                        if (project.Disbursements.Any())
                        {
                            projectDisbursements = (from d in project.Disbursements
                                                    orderby d.Year.FinancialYear, d.DisbursementType ascending
                                                    select d);
                        }
                        if (project.Locations.Count > 0)
                        {
                            project.Locations = (from l in project.Locations
                                                 orderby l.Location.Location
                                                 select l).ToList();
                        }
                        if (project.Sectors.Count > 0)
                        {
                            project.Sectors = (from s in project.Sectors
                                               orderby s.Sector.SectorName
                                               select s).ToList();
                        }

                        string startDate = (project.StartDate != null) ? Convert.ToDateTime(project.StartDate).ToShortDateString() : null;
                        string endDate = (project.EndDate != null) ? Convert.ToDateTime(project.EndDate).ToShortDateString() : null;
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.FundingTypeId = project.FundingTypeId;
                        profileView.FundingType = project.FundingType.FundingType;
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ExchangeRate = project.ExchangeRate;
                        profileView.Description = project.Description;
                        profileView.StartDate = startDate;
                        profileView.EndDate = endDate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(projectDisbursements); profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);
                        profileView.Markers = mapper.Map<List<ProjectMarkersView>>(project.Markers);
                    }
                }
                ProjectProfileReport projectProfileReport = new ProjectProfileReport()
                {
                    ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_PROFILE_TITLE,
                        SubTitle = ReportConstants.PROJECTS_PROFILE_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_PROFILE_FOOTER,
                        Dated = DateTime.Now.ToLongDateString()
                    },
                    ProjectProfile = profileView
                };
                return await Task<ProjectProfileReport>.Run(() => projectProfileReport).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<ProjectProfileView>> SearchProjectsByCriteria(SearchProjectModel model, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IQueryable<EFProject> projectProfileList = null;
                List<ProjectProfileView> projectsList = new List<ProjectProfileView>();

                try
                {
                    exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                    if (model.ProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (!string.IsNullOrEmpty(model.Description))
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => EF.Functions.Like(p.Description, "%" + model.Description + "%")
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where p.Description.Contains(model.Description, StringComparison.OrdinalIgnoreCase)
                                                  select p);
                        }
                    }

                    if (model.StartingYear != 0 && model.EndingYear != 0)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from project in projectProfileList
                                                  where project.StartingFinancialYear.FinancialYear >= model.StartingYear &&
                                                  project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                  select project);
                        }
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        var projectSectors = unitWork.ProjectSectorsRepository.GetMany(s => model.SectorIds.Contains(s.SectorId));
                        var projectIds = (from pSector in projectSectors
                                          select pSector.ProjectId).ToList<int>().Distinct();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
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


                        var projectIds = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.LocationIds.Count > 0)
                    {
                        var projectLocations = unitWork.ProjectLocationsRepository.GetMany(l => model.LocationIds.Contains(l.LocationId));
                        var projectIds = (from pLocation in projectLocations
                                          select pLocation.ProjectId).ToList<int>().Distinct();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers.Marker" });
                    }

                    foreach (var project in projectProfileList)
                    {
                        ProjectProfileView profileView = new ProjectProfileView();
                        project.ExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;                        
                        profileView.ProjectValueInDefaultCurrency = ((exchangeRate / project.ExchangeRate) * project.ProjectValue);
                        profileView.Description = project.Description;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);
                        profileView.Markers = mapper.Map<List<ProjectMarkersView>>(project.Markers);

                        projectsList.Add(profileView);
                    }

                    if (model.LowerRange > 0)
                    {
                        projectsList = (from p in projectsList
                                        where p.ProjectValueInDefaultCurrency >= model.LowerRange
                                        select p).ToList();
                    }

                    if (model.UpperRange > 0)
                    {
                        projectsList = (from p in projectsList
                                        where p.ProjectValueInDefaultCurrency <= model.UpperRange
                                        select p).ToList();
                    }


                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<ProjectProfileView>.Run(() => projectsList).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<ProjectView>> SearchProjectsViewByCriteria(SearchProjectModel model, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate <= 0) ? 1 : exchangeRate;
                IQueryable<EFProject> projectProfileList = null;
                List<ProjectView> projectsList = new List<ProjectView>();

                try
                {
                    var financialYearSettings = unitWork.FinancialYearSettingsRepository.GetOne(fy => fy.Id != 0);
                    int fyDay = 1, fyMonth = 1;
                    if (financialYearSettings != null)
                    {
                        fyDay = financialYearSettings.Day;
                        fyMonth = financialYearSettings.Month;
                    }

                    int currentFinancialYear = DateTime.Now.Year;
                    int currentMonth = DateTime.Now.Month;
                    int currentDay = DateTime.Now.Day;

                    if (fyDay != 1 && fyMonth != 1)
                    {
                        if (currentMonth < fyMonth)
                        {
                            --currentFinancialYear;
                        }
                        else if (currentMonth == fyMonth && currentDay < fyDay)
                        {
                            --currentFinancialYear;
                        }
                    }
                    if (model.ProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                    }

                    if (!string.IsNullOrEmpty(model.Description))
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => EF.Functions.Like(p.Description, "%" + model.Description + "%")
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where p.Description.Contains(model.Description, StringComparison.OrdinalIgnoreCase)
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
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear"});
                            }

                            else if (model.EndingYear > 0 && model.StartingYear <= 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndingFinancialYear.FinancialYear <= model.EndingYear || p.StartingFinancialYear.FinancialYear <= model.EndingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                            }

                            else if (model.StartingYear > 0 && model.EndingYear > 0)
                            {
                                projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.StartingFinancialYear.FinancialYear <= model.EndingYear) || (p.StartingFinancialYear.FinancialYear <= model.StartingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)
                                                        || (p.EndingFinancialYear.FinancialYear <= model.EndingYear && p.EndingFinancialYear.FinancialYear >= model.StartingYear)),
                                    new string[] { "StartingFinancialYear", "EndingFinancialYear" });
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

                    if (model.SectorIds.Count > 0)
                    {
                        var projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(s => model.SectorIds.Contains(s.SectorId) ||
                        model.SectorIds.Contains((int)s.Sector.ParentSectorId), new string[] { "Sector" });
                        var projectIds = (from pSector in projectSectors
                                          select pSector.ProjectId).ToList<int>().Distinct();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id), new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetManyQueryable(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIds = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id), new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.LocationIds.Count > 0)
                    {
                        var projectLocations = unitWork.ProjectLocationsRepository.GetManyQueryable(l => model.LocationIds.Contains(l.LocationId));
                        var projectIds = (from pLocation in projectLocations
                                          select pLocation.ProjectId).ToList<int>().Distinct();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id), new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                    }

                    var projectDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.Year.FinancialYear == currentFinancialYear && d.DisbursementType == DisbursementTypes.Planned, new string[] { "Year" });
                    string yearLabel = "";
                    if (projectProfileList.Any())
                    {
                        yearLabel = (from d in projectDisbursements
                                     where d.Year.FinancialYear == currentFinancialYear
                                     select d.Year.Label).FirstOrDefault();
                    }
                    foreach (var project in projectProfileList)
                    {
                        decimal currentFYPlannedDisbursements = (from d in projectDisbursements
                                                                 where d.ProjectId == project.Id && d.Year.FinancialYear == currentFinancialYear
                                                                 select ((exchangeRate / d.ExchangeRate) * d.Amount)).FirstOrDefault();
                        project.ExchangeRate = (project.ExchangeRate <= 0) ? 1 : project.ExchangeRate;
                        ProjectView profileView = new ProjectView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.ProjectValueInDefaultCurrency = ((exchangeRate / project.ExchangeRate) * project.ProjectValue);
                        profileView.Description = project.Description;
                        profileView.CurrentYearLabel = yearLabel;
                        profileView.CurrentYearPlannedDisbursements = currentFYPlannedDisbursements;
                        projectsList.Add(profileView);
                    }

                    if (model.LowerRange > 0)
                    {
                        projectsList = (from p in projectsList
                                        where p.ProjectValueInDefaultCurrency >= model.LowerRange
                                        select p).ToList();
                    }

                    if (model.UpperRange > 0)
                    {
                        projectsList = (from p in projectsList
                                        where p.ProjectValueInDefaultCurrency <= model.UpperRange
                                        select p).ToList();
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<ProjectProfileView>.Run(() => projectsList).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<ProjectProfileView>> GetProjectsByIdsAsync(List<int> ids)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ids.Contains(p.Id), new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers.Marker" });
                List<ProjectProfileView> profileViewList = new List<ProjectProfileView>();

                if (projectProfileList != null)
                {
                    foreach (var project in projectProfileList)
                    {
                        string startDate = (project.StartDate != null) ? Convert.ToDateTime(project.StartDate).ToShortDateString() : null;
                        string endDate = (project.EndDate != null) ? Convert.ToDateTime(project.EndDate).ToShortDateString() : null;
                        if (project.Locations.Count > 0)
                        {
                            project.Locations = (from l in project.Locations
                                                 orderby l.Location.Location
                                                 select l).ToList();
                        }
                        if (project.Sectors.Count > 0)
                        {
                            project.Sectors = (from s in project.Sectors
                                               orderby s.Sector.SectorName
                                               select s).ToList();
                        }
                        profileViewList.Add(new ProjectProfileView()
                        {
                            Id = project.Id,
                            Title = project.Title,
                            ProjectCurrency = project.ProjectCurrency,
                            ExchangeRate = project.ExchangeRate,
                            ProjectValue = project.ProjectValue,
                            Description = project.Description,
                            StartDate = startDate,
                            EndDate = endDate,
                            StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString(),
                            EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString(),
                            Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors),
                            Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations),
                            Funders = mapper.Map<List<ProjectFunderView>>(project.Funders),
                            Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers),
                            Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements),
                            Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents),
                            Markers = mapper.Map<List<ProjectMarkersView>>(project.Markers)
                        });
                    }
                }
                return await Task<IEnumerable<ProjectProfileView>>.Run(() => profileViewList).ConfigureAwait(false);
            }
        }

        public async Task<FilteredProjectProfileReport> GetProjectsReportForSectorAsync(int sectorId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                FilteredProjectProfileReport projectProfileReport = new FilteredProjectProfileReport()
                {
                    ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_PROFILE_TITLE,
                        SubTitle = ReportConstants.PROJECTS_PROFILE_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_PROFILE_FOOTER,
                        Dated = DateTime.Now.ToLongDateString()
                    },
                    ProjectsList = projectsList
                };

                List<int> projectIds = new List<int>();
                var sectorsList = unitWork.ProjectSectorsRepository.GetMany(s => s.SectorId == sectorId);
                foreach (var sector in sectorsList)
                {
                    projectIds.Add(sector.ProjectId);
                }

                if (projectIds.Count > 0)
                {
                    var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id), new string[] { "Sectors", "Locations", "Disbursements", "Funders", "Implementers", "Documents" });
                    if (projectProfileList != null)
                    {
                        foreach (var project in projectProfileList)
                        {
                            ProjectProfileView profileView = new ProjectProfileView();
                            profileView.Id = project.Id;
                            profileView.Title = project.Title;
                            profileView.Description = project.Description;
                            profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                            profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                            profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                            profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                            profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                            profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                            profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                            profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);
                            projectsList.Add(profileView);
                        }
                    }
                    projectProfileReport.ProjectsList = projectsList;
                }

                return await Task<FilteredProjectProfileReport>.Run(() => projectProfileReport).ConfigureAwait(false);
            }
        }

        public ProjectReport GetProjectReport(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectReport report = new ProjectReport();
                return report;
            }
        }

        public IEnumerable<ProjectView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectView> sectorTypesList = new List<ProjectView>();
                var projects = unitWork.ProjectRepository.GetMany(p => p.Title.Contains(criteria, StringComparison.OrdinalIgnoreCase));
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public IEnumerable<LocationView> GetProjectLocations(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var locations = unitWork.ProjectLocationsRepository.GetWithInclude(l => l.ProjectId == id, new string[] { "Location" });
                locations = (from l in locations
                             orderby l.Location.Location
                             select l);
                return mapper.Map<List<LocationView>>(locations);
            }
        }

        public IEnumerable<ProjectSectorView> GetProjectSectors(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.ProjectSectorsRepository.GetWithInclude(s => s.ProjectId == id, new string[] { "Sector" });
                sectors = (from s in sectors
                           orderby s.Sector.SectorName
                           select s);
                return mapper.Map<List<ProjectSectorView>>(sectors);
            }
        }

        public IEnumerable<ProjectFundsView> GetProjectFunds(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var funds = unitWork.ProjectFundersRepository.GetWithInclude(f => f.ProjectId == id, new string[] { "Funder" });
                return mapper.Map<List<ProjectFundsView>>(funds);
            }
        }

        public IEnumerable<ProjectFunderView> GetProjectFunders(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var funders = unitWork.ProjectFundersRepository.GetWithInclude(s => s.ProjectId == id, new string[] { "Funder" });
                return mapper.Map<List<ProjectFunderView>>(funders);
            }
        }

        public IEnumerable<ProjectImplementerView> GetProjectImplementers(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var implementers = unitWork.ProjectImplementersRepository.GetWithInclude(s => s.ProjectId == id, new string[] { "Implementer" });
                return mapper.Map<List<ProjectImplementerView>>(implementers);
            }
        }

        public async Task<IEnumerable<ProjectDisbursementView>> CreateProjectDisbursementsAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                decimal exchangeRate = 0;
                List<ProjectDisbursementView> disbursementsList = new List<ProjectDisbursementView>();
                var project = unitWork.ProjectRepository.GetWithInclude(p => p.Id == id, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Disbursements.Year" }).FirstOrDefault();
                
                if (project != null)
                {
                    if (project.Disbursements.Count == 0)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                var financialYears = await unitWork.FinancialYearRepository.GetManyQueryableAsync(f => f.Id != 0);
                                exchangeRate = project.ExchangeRate;
                                var fySettings = await unitWork.FinancialYearSettingsRepository.GetOneAsync(f => f.Id != 0);
                                int fyMonth = 0, fyDay = 0, currentYear = DateTime.Now.Year,
                                    startMonth = project.StartDate.Month, startDay = project.StartDate.Day;
                                int startYear = project.StartingFinancialYear.FinancialYear, endYear = project.EndingFinancialYear.FinancialYear;
                                if (fySettings != null)
                                {
                                    fyMonth = fySettings.Month;
                                    fyDay = fySettings.Day;
                                }

                                bool isSimilarToCalendarYear = (fyMonth == 1 && fyDay == 1) ? true : false;
                                int startingYear = 0, endingYear = 0, currentActiveYear = DateTime.Now.Year;
                                startMonth = project.StartDate.Month;
                                startDay = project.StartDate.Day;
                                currentActiveYear = DateTime.Now.Year;
                                startingYear = project.StartingFinancialYear.FinancialYear;
                                endingYear = project.EndingFinancialYear.FinancialYear;

                                if (!isSimilarToCalendarYear)
                                {
                                    if (startMonth < fyMonth)
                                    {
                                        --currentActiveYear;
                                    }
                                    else if (startMonth == fyMonth && startDay < fyDay)
                                    {
                                        --currentActiveYear;
                                    }
                                }

                                for (int yr = startingYear; yr <= endingYear; yr++)
                                {
                                    var financialYear = (from fy in financialYears
                                                         where fy.FinancialYear == yr
                                                         select fy).FirstOrDefault();
                                    if (financialYear == null)
                                    {
                                        string label = (isSimilarToCalendarYear) ? "FY " + yr : "FY " + yr + "/" + (yr + 1);
                                        financialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            Label = label,
                                            FinancialYear = yr
                                        });
                                        unitWork.Save();
                                    }

                                    if (yr < currentActiveYear)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = 0,
                                            Currency = project.ProjectCurrency,
                                            DisbursementType = DisbursementTypes.Actual,
                                            ExchangeRate = project.ExchangeRate,
                                        });
                                        unitWork.Save();
                                    }
                                    else if (yr == currentActiveYear)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = 0,
                                            Currency = project.ProjectCurrency,
                                            DisbursementType = DisbursementTypes.Actual,
                                            ExchangeRate = project.ExchangeRate,
                                        });
                                        unitWork.Save();

                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = 0,
                                            Currency = project.ProjectCurrency,
                                            DisbursementType = DisbursementTypes.Planned,
                                            ExchangeRate = project.ExchangeRate,
                                        });
                                        unitWork.Save();
                                    }
                                    else if (yr > currentActiveYear)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = 0,
                                            Currency = project.ProjectCurrency,
                                            DisbursementType = DisbursementTypes.Planned,
                                            ExchangeRate = project.ExchangeRate,
                                        });
                                        unitWork.Save();
                                    }
                                }
                                transaction.Commit();
                                var disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(p => p.ProjectId == id, new string[] { "Year" } );
                                if (disbursements.Any())
                                {
                                    disbursementsList = mapper.Map<List<ProjectDisbursementView>>(disbursements);
                                }
                            }
                        });
                    }
                }
                return disbursementsList;
            }
        }

        public async Task<IEnumerable<ProjectDisbursementView>> AdjustProjectDisbursementsAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                bool isSaved = false;
                decimal exchangeRate = 0;
                List<ProjectDisbursementView> disbursementsList = new List<ProjectDisbursementView>();
                var disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == id, new string[] { "Year" });
                var project = unitWork.ProjectRepository.GetWithInclude(p => p.Id == id, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Disbursements.Year" }).FirstOrDefault();
                if (project != null)
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(y => y.Id != 0);
                            exchangeRate = project.ExchangeRate;
                            var fySettings = await unitWork.FinancialYearSettingsRepository.GetOneAsync(f => f.Id != 0);
                            int fyMonth = 0, fyDay = 0, currentYear = DateTime.Now.Year,
                                startMonth = project.StartDate.Month, startDay = project.StartDate.Day;
                            int startYear = project.StartingFinancialYear.FinancialYear, endYear = project.EndingFinancialYear.FinancialYear;
                            if (fySettings != null)
                            {
                                fyMonth = fySettings.Month;
                                fyDay = fySettings.Day;
                            }

                            bool isSimilarToCalendarYear = (fyMonth == 1 && fyDay == 1) ? true : false;
                            int startingYear = 0, endingYear = 0, currentActiveYear = DateTime.Now.Year;
                            startMonth = project.StartDate.Month;
                            startDay = project.StartDate.Day;
                            currentActiveYear = DateTime.Now.Year;
                            startingYear = project.StartingFinancialYear.FinancialYear;
                            endingYear = project.EndingFinancialYear.FinancialYear;

                            if (!isSimilarToCalendarYear)
                            {
                                if (startMonth < fyMonth)
                                {
                                    --currentActiveYear;
                                }
                                else if (startMonth == fyMonth && startDay < fyDay)
                                {
                                    --currentActiveYear;
                                }
                            }

                            var disbursementsToDelete = (from disbursement in project.Disbursements
                                                         where (disbursement.Year.FinancialYear < startingYear) ||
                                                         (disbursement.Year.FinancialYear > endingYear) ||
                                                         (disbursement.Year.FinancialYear > currentActiveYear &&
                                                            disbursement.DisbursementType == DisbursementTypes.Actual)
                                                         select disbursement);
                            bool isDeleted = false;
                            decimal deletedActualDisbursements = 0, deletedPlannedDisbursements = 0;
                            foreach (var disbursement in disbursementsToDelete)
                            {
                                if (disbursement.DisbursementType == DisbursementTypes.Actual)
                                {
                                    deletedActualDisbursements += disbursement.Amount;
                                }
                                else if (disbursement.DisbursementType == DisbursementTypes.Planned)
                                {
                                    deletedPlannedDisbursements += disbursement.Amount;
                                }
                                unitWork.ProjectDisbursementsRepository.Delete(disbursement);
                                isDeleted = true;
                            }
                            if (isDeleted)
                            {
                                unitWork.Save();
                            }

                            var projectDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == project.Id, new string[] { "Year" });
                            for(int yr = startingYear; yr <= endingYear; yr++)
                            {
                                var financialYear = (from fy in financialYears
                                                     where fy.FinancialYear == yr
                                                     select fy).FirstOrDefault();
                                if (financialYear == null)
                                {
                                    string label = (isSimilarToCalendarYear) ? "FY " + yr : "FY " + yr + "/" + (yr + 1);
                                    financialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                    {
                                        FinancialYear = yr,
                                        Label = label
                                    });
                                    unitWork.Save();
                                }

                                if (yr < currentActiveYear)
                                {
                                    var disbursement = (from d in projectDisbursements
                                                        where d.Year.FinancialYear == yr
                                                        && d.DisbursementType == DisbursementTypes.Actual
                                                        select d).FirstOrDefault();
                                    if (disbursement == null)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = deletedActualDisbursements,
                                            Currency = project.ProjectCurrency,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = DisbursementTypes.Actual
                                        });
                                        unitWork.Save();
                                        deletedActualDisbursements = 0;
                                    }
                                }
                                else if (yr == currentActiveYear)
                                {
                                    var actualDisbursement = (from d in projectDisbursements
                                                        where d.Year.FinancialYear == yr
                                                        && d.DisbursementType == DisbursementTypes.Actual
                                                        select d).FirstOrDefault();
                                    if (actualDisbursement == null)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = deletedActualDisbursements,
                                            Currency = project.ProjectCurrency,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = DisbursementTypes.Actual
                                        });
                                        unitWork.Save();
                                        deletedActualDisbursements = 0;
                                    }

                                    var plannedDisbursement = (from d in projectDisbursements
                                                              where d.Year.FinancialYear == yr
                                                              && d.DisbursementType == DisbursementTypes.Planned
                                                              select d).FirstOrDefault();
                                    if (plannedDisbursement == null)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = deletedPlannedDisbursements,
                                            Currency = project.ProjectCurrency,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = DisbursementTypes.Planned
                                        });
                                        unitWork.Save();
                                        deletedPlannedDisbursements = 0;
                                    }
                                }
                                else if (yr > currentActiveYear)
                                {
                                    var disbursement = (from d in projectDisbursements
                                                        where d.Year.FinancialYear == yr
                                                        && d.DisbursementType == DisbursementTypes.Planned
                                                        select d).FirstOrDefault();
                                    if (disbursement == null)
                                    {
                                        unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = deletedPlannedDisbursements,
                                            Currency = project.ProjectCurrency,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = DisbursementTypes.Planned
                                        });
                                        unitWork.Save();
                                        deletedPlannedDisbursements = 0;
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                    });
                }

                if (isSaved)
                {
                    disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == id, new string[] { "Year" });
                }
                if (disbursements.Any())
                {
                    disbursements = (from d in disbursements
                                     orderby d.Year.FinancialYear, d.DisbursementType
                                     select d);
                }
                return mapper.Map<List<ProjectDisbursementView>>(disbursements);
            }
        }

        public async Task<IEnumerable<ProjectDisbursementView>> GetProjectDisbursementsAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectDisbursementView> disbursementsList = new List<ProjectDisbursementView>();
                var disbursements = await unitWork.ProjectDisbursementsRepository.GetWithIncludeAsync(d => d.ProjectId == id, new string[] { "Year" });
                if (disbursements.Any())
                {
                    disbursements = (from d in disbursements
                                     orderby d.Year.FinancialYear, d.DisbursementType
                                     select d);
                }
                return mapper.Map<List<ProjectDisbursementView>>(disbursements);
            }
        }

        public IEnumerable<ProjectDocumentView> GetProjectDocuments(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var documents = unitWork.ProjectDocumentRepository.GetMany(d => d.ProjectId == id);
                return mapper.Map<List<ProjectDocumentView>>(documents);
            }
        }

        public IEnumerable<UserProjectsView> GetUserProjects(int userId, int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(f => f.FunderId == funderId, f => f.ProjectId).ToList();
                var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ImplementerId == funderId, i => i.ProjectId).ToList();
                var membershipProjectIds = unitWork.ProjectMembershipRepository.GetProjection(m => (m.UserId == userId && m.IsApproved == true), m => m.ProjectId);
                var combinedProjectIds = funderProjectIds.Union(implementerProjectIds);
                membershipProjectIds = membershipProjectIds.Union(combinedProjectIds);
                List<UserProjectsView> projectIds = new List<UserProjectsView>();
                foreach (var id in membershipProjectIds)
                {
                    projectIds.Add(new UserProjectsView() { Id = id });
                }
                return projectIds;
            }
        }

        public async Task<ActionResponse> AddAsync(ProjectModel model, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                try
                {
                    var createdBy = unitWork.UserRepository.GetByID(userId);
                    if (createdBy == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetUnAuthorizedAccessMessage();
                        return response;
                    }

                    var primarySectorType = unitWork.SectorTypesRepository.GetOne(t => t.IsPrimary == true);
                    if (primarySectorType == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.PrimarySectorTypeMissing();
                        response.Success = false;
                        return response;
                    }

                    var fundingType = unitWork.FundingTypeRepository.GetOne(f => f.Id == model.FundingTypeId);
                    if (fundingType == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Funding Type");
                        return response;
                    }

                    int fyMonth = 1, fyDay = 1, startingMonth = model.StartDate.Month, startDay = model.StartDate.Day,
                        endingMonth = model.EndDate.Month, endingDay = model.EndDate.Day;
                    var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
                    if (fySettings != null)
                    {
                        fyMonth = fySettings.Month;
                        fyDay = fySettings.Day;
                    }

                    if (fyMonth > 1)
                    {
                        if (startingMonth < fyMonth)
                        {
                            model.StartingFinancialYear = (model.StartingFinancialYear - 1);
                        }
                        else if (startingMonth == fyMonth && fyDay < startDay)
                        {
                            model.StartingFinancialYear = (model.StartingFinancialYear - 1);
                        }
                    }

                    var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => (f.FinancialYear == model.StartingFinancialYear || f.FinancialYear == model.EndingFinancialYear));
                    var currency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.ProjectCurrency);
                    if (currency == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Currency");
                        return response;
                    }

                    if (model.ExchangeRate <= 0)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Exchange rate for project financial years");
                        return response;
                    }

                    var startingFinancialYear = (from fy in financialYears
                                                 where fy.FinancialYear == model.StartingFinancialYear
                                                 select fy).FirstOrDefault();

                    if (startingFinancialYear == null)
                    {
                        startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                        {
                            FinancialYear = model.StartingFinancialYear
                        });
                        unitWork.Save();
                    }

                    var endingFinancialYear = (from fy in financialYears
                                               where fy.FinancialYear == model.EndingFinancialYear
                                               select fy).FirstOrDefault();

                    if (startingFinancialYear == null)
                    {
                        endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                        {
                            FinancialYear = model.EndingFinancialYear
                        });
                        unitWork.Save();
                    }

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var newProject = unitWork.ProjectRepository.Insert(new EFProject()
                            {
                                Title = model.Title,
                                Description = model.Description,
                                FundingType = fundingType,
                                StartDate = model.StartDate,
                                EndDate = model.EndDate,
                                StartingFinancialYear = startingFinancialYear,
                                EndingFinancialYear = endingFinancialYear,
                                ProjectValue = model.ProjectValue,
                                ExchangeRate = model.ExchangeRate,
                                ProjectCurrency = model.ProjectCurrency,
                                DateUpdated = DateTime.Now,
                                CreatedById = userId
                            });
                            await unitWork.SaveAsync();
                            //Add user organization to valid funders
                            unitWork.ProjectMembershipRepository.Insert(new EFProjectMembershipRequests()
                            {
                                ProjectId = newProject.Id,
                                UserId = userId,
                                Dated = DateTime.Now,
                                IsApproved = true
                            });
                            await unitWork.SaveAsync();
                            response.ReturnedId = newProject.Id;

                            /*var unattributedLocation = unitWork.LocationRepository.GetOne(l => l.Location.Equals(UNATTRIBUTED, StringComparison.OrdinalIgnoreCase));
                            if (unattributedLocation == null)
                            {
                                unattributedLocation = unitWork.LocationRepository.Insert(new EFLocation()
                                {
                                    Location = UNATTRIBUTED,
                                    Longitude = 0,
                                    Latitude = 0,
                                    IsUnAttributed = true
                                });
                                unitWork.Save();
                            }
                            unitWork.ProjectLocationsRepository.Insert(new EFProjectLocations()
                            {
                                Project = newProject,
                                Location = unattributedLocation,
                                FundsPercentage = 100
                            });
                            unitWork.Save();
                           
                            var unattributedSector = unitWork.SectorRepository.GetOne(s => s.SectorName.Equals(UNATTRIBUTED, StringComparison.OrdinalIgnoreCase));
                            if (unattributedSector == null)
                            {
                                unattributedSector = unitWork.SectorRepository.Insert(new EFSector()
                                {
                                    SectorName = UNATTRIBUTED,
                                    SectorType = primarySectorType,
                                    ParentSector = null,
                                    IsUnAttributed = true
                                });
                                unitWork.Save();
                            }
                            unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                            {
                                Project = newProject,
                                Sector = unattributedSector,
                                FundsPercentage = 100
                            });
                            unitWork.Save();*/
                            transaction.Commit();
                        }
                    });
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> AdjustDisbursementsForProjectsAsync()
        {
            ActionResponse response = new ActionResponse();
            IMessageHelper mHelper;
            var unitWork = new UnitOfWork(context);
            var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(s => s.Id != 0);
            if (fySettings == null)
            {
                mHelper = new MessageHelper();
                response.Success = false;
                response.Message = mHelper.GetNotFound("Financial year settings");
                return response;
            }

            try
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        bool isSimilarToCalendarYear = (fySettings.Month == 1 && fySettings.Day == 1) ? true : false;
                        int fyMonth = 0, fyDay = 0;
                        if (fySettings != null)
                        {
                            fyMonth = fySettings.Month;
                            fyDay = fySettings.Day;
                        }

                        int startingYear = 0, endingYear = 0, startMonth = 0, startDay = 0, currentActiveYear = DateTime.Now.Year;
                        var adjustProjects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndDate.Year >= currentActiveYear, new string[] { "EndingFinancialYear", "Disbursements", "Disbursements.Year" });
                        foreach (var project in adjustProjects)
                        {
                            startMonth = project.StartDate.Month;
                            startDay = project.StartDate.Day;
                            currentActiveYear = DateTime.Now.Year;
                            startingYear = project.StartingFinancialYear.FinancialYear;
                            endingYear = project.EndingFinancialYear.FinancialYear;

                            if (!isSimilarToCalendarYear)
                            {
                                if (startMonth < fyMonth)
                                {
                                    --currentActiveYear;
                                }
                                else if (startMonth == fyMonth && startDay < fyDay)
                                {
                                    --currentActiveYear;
                                }
                            }

                            var disbursementsToDelete = (from disbursement in project.Disbursements
                                                         where (disbursement.Year.FinancialYear < startingYear) ||
                                                         (disbursement.Year.FinancialYear > endingYear) ||
                                                         (disbursement.Year.FinancialYear != currentActiveYear &&
                                                            disbursement.DisbursementType == DisbursementTypes.Actual)
                                                         select disbursement);
                            bool isDeleted = false;
                            decimal deletedActualDisbursements = 0, deletedPlannedDisbursements = 0;
                            foreach (var disbursement in disbursementsToDelete)
                            {
                                if (disbursement.DisbursementType == DisbursementTypes.Actual)
                                {
                                    deletedActualDisbursements += disbursement.Amount;
                                }
                                else if (disbursement.DisbursementType == DisbursementTypes.Planned)
                                {
                                    deletedPlannedDisbursements += disbursement.Amount;
                                }
                                unitWork.ProjectDisbursementsRepository.Delete(disbursement);
                                isDeleted = true;
                            }
                            if (isDeleted)
                            {
                                unitWork.Save();
                            }

                            var disbursementsForCurrentYear = (from disbursement in project.Disbursements
                                                               where (disbursement.Year.FinancialYear == currentActiveYear) &&
                                                               (disbursement.DisbursementType == DisbursementTypes.Planned ||
                                                               disbursement.DisbursementType == DisbursementTypes.Actual)
                                                               select disbursement);
                            var actualDisbursementCurrentYear = (from disbursement in disbursementsForCurrentYear
                                                                 where disbursement.DisbursementType == DisbursementTypes.Actual
                                                                 select disbursement).FirstOrDefault();
                            var plannedDisbursementCurrentYear = (from disbursement in disbursementsForCurrentYear
                                                                  where disbursement.DisbursementType == DisbursementTypes.Planned
                                                                  select disbursement).FirstOrDefault();
                            var currentFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == currentActiveYear);
                            if (currentFinancialYear == null)
                            {
                                string label = (fyMonth == 1 && fyDay == 1) ? "FY " + currentActiveYear : "FY " + currentActiveYear + "/" + (currentActiveYear + 1);
                                currentFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = currentActiveYear,
                                    Label = label
                                });
                                unitWork.Save();
                            }

                            if (actualDisbursementCurrentYear == null)
                            {
                                actualDisbursementCurrentYear = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                {
                                    Project = project,
                                    Year = currentFinancialYear,
                                    Currency = project.ProjectCurrency,
                                    Amount = deletedActualDisbursements,
                                    ExchangeRate = project.ExchangeRate,
                                    DisbursementType = DisbursementTypes.Actual
                                });
                                unitWork.Save();
                            }
                            else
                            {
                                actualDisbursementCurrentYear.Amount += deletedActualDisbursements;
                                unitWork.ProjectDisbursementsRepository.Update(actualDisbursementCurrentYear);
                                unitWork.Save();
                            }
                            if (plannedDisbursementCurrentYear == null)
                            {
                                plannedDisbursementCurrentYear = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                {
                                    Project = project,
                                    Year = currentFinancialYear,
                                    Currency = project.ProjectCurrency,
                                    Amount = deletedPlannedDisbursements,
                                    ExchangeRate = project.ExchangeRate,
                                    DisbursementType = DisbursementTypes.Planned
                                });
                                unitWork.Save();
                            }
                            else
                            {
                                plannedDisbursementCurrentYear.Amount += deletedPlannedDisbursements;
                                unitWork.ProjectDisbursementsRepository.Update(plannedDisbursementCurrentYear);
                                unitWork.Save();
                            }
                        }
                        transaction.Commit();
                    }
                });
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }

        public ActionResponse UpdateFinancialYearsForProjects()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                int month = 0, day = 1;
                ActionResponse response = new ActionResponse();
                var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(s => s.Id != 0);
                if (fySettings != null)
                {
                    month = fySettings.Month;
                    day = fySettings.Day;
                }

                EFFinancialYears startingFinancialYear, endingFinancialYear = null;
                var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(y => y.Id != 0);
                var projects = unitWork.ProjectRepository.GetWithInclude(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                foreach (var project in projects)
                {
                    int startingYear = project.StartDate.Year;
                    int endingYear = project.EndDate.Year;
                    if (month == 1 && day == 1)
                    {
                        startingFinancialYear = (from fy in financialYears
                                                 where fy.FinancialYear == startingYear
                                                 select fy).FirstOrDefault();

                        if (startingFinancialYear == null)
                        {
                            startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                            {
                                FinancialYear = startingYear,
                                Label = "FY " + startingYear
                            });
                        }
                        else
                        {
                            startingFinancialYear.Label = "FY " + startingYear;
                        }
                        unitWork.Save();

                        endingFinancialYear = (from fy in financialYears
                                               where fy.FinancialYear == endingYear
                                               select fy).FirstOrDefault();
                        if (endingFinancialYear == null)
                        {
                            endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                            {
                                FinancialYear = endingYear,
                                Label = "FY " + endingYear
                            });
                        }
                        else
                        {
                            endingFinancialYear.Label = "FY " + endingYear;
                        }
                        unitWork.Save();
                    }
                    else if (month >= 1 && day > 1)
                    {
                        int startingMonth = project.StartDate.Month;
                        int startingDay = project.StartDate.Day;
                        int endingMonth = project.EndDate.Month;
                        int endingDay = project.EndDate.Day;

                        if (startingMonth < month)
                        {
                            --startingYear;
                        }
                        else if (startingMonth == month && startingDay < day)
                        {
                            --startingYear;
                        }

                        startingFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == startingYear);
                        if (startingFinancialYear == null)
                        {
                            startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                            {
                                FinancialYear = startingYear,
                                Label = "FY " + startingYear + "/" + (startingYear + 1)
                            });
                            unitWork.Save();
                        }

                        if (endingMonth < month)
                        {
                            --endingYear;
                        }
                        else if (endingMonth == month && endingDay < day)
                        {
                            --endingYear;
                        }

                        endingFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == endingYear);
                        if (endingFinancialYear == null)
                        {
                            endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                            {
                                FinancialYear = endingYear,
                                Label = "FY " + endingYear + "/" + (startingYear + 1)
                            });
                            unitWork.Save();
                        }

                        project.StartingFinancialYear = startingFinancialYear;
                        project.EndingFinancialYear = endingFinancialYear;
                        unitWork.Save();
                    }
                }
                return response;
            }
        }

        public async Task<ActionResponse> ImportProjects(List<NewImportedAidData> projects)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                EFFinancialYears startingFinancialYear = null;
                EFFinancialYears endingFinancialYear = null;
                EFUser user = unitWork.UserRepository.GetOne(u => u.UserType == UserTypes.Manager);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetUnAuthorizedAccessMessage();
                    return response;
                }

                var defaultSectorType = unitWork.SectorTypesRepository.GetOne(t => t.IsPrimary == true);
                if (defaultSectorType == null)
                {
                   defaultSectorType = unitWork.SectorTypesRepository.Insert(new EFSectorTypes()
                    {
                        TypeName = "Somali Sectors",
                        IsPrimary = true,
                        IsSourceType = false,
                        IATICode = null
                    });
                }
                int currentYear = DateTime.Now.Year, fyMonth = 1, fyDay = 1;
                var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(p => p.Id != 0).ToList();
                var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => o.Id != 0).ToList();
                var ndpSectors = unitWork.SectorRepository.GetManyQueryable(s => s.SectorTypeId == defaultSectorType.Id).ToList();
                var locations = unitWork.LocationRepository.GetManyQueryable(l => l.Id != 0).ToList();
                var fundingType = unitWork.FundingTypeRepository.GetOne(f => f.Id == 1);
                var organizationType = unitWork.OrganizationTypesRepository.GetOne(o => o.Id == 1);
                var markers = unitWork.MarkerRepository.GetManyQueryable(m => m.Id != 0);
                var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
                if (fySettings == null)
                {
                    fySettings = unitWork.FinancialYearSettingsRepository.Insert(new EFFinancialYearSettings()
                    {
                        Day = 1,
                        Month = 1
                    });
                    unitWork.Save();
                }
                else
                {
                    fyMonth = fySettings.Month;
                    fyDay = fySettings.Day;
                }
                bool isCalendarYear = (fyMonth == 1 && fyDay == 1) ? true : false;

                List<EFSector> ndpSectorsList = new List<EFSector>();
                List<EFLocation> locationsList = new List<EFLocation>();
                EFFinancialYears twentySixteenFinancialYear = null;
                EFFinancialYears twentySeventeenFinancialYear = null;
                EFFinancialYears twentyEighteenFinancialYear = null;
                EFFinancialYears twentyNineteenFinancialYear = null;
                EFFinancialYears twentyTwentyFinancialYear = null;
                EFFinancialYears twentyOneFinancialYear = null;
                EFFinancialYears twentyTwoFinancialYear = null;
                EFFinancialYears twentyThreeFinancialYear = null;
                EFFinancialYears twentyFourFinancialYear = null;


                foreach (var sector in ndpSectors)
                {
                    ndpSectorsList.Add(sector);
                }

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            twentySixteenFinancialYear = (from fy in financialYears
                                                          where fy.FinancialYear == (2016)
                                                          select fy).FirstOrDefault();
                            if (twentySixteenFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2016" : "FY 2016/2017";
                                twentySixteenFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears() 
                                { 
                                    FinancialYear = 2016, 
                                    Label = label 
                                });
                                unitWork.Save();
                                financialYears.Add(twentySixteenFinancialYear);
                            }

                            twentySeventeenFinancialYear = (from fy in financialYears
                                                            where fy.FinancialYear == (2017)
                                                            select fy).FirstOrDefault();

                            if (twentySeventeenFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2017" : "FY 2017/2018";
                                twentySeventeenFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2017,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentySeventeenFinancialYear);
                            }

                            twentyEighteenFinancialYear = (from fy in financialYears
                                                           where fy.FinancialYear == (2018)
                                                           select fy).FirstOrDefault();
                            if (twentyEighteenFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2018" : "FY 2018/2019";
                                twentyEighteenFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2018,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyEighteenFinancialYear);
                            }

                            twentyNineteenFinancialYear = (from fy in financialYears
                                                           where fy.FinancialYear == (2019)
                                                           select fy).FirstOrDefault();
                            if (twentyNineteenFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2019" : "FY 2019/2020";
                                twentyNineteenFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2019,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyNineteenFinancialYear);
                            }

                            twentyTwentyFinancialYear = (from fy in financialYears
                                                         where fy.FinancialYear == (2020)
                                                         select fy).FirstOrDefault();

                            if (twentyTwentyFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2020" : "FY 2020/2021";
                                twentyTwentyFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2020,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyTwentyFinancialYear);
                            }

                            twentyOneFinancialYear = (from fy in financialYears
                                                      where fy.FinancialYear == (2021)
                                                      select fy).FirstOrDefault();

                            if (twentyOneFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2021" : "FY 2021/2022";
                                twentyOneFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2021,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyOneFinancialYear);
                            }

                            twentyTwoFinancialYear = (from fy in financialYears
                                                      where fy.FinancialYear == (2022)
                                                      select fy).FirstOrDefault();

                            if (twentyTwoFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2022" : "FY 2022/2023";
                                twentyTwoFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2022,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyTwoFinancialYear);
                            }

                            twentyThreeFinancialYear = (from fy in financialYears
                                                        where fy.FinancialYear == (2023)
                                                        select fy).FirstOrDefault();

                            if (twentyThreeFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2023" : "FY 2023/2024";
                                twentyThreeFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2023,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyThreeFinancialYear);
                            }

                            twentyFourFinancialYear = (from fy in financialYears
                                                       where fy.FinancialYear == (2024)
                                                       select fy).FirstOrDefault();

                            if (twentyFourFinancialYear == null)
                            {
                                string label = (isCalendarYear) ? "FY 2024" : "FY 2024/2025";
                                twentyFourFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = 2024,
                                    Label = label
                                });
                                unitWork.Save();
                                financialYears.Add(twentyFourFinancialYear);
                            }

                            foreach (var project in projects)
                            {
                                startingFinancialYear = (from y in financialYears
                                                         where y.FinancialYear == Convert.ToInt32(project.StartYear)
                                                         select y).FirstOrDefault();
                                if (startingFinancialYear == null)
                                {
                                    int financialYear = Convert.ToInt32(project.StartYear);
                                    startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears() { FinancialYear = financialYear, Label = "FY " + (financialYear - 1) + "/" + financialYear });
                                    unitWork.Save();
                                    financialYears.Add(startingFinancialYear);
                                }

                                endingFinancialYear = (from y in financialYears
                                                       where y.FinancialYear == Convert.ToInt32(project.EndYear)
                                                       select y).FirstOrDefault();
                                if (endingFinancialYear == null)
                                {
                                    int financialYear = Convert.ToInt32(project.EndYear);
                                    endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears() { FinancialYear = financialYear, Label = "FY " + (financialYear - 1) + "/" + financialYear });
                                    unitWork.Save();
                                    financialYears.Add(endingFinancialYear);
                                }

                                var newProject = unitWork.ProjectRepository.Insert(new EFProject()
                                {
                                    Title = project.ProjectTitle,
                                    ProjectCurrency = project.Currency,
                                    ProjectValue = project.ProjectValue,
                                    ExchangeRate = project.ExchangeRate,
                                    Description = project.ProjectDescription,
                                    StartDate = project.StartDate,
                                    EndDate = project.EndDate,
                                    StartingFinancialYear = startingFinancialYear,
                                    EndingFinancialYear = endingFinancialYear,
                                    CreatedBy = user,
                                    FundingType = fundingType,
                                    DateUpdated = DateTime.Now
                                });
                                unitWork.Save();

                                List<EFProjectFunders> projectFunders = new List<EFProjectFunders>();
                                List<EFOrganization> newOrganizations = new List<EFOrganization>();
                                if (!string.IsNullOrEmpty(project.Funders))
                                {
                                    string[] funderNames = project.Funders.Split(",");
                                    for (int i = 0; i < funderNames.Length; i++)
                                    {
                                        funderNames[i] = funderNames[i].Trim();
                                    }
                                    foreach (string funderName in funderNames)
                                    {
                                        EFProjectFunders isFunderExists = null;
                                        var organization = (from org in organizations
                                                            where org.OrganizationName.Equals(funderName, StringComparison.OrdinalIgnoreCase)
                                                            select org).FirstOrDefault();

                                        if (organization == null)
                                        {
                                            if (!string.IsNullOrEmpty(funderName) && !string.IsNullOrWhiteSpace(funderName))
                                            {
                                                organization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                                                {
                                                    OrganizationName = funderName,
                                                    OrganizationTypeId = 1,
                                                    IsApproved = true
                                                });
                                                unitWork.Save();
                                                newOrganizations.Add(organization);
                                                organizations.Add(organization);
                                            }
                                        }

                                        if (organization != null)
                                        {
                                            isFunderExists = (from f in projectFunders
                                                              where f.ProjectId == newProject.Id && f.FunderId == organization.Id
                                                              select f).FirstOrDefault();
                                        }

                                        if (isFunderExists == null && organization != null)
                                        {
                                            projectFunders.Add(new EFProjectFunders() { ProjectId = newProject.Id, FunderId = organization.Id });
                                        }
                                    }
                                }

                                if (projectFunders.Count > 0)
                                {
                                    unitWork.ProjectFundersRepository.InsertMultiple(projectFunders);
                                    unitWork.Save();
                                }

                                List<EFProjectImplementers> projectImplementers = new List<EFProjectImplementers>();
                                if (!string.IsNullOrEmpty(project.Implementers))
                                {
                                    string[] implementerNames = project.Implementers.Split(",");
                                    for (int i = 0; i < implementerNames.Length; i++)
                                    {
                                        implementerNames[i] = implementerNames[i].Trim();
                                    }
                                    EFOrganization implementer = null;
                                    foreach (string implementerName in implementerNames)
                                    {
                                        EFProjectImplementers isImplementerExists = null;
                                        implementer = (from org in organizations
                                                       where org.OrganizationName.Equals(implementerName.Trim(), StringComparison.OrdinalIgnoreCase)
                                                       select org).FirstOrDefault();

                                        if (implementer == null)
                                        {
                                            if (!string.IsNullOrEmpty(implementerName) && !string.IsNullOrWhiteSpace(implementerName))
                                            {
                                                implementer = unitWork.OrganizationRepository.Insert(new EFOrganization()
                                                {
                                                    OrganizationName = implementerName,
                                                    OrganizationTypeId = 1,
                                                    IsApproved = true
                                                });
                                                unitWork.Save();
                                                newOrganizations.Add(implementer);
                                                organizations.Add(implementer);
                                            }
                                        }

                                        if (implementer != null)
                                        {
                                            isImplementerExists = (from i in projectImplementers
                                                                   where i.ProjectId == newProject.Id && i.ImplementerId == implementer.Id
                                                                   select i).FirstOrDefault();
                                        }

                                        if (isImplementerExists == null && implementer != null)
                                        {
                                            projectImplementers.Add(new EFProjectImplementers() { ProjectId = newProject.Id, ImplementerId = implementer.Id });
                                        }
                                    }
                                }

                                if (projectImplementers.Count > 0)
                                {
                                    unitWork.ProjectImplementersRepository.InsertMultiple(projectImplementers);
                                    unitWork.Save();
                                }

                                List<EFProjectMarkers> projectMarkers = new List<EFProjectMarkers>();
                                if (project.CustomFields.Count > 0)
                                {
                                    foreach (var field in project.CustomFields)
                                    {
                                        var marker = (from m in markers
                                                      where m.FieldTitle.Equals(field.CustomField, StringComparison.OrdinalIgnoreCase)
                                                      select m).FirstOrDefault();

                                        if (marker == null)
                                        {
                                            marker = unitWork.MarkerRepository.Insert(new EFMarkers()
                                            {
                                                FieldTitle = field.CustomField,
                                                FieldType = FieldTypes.Text,
                                                Values = "",
                                                Help = ""
                                            });
                                            unitWork.Save();
                                        }

                                        var projectMarker = (from m in projectMarkers
                                                             where m.ProjectId == newProject.Id && m.MarkerId == marker.Id
                                                             select m).FirstOrDefault();

                                        if (marker != null && projectMarker == null)
                                        {
                                            unitWork.ProjectMarkersRepository.Insert(new EFProjectMarkers()
                                            {
                                                Project = newProject,
                                                Marker = marker,
                                                Values = field.Value
                                            });
                                        }
                                    }
                                    if (projectMarkers.Count > 0)
                                    {
                                        unitWork.ProjectMarkersRepository.InsertMultiple(projectMarkers);
                                        unitWork.Save();
                                    }
                                }

                                List<EFProjectDocuments> projectDocuments = new List<EFProjectDocuments>();
                                if (project.DocumentLinks.Count > 0)
                                {
                                    foreach (var document in project.DocumentLinks)
                                    {
                                        projectDocuments.Add(new EFProjectDocuments()
                                        {
                                            DocumentTitle = document.DocumentTitle,
                                            DocumentUrl = document.DocumentUrl,
                                            Project = newProject
                                        });
                                    }
                                }

                                if (projectDocuments.Count > 0)
                                {
                                    unitWork.ProjectDocumentRepository.InsertMultiple(projectDocuments);
                                    unitWork.Save();
                                }

                                if (!string.IsNullOrEmpty(project.Sector))
                                {
                                    var dbSector = (from sector in ndpSectorsList
                                                    where sector.SectorName.Equals(project.Sector, StringComparison.OrdinalIgnoreCase)
                                                    select sector).FirstOrDefault();
                                    if (dbSector == null)
                                    {
                                        dbSector = unitWork.SectorRepository.Insert(new EFSector()
                                        {
                                            SectorTypeId = 1,
                                            SectorName = project.Sector,
                                            ParentSector = null,
                                            TimeStamp = DateTime.Now
                                        });
                                    }
                                    ndpSectorsList.Add(dbSector);

                                    unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                                    {
                                        Project = newProject,
                                        Sector = dbSector,
                                        FundsPercentage = 100
                                    });
                                    unitWork.Save();
                                }

                                if (project.Locations.Count > 0)
                                {
                                    List<EFProjectLocations> newProjectLocations = new List<EFProjectLocations>();
                                    decimal fundsPercentage = 0;
                                    foreach (var location in project.Locations)
                                    {
                                        if (!string.IsNullOrEmpty(location.Location))
                                        {
                                            var dbLocation = (from loc in locations
                                                              where loc.Location.Equals(location.Location, StringComparison.OrdinalIgnoreCase)
                                                              select loc).FirstOrDefault();
                                            if (dbLocation == null)
                                            {
                                                dbLocation = unitWork.LocationRepository.Insert(new EFLocation()
                                                {
                                                    Location = location.Location
                                                });
                                                unitWork.Save();
                                                locations.Add(dbLocation);
                                            }

                                            if (location.Percentage > 0)
                                            {
                                                var isLocationExists = (from loc in newProjectLocations
                                                                        where loc.ProjectId == newProject.Id && loc.LocationId == dbLocation.Id
                                                                        select loc).FirstOrDefault();

                                                if (isLocationExists == null)
                                                {
                                                    fundsPercentage += location.Percentage;
                                                    newProjectLocations.Add(new EFProjectLocations()
                                                    {
                                                        Project = newProject,
                                                        Location = dbLocation,
                                                        FundsPercentage = location.Percentage
                                                    });
                                                }
                                            }
                                        }

                                        if (fundsPercentage == 100)
                                        {
                                            break;
                                        }
                                    }
                                    if (newProjectLocations.Count > 0)
                                    {
                                        unitWork.ProjectLocationsRepository.InsertMultiple(newProjectLocations);
                                        unitWork.Save();
                                    }
                                }

                                int startYear = Convert.ToDateTime(newProject.StartDate).Year;
                                int endYear = Convert.ToDateTime(newProject.EndDate).Year;
                                if (twentySixteenFinancialYear.FinancialYear >= startYear && twentySixteenFinancialYear.FinancialYear <= endYear &&
                                project.TwentySixteenDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentySixteenFinancialYear,
                                        Amount = project.TwentySixteenDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Actual,
                                        ExchangeRate = project.ExchangeRate,
                                    });
                                    unitWork.Save();
                                }

                                if (twentySeventeenFinancialYear.FinancialYear >= startYear && twentySeventeenFinancialYear.FinancialYear <= endYear &&
                                project.TwentySeventeenDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentySeventeenFinancialYear,
                                        Amount = project.TwentySeventeenDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Actual,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    unitWork.Save();
                                }

                                if (twentyEighteenFinancialYear.FinancialYear >= startYear && twentyEighteenFinancialYear.FinancialYear <= endYear &&
                                project.TwentyEighteenDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyEighteenFinancialYear,
                                        Amount = project.TwentyEighteenDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Actual,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    unitWork.Save();
                                }

                                if (twentyNineteenFinancialYear.FinancialYear >= startYear && twentyNineteenFinancialYear.FinancialYear <= endYear &&
                                project.TwentyNineteenDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyNineteenFinancialYear,
                                        Amount = project.TwentyNineteenDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Actual,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    unitWork.Save();
                                }

                                if (twentyTwentyFinancialYear.FinancialYear >= startYear && twentyTwentyFinancialYear.FinancialYear <= endYear &&
                                project.TwentyTwentyDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyTwentyFinancialYear,
                                        Amount = project.TwentyTwentyDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Planned,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    await unitWork.SaveAsync();
                                }

                                if (twentyOneFinancialYear.FinancialYear >= startYear && twentyOneFinancialYear.FinancialYear <= endYear &&
                                project.TwentyOneDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyOneFinancialYear,
                                        Amount = project.TwentyOneDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Planned,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    await unitWork.SaveAsync();
                                }

                                if (twentyTwoFinancialYear.FinancialYear >= startYear && twentyTwoFinancialYear.FinancialYear <= endYear &&
                                project.TwentyTwoDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyTwoFinancialYear,
                                        Amount = project.TwentyTwoDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Planned,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    await unitWork.SaveAsync();
                                }

                                if (twentyThreeFinancialYear.FinancialYear >= startYear && twentyThreeFinancialYear.FinancialYear <= endYear &&
                                project.TwentyThreeDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyThreeFinancialYear,
                                        Amount = project.TwentyThreeDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Planned,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    await unitWork.SaveAsync();
                                }

                                if (twentyFourFinancialYear.FinancialYear >= startYear && twentyFourFinancialYear.FinancialYear <= endYear &&
                                project.TwentyFourDisbursements > 0)
                                {
                                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        Year = twentyFourFinancialYear,
                                        Amount = project.TwentyFourDisbursements,
                                        Currency = project.Currency,
                                        DisbursementType = DisbursementTypes.Planned,
                                        ExchangeRate = project.ExchangeRate
                                    });
                                    await unitWork.SaveAsync();
                                }
                            }

                            var invalidFyear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == 1);
                            if (invalidFyear != null)
                            {
                                unitWork.FinancialYearRepository.Delete(invalidFyear);
                                unitWork.Save();
                            }
                            transaction.Commit();
                        }
                    });
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public async Task<ActionResponse> AddProjectLocation(ProjectLocationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                    }
                    var locationIds = (from l in model.ProjectLocations
                                       select l.LocationId).ToList<int>();
                    var locations = unitWork.LocationRepository.GetManyQueryable(l => locationIds.Contains(l.Id));
                    if (locations.Count() < locationIds.Count)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Location/s");
                        response.Success = false;
                    }


                    var projectLocations = unitWork.ProjectLocationsRepository.GetManyQueryable(s => (s.ProjectId.Equals(model.ProjectId)));
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            int newLocations = 0;
                            foreach (var location in model.ProjectLocations)
                            {
                                var isProjectLocationExists = (from l in projectLocations
                                                               where l.LocationId == location.LocationId && l.ProjectId == model.ProjectId
                                                               select l).FirstOrDefault();
                                if (isProjectLocationExists != null)
                                {
                                    isProjectLocationExists.FundsPercentage += location.FundsPercentage;
                                    unitWork.ProjectLocationsRepository.Update(isProjectLocationExists);
                                    await unitWork.SaveAsync();
                                }
                                else
                                {
                                    unitWork.ProjectLocationsRepository.Insert(new EFProjectLocations()
                                    {
                                        ProjectId = model.ProjectId,
                                        LocationId = location.LocationId,
                                        FundsPercentage = location.FundsPercentage
                                    });
                                    ++newLocations;
                                }
                            }

                            if (newLocations > 0)
                            {
                                await unitWork.SaveAsync();
                            }
                            project.DateUpdated = DateTime.Now;
                            unitWork.ProjectRepository.Update(project);
                            await unitWork.SaveAsync();

                            var updatedLocations = unitWork.ProjectLocationsRepository.GetWithInclude(p => p.ProjectId == model.ProjectId, new string[] { "Location" });
                            var unattributedLocation = (from loc in updatedLocations
                                                        where loc.Location.Location.Equals("Unattributed", StringComparison.OrdinalIgnoreCase)
                                                        select loc).FirstOrDefault();

                            int fundsPercentage = (int)(from loc in updatedLocations
                                                        select loc.FundsPercentage).Sum();

                            if (fundsPercentage < 100)
                            {
                                int leftPercentage = (100 - fundsPercentage);
                                if (unattributedLocation != null)
                                {
                                    unattributedLocation.FundsPercentage += leftPercentage;
                                    unitWork.ProjectLocationsRepository.Update(unattributedLocation);
                                    unitWork.Save();
                                }
                                else
                                {
                                    var location = unitWork.LocationRepository.GetOne(l => l.Location.Equals("Unattributed", StringComparison.OrdinalIgnoreCase));
                                    if (location == null)
                                    {
                                        location = unitWork.LocationRepository.Insert(new EFLocation()
                                        {
                                            Location = "Unattributed"
                                        });
                                        unitWork.Save();
                                    }

                                    unitWork.ProjectLocationsRepository.Insert(new EFProjectLocations()
                                    {
                                        Project = project,
                                        Location = location,
                                        FundsPercentage = leftPercentage
                                    });
                                    unitWork.Save();
                                }
                            }

                            if (fundsPercentage > 100)
                            {
                                mHelper = new MessageHelper();
                                response.Success = false;
                                response.Message = "Total percentage cannot be greater than 100";
                            }
                            else
                            {
                                transaction.Commit();
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> AddProjectSector(ProjectSectorModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    List<int> sectorIds = new List<int>();
                    List<int> mappingIds = new List<int>();
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                    }

                    bool isError = false;
                    foreach (var sector in model.ProjectSectors)
                    {
                        if (sector.SectorId == 0 && sector.MappingId == 0)
                        {
                            isError = true;
                            break;
                        }
                    }

                    if (isError)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetInvalidAttempt("Sector Id");
                        response.Success = false;
                        return response;
                    }

                    if (model.ProjectSectors.Any())
                    {
                        mappingIds = (from s in model.ProjectSectors
                                      select s.MappingId).ToList<int>();
                        sectorIds = (from s in model.ProjectSectors
                                     select s.SectorId).ToList<int>();
                    }
                    var allSectors = unitWork.SectorRepository.GetWithInclude(s => sectorIds.Contains(s.Id) || mappingIds.Contains(s.Id), new string[] { "SectorType" });
                    var sectors = (from s in allSectors
                                   where sectorIds.Contains(s.Id)
                                   select s);
                    var mappingSectors = (from s in allSectors
                                          where mappingIds.Contains(s.Id) && s.SectorType.IsPrimary == true
                                          select s);
                    int sectorIdsCount = (from id in sectorIds
                                          where id != 0
                                          select id).Count();

                    if (sectors.Count() < sectorIdsCount)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Sector/s");
                        response.Success = false;
                    }

                    EFSectorTypes defaultSectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsPrimary == true);
                    if (defaultSectorType == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetDefaultSectorTypeMissingMessage();
                        return response;
                    }

                    var projectSectors = unitWork.ProjectSectorsRepository.GetManyQueryable(s => (s.ProjectId.Equals(model.ProjectId)));
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            int newSectors = 0;
                            foreach (var sector in model.ProjectSectors)
                            {
                                EFSectorTypes sectorType = null;
                                EFSector newSector = null;
                                if (sector.SectorId == 0)
                                {
                                    if (sector.SectorTypeId == 0)
                                    {
                                        if (sectorType == null)
                                        {
                                            sectorType = unitWork.SectorTypesRepository.Insert(new EFSectorTypes()
                                            {
                                                TypeName = "Other",
                                                IsPrimary = false,
                                                IsSourceType = false,
                                            });
                                            unitWork.Save();
                                        }
                                    }
                                    else
                                    {
                                        sectorType = unitWork.SectorTypesRepository.GetOne(s => s.Id == sector.SectorTypeId);
                                    }

                                    newSector = unitWork.SectorRepository.GetOne(s => s.SectorName.Equals(sector.Sector, StringComparison.OrdinalIgnoreCase));
                                    if (newSector == null)
                                    {
                                        newSector = unitWork.SectorRepository.Insert(new EFSector()
                                        {
                                            SectorName = sector.Sector,
                                            SectorType = sectorType,
                                            ParentSector = null,
                                            TimeStamp = DateTime.Now
                                        });
                                    }
                                    model.NewMappings.Add(new SectorMappings()
                                    {
                                        SectorTypeId = sectorType.Id,
                                        SectorId = newSector.Id,
                                        MappingId = sector.MappingId
                                    });
                                }
                                var isProjectSectorExists = (from s in projectSectors
                                                             where s.SectorId == sector.MappingId && s.ProjectId == model.ProjectId
                                                             select s).FirstOrDefault();
                                if (isProjectSectorExists != null)
                                {
                                    isProjectSectorExists.FundsPercentage += sector.FundsPercentage;
                                    unitWork.ProjectSectorsRepository.Update(isProjectSectorExists);
                                    await unitWork.SaveAsync();
                                }
                                else
                                {
                                    unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                                    {
                                        ProjectId = model.ProjectId,
                                        SectorId = sector.MappingId,
                                        FundsPercentage = sector.FundsPercentage
                                    });
                                    ++newSectors;
                                }
                            }

                            if (newSectors > 0)
                            {
                                await unitWork.SaveAsync();
                            }

                            int defaultSectorId = 0;
                            if (model.NewMappings.Count() > 0)
                            {
                                defaultSectorId = defaultSectorType.Id;
                                var newFoundMappings = (from m in model.NewMappings
                                                        where m.SectorTypeId != defaultSectorId
                                                        select m);

                                if (newFoundMappings.Count() > 0)
                                {
                                    var sectorMappings = unitWork.SectorMappingsRepository.GetManyQueryable(s => sectorIds.Contains(s.SectorId));
                                    int newMappings = 0;
                                    foreach (var mapping in newFoundMappings)
                                    {
                                        if (mapping.SectorId == mapping.MappingId)
                                        {
                                            continue;
                                        }

                                        var isMappingExists = (from m in sectorMappings
                                                               where m.SectorId == mapping.SectorId && m.MappedSectorId == mapping.MappingId
                                                               select m).FirstOrDefault();
                                        if (isMappingExists == null)
                                        {
                                            if (mapping.SectorTypeId != 0)
                                            {
                                                unitWork.SectorMappingsRepository.Insert(new EFSectorMappings()
                                                {
                                                    SectorId = mapping.SectorId,
                                                    MappedSectorId = mapping.MappingId,
                                                    SectorTypeId = mapping.SectorTypeId
                                                });
                                                ++newMappings;
                                            }
                                        }
                                    }
                                    if (newMappings > 0)
                                    {
                                        await unitWork.SaveAsync();
                                    }
                                }
                            }

                            var sectorsPercentage = unitWork.ProjectSectorsRepository.GetProjection(s => s.ProjectId == model.ProjectId, s => s.FundsPercentage).Sum();
                            decimal leftPercentage = (100 - sectorsPercentage);
                            if (leftPercentage > 0)
                            {
                                var notAvailableSector = unitWork.SectorRepository.GetOne(s => s.SectorTypeId == defaultSectorType.Id && s.SectorName.Equals(UNATTRIBUTED, StringComparison.OrdinalIgnoreCase));
                                if (notAvailableSector == null)
                                {
                                    notAvailableSector = unitWork.SectorRepository.Insert(new EFSector()
                                    {
                                        SectorType = defaultSectorType,
                                        SectorName = UNATTRIBUTED,
                                        ParentSector = null,
                                        IsUnAttributed = true
                                    });
                                    unitWork.Save();

                                    unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                                    {
                                        Project = project,
                                        Sector = notAvailableSector,
                                        FundsPercentage = leftPercentage
                                    });
                                    unitWork.Save();
                                }
                                else
                                {
                                    var sectorExists = unitWork.ProjectSectorsRepository.GetOne(s => s.SectorId == notAvailableSector.Id);
                                    if (sectorExists == null)
                                    {
                                        unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                                        {
                                            Project = project,
                                            Sector = notAvailableSector,
                                            FundsPercentage = leftPercentage
                                        });
                                        unitWork.Save();
                                    }
                                    else
                                    {
                                        sectorExists.FundsPercentage = leftPercentage;
                                        unitWork.ProjectSectorsRepository.Update(sectorExists);
                                        unitWork.Save();
                                    }
                                }
                            }
                            project.DateUpdated = DateTime.Now;
                            unitWork.ProjectRepository.Update(project);
                            await unitWork.SaveAsync();
                            transaction.Commit();
                        }
                    });

                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse AddProjectFunderFromSource(ProjectFunderSourceModel model, int userOrganizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => o.Id != 0);
                if (model.Funders.Count() > 0)
                {
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Project");
                        return response;
                    }

                    try
                    {
                        List<EFProjectFunders> newFundersList = new List<EFProjectFunders>();
                        foreach (string funder in model.Funders)
                        {
                            var defaultOrganizationType = unitWork.OrganizationTypesRepository.GetOne(o => o.TypeName == "Default");
                            if (defaultOrganizationType == null)
                            {
                                defaultOrganizationType = unitWork.OrganizationTypesRepository.Insert(new EFOrganizationTypes() { TypeName = "Default" });
                                unitWork.Save();
                            }
                            var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => f.ProjectId == project.Id);
                            var isOrganizationInDB = (from org in organizations
                                                      where org.OrganizationName.Equals(funder, StringComparison.OrdinalIgnoreCase)
                                                      select org).FirstOrDefault();

                            if (isOrganizationInDB == null)
                            {
                                isOrganizationInDB = unitWork.OrganizationRepository.Insert(new EFOrganization()
                                {
                                    OrganizationType = defaultOrganizationType,
                                    OrganizationName = funder,
                                });
                                unitWork.Save();
                            }

                            var isFunderInDB = (from f in projectFunders
                                                where f.ProjectId == project.Id && f.FunderId == isOrganizationInDB.Id
                                                select f).FirstOrDefault();
                            var isFunderInList = (from f in newFundersList
                                                  where f.ProjectId == project.Id && f.FunderId == isOrganizationInDB.Id
                                                  select f).FirstOrDefault();

                            if (isFunderInDB == null && isFunderInList == null)
                            {
                                newFundersList.Add(new EFProjectFunders() { ProjectId = project.Id, FunderId = isOrganizationInDB.Id });
                            }
                        }

                        if (newFundersList.Count > 0)
                        {
                            unitWork.ProjectFundersRepository.InsertMultiple(newFundersList);
                            unitWork.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message;
                    }
                }
                return response;
            }
        }

        public ActionResponse AddProjectImplementerFromSource(ProjectImplementerSourceModel model, int userOrganizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => o.Id != 0);
                if (model.Implementers.Count() > 0)
                {
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Project");
                        return response;
                    }

                    try
                    {
                        List<EFProjectImplementers> newImplementersList = new List<EFProjectImplementers>();
                        foreach (string funder in model.Implementers)
                        {
                            var defaultOrganizationType = unitWork.OrganizationTypesRepository.GetOne(o => o.TypeName == "Default");
                            if (defaultOrganizationType == null)
                            {
                                defaultOrganizationType = unitWork.OrganizationTypesRepository.Insert(new EFOrganizationTypes() { TypeName = "Default" });
                                unitWork.Save();
                            }
                            var projectImplementers = unitWork.ProjectImplementersRepository.GetManyQueryable(f => f.ProjectId == project.Id);
                            var isOrganizationInDB = (from org in organizations
                                                      where org.OrganizationName.Equals(funder, StringComparison.OrdinalIgnoreCase)
                                                      select org).FirstOrDefault();

                            if (isOrganizationInDB == null)
                            {
                                isOrganizationInDB = unitWork.OrganizationRepository.Insert(new EFOrganization()
                                {
                                    OrganizationType = defaultOrganizationType,
                                    OrganizationName = funder,
                                });
                                unitWork.Save();
                            }

                            var isImplementerInDB = (from i in projectImplementers
                                                     where i.ProjectId == project.Id && i.ImplementerId == isOrganizationInDB.Id
                                                     select i).FirstOrDefault();
                            var isImplementerInList = (from i in newImplementersList
                                                       where i.ProjectId == project.Id && i.ImplementerId == isOrganizationInDB.Id
                                                       select i).FirstOrDefault();

                            if (isImplementerInDB == null && isImplementerInList == null)
                            {
                                newImplementersList.Add(new EFProjectImplementers() { ProjectId = project.Id, ImplementerId = isOrganizationInDB.Id });
                            }
                        }

                        if (newImplementersList.Count > 0)
                        {
                            unitWork.ProjectImplementersRepository.InsertMultiple(newImplementersList);
                            unitWork.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message;
                    }
                }
                return response;
            }
        }

        public ActionResponse AddProjectFunder(ProjectFunderModel model, int userOrganizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    List<int> updatedFunderIds = new List<int>();
                    mHelper = new MessageHelper();
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                        return response;
                    }
                    var funders = unitWork.OrganizationRepository.GetManyQueryable(o => model.FunderIds.Contains(o.Id));
                    if (funders.Count() < model.FunderIds.Count())
                    {
                        response.Message = mHelper.GetNotFound("Funder/s");
                        response.Success = false;
                        return response;
                    }

                    var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => f.ProjectId == model.ProjectId);
                    List<EFProjectFunders> newFunders = new List<EFProjectFunders>();
                    var previousFunderIds = (from f in projectFunders
                                             select f.FunderId).ToList();
                    var fundersToDelete = (from f in projectFunders
                                           where !model.FunderIds.Contains(f.FunderId)
                                           select f);
                    var alreadyFunderIds = (from f in fundersToDelete
                                            select f.FunderId).ToList<int>();
                    updatedFunderIds = (model.FunderIds.Except(alreadyFunderIds).ToList<int>());
                    if (fundersToDelete.Any())
                    {
                        foreach (var delFunder in fundersToDelete)
                        {
                            unitWork.ProjectFundersRepository.Delete(delFunder);
                        }
                        unitWork.Save();
                    }

                    foreach (var funder in funders)
                    {
                        var isFunderInDB = (from f in projectFunders
                                            where f.ProjectId == model.ProjectId && f.FunderId == funder.Id
                                            select f).FirstOrDefault();
                        var isFunderAdded = (from f in newFunders
                                             where f.ProjectId == model.ProjectId && f.FunderId == funder.Id
                                             select f).FirstOrDefault();

                        if (isFunderInDB == null && isFunderAdded == null)
                        {
                            newFunders.Add(new EFProjectFunders()
                            {
                                FunderId = funder.Id,
                                ProjectId = model.ProjectId
                            });
                        }
                    }

                    if (projectFunders.Any())
                    {
                        unitWork.Save();
                    }

                    if (newFunders.Count() > 0)
                    {
                        unitWork.ProjectFundersRepository.InsertMultiple(newFunders);
                        unitWork.Save();
                    }
                    project.DateUpdated = DateTime.Now;
                    unitWork.ProjectRepository.Update(project);
                    unitWork.Save();

                    var projectFunderIds = (from f in funders
                                            where updatedFunderIds.Contains(f.Id)
                                            select f.Id).ToList<int>().Except(previousFunderIds);
                    var users = unitWork.UserRepository.GetManyQueryable(u => projectFunderIds.Contains(u.OrganizationId));
                    List<EmailAddress> emailAddresses = new List<EmailAddress>();
                    var updatedOrganizationNames = (from o in funders
                                                    where updatedFunderIds.Contains(o.Id)
                                                    select o.OrganizationName).ToList<string>();
                    foreach (var user in users)
                    {
                        emailAddresses.Add(new EmailAddress()
                        {
                            Email = user.Email
                        });
                    }

                    if (emailAddresses.Count > 0)
                    {
                        ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                        var smtpSettings = smtpService.GetPrivate();
                        SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                        if (smtpSettings != null)
                        {
                            smtpSettingsModel.Host = smtpSettings.Host;
                            smtpSettingsModel.Port = smtpSettings.Port;
                            smtpSettingsModel.Username = smtpSettings.Username;
                            smtpSettingsModel.Password = smtpSettings.Password;
                            smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                            smtpSettingsModel.SenderName = smtpSettings.SenderName;
                        }

                        string subject = "", message = "", footerMessage = "";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewProjectToOrg);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }
                        string projectUrl = model.ProjectUrl + project.Id;
                        message += mHelper.ProjectToOrganizationMessage(project.Title, string.Join(",", updatedOrganizationNames), projectUrl);
                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                        emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse AddProjectImplementer(ProjectImplementerModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    List<int> updatedImplementerIds = new List<int>();
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                    }

                    var implementers = unitWork.OrganizationRepository.GetManyQueryable(o => model.ImplementerIds.Contains(o.Id));
                    if (implementers.Count() < model.ImplementerIds.Count())
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Implementer/s");
                        response.Success = false;
                        return response;
                    }

                    var projectImplementers = unitWork.ProjectImplementersRepository.GetManyQueryable(i => i.ProjectId == model.ProjectId);
                    List<EFProjectImplementers> newImplementers = new List<EFProjectImplementers>();
                    var previousImplementerIds = (from i in projectImplementers
                                                  select i.ImplementerId).ToList();
                    var implementersToDelete = (from i in projectImplementers
                                                where !model.ImplementerIds.Contains(i.ImplementerId)
                                                select i);
                    var alreadyImplementerIds = (from i in implementersToDelete
                                                 select i.ImplementerId).ToList<int>();
                    updatedImplementerIds = (model.ImplementerIds.Except(alreadyImplementerIds).ToList<int>());
                    if (implementersToDelete.Any())
                    {
                        foreach (var delImplementer in implementersToDelete)
                        {
                            unitWork.ProjectImplementersRepository.Delete(delImplementer);
                        }
                        unitWork.Save();
                    }

                    foreach (var implementer in implementers)
                    {
                        var isImplementerInDB = (from i in projectImplementers
                                                 where i.ProjectId == model.ProjectId && i.ImplementerId == implementer.Id
                                                 select i).FirstOrDefault();
                        var isImplementerAdded = (from i in newImplementers
                                                  where i.ProjectId == model.ProjectId && i.ImplementerId == implementer.Id
                                                  select i).FirstOrDefault();

                        if (isImplementerInDB == null && isImplementerAdded == null)
                        {
                            newImplementers.Add(new EFProjectImplementers()
                            {
                                ImplementerId = implementer.Id,
                                ProjectId = model.ProjectId
                            });
                        }
                    }

                    if (projectImplementers.Any())
                    {
                        unitWork.Save();
                    }

                    if (newImplementers.Count() > 0)
                    {
                        unitWork.ProjectImplementersRepository.InsertMultiple(newImplementers);
                        unitWork.Save();
                    }
                    project.DateUpdated = DateTime.Now;
                    unitWork.ProjectRepository.Update(project);
                    unitWork.Save();

                    var projectImplementerIds = (from i in implementers
                                                 where updatedImplementerIds.Contains(i.Id)
                                                 select i.Id).ToList<int>().Except(previousImplementerIds);
                    var users = unitWork.UserRepository.GetManyQueryable(u => projectImplementerIds.Contains(u.OrganizationId));
                    List<EmailAddress> emailAddresses = new List<EmailAddress>();
                    var updatedOrganizationNames = (from i in implementers
                                                    where updatedImplementerIds.Contains(i.Id)
                                                    select i.OrganizationName).ToList<string>();
                    foreach (var user in users)
                    {
                        emailAddresses.Add(new EmailAddress()
                        {
                            Email = user.Email
                        });
                    }

                    if (emailAddresses.Count > 0)
                    {
                        ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                        var smtpSettings = smtpService.GetPrivate();
                        SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                        if (smtpSettings != null)
                        {
                            smtpSettingsModel.Host = smtpSettings.Host;
                            smtpSettingsModel.Port = smtpSettings.Port;
                            smtpSettingsModel.Username = smtpSettings.Username;
                            smtpSettingsModel.Password = smtpSettings.Password;
                            smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                            smtpSettingsModel.SenderName = smtpSettings.SenderName;
                        }

                        string subject = "", message = "", footerMessage = "";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewProjectToOrg);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }

                        string projectUrl = model.ProjectUrl + project.Id;
                        mHelper = new MessageHelper();
                        message += mHelper.ProjectToOrganizationMessage(project.Title, string.Join(",", updatedOrganizationNames), projectUrl);
                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                        emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public async Task<ActionResponse> AddProjectDisbursement(ProjectDisbursementModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    int currentYear = DateTime.Now.Year;
                    var project = unitWork.ProjectRepository.GetWithInclude(p => p.Id == model.ProjectId, new string[] { "StartingFinancialYear", "EndingFinancialYear" }).FirstOrDefault();
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                        return response;
                    }

                    int startingYear = (from y in model.YearlyDisbursements
                                        select y.Year).Min();
                    int endingYear = (from y in model.YearlyDisbursements
                                      select y.Year).Max();

                    if (startingYear < project.StartingFinancialYear.FinancialYear)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetInvalidStartingFinancialYearMessage();
                        response.Success = false;
                        return response;
                    }

                    if (endingYear < project.EndingFinancialYear.FinancialYear)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetInvalidEndingFinancialYearMessage();
                        response.Success = false;
                        return response;
                    }

                    decimal projectValue = (project.ProjectValue);
                    decimal totalDisbursements = (from d in model.YearlyDisbursements
                                                  select (d.Amount)).Sum();

                    if (totalDisbursements > projectValue)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.InvalidDisbursement();
                        response.Success = false;
                        return response;
                    }

                    var disbursementCurrency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.Currency);
                    if (disbursementCurrency == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Currency");
                        response.Success = false;
                        return response;
                    }

                    var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => f.FinancialYear >= startingYear).ToList();
                    var disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == model.ProjectId, new string[] { "Year" });
                    List<EFProjectDisbursements> newDisbursementsList = new List<EFProjectDisbursements>();
                    List<EFFinancialYears> newFinancialYears = new List<EFFinancialYears>();

                    for (var year = startingYear; year <= endingYear; year++)
                    {
                        var isYearExists = (from yr in financialYears
                                            where yr.FinancialYear == year
                                            select yr).FirstOrDefault();

                        if (isYearExists == null)
                        {
                            newFinancialYears.Add(new EFFinancialYears() { FinancialYear = year });
                            unitWork.Save();
                        }
                    }

                    if (newFinancialYears.Count > 0)
                    {
                        unitWork.Save();
                        foreach (var newYear in newFinancialYears)
                        {
                            financialYears.Add(newYear);
                        }
                    }

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            EFProjectDisbursements isDisbursementInDB = null;
                            foreach (var disbursement in model.YearlyDisbursements)
                            {
                                isDisbursementInDB = null;
                                if (disbursements.Any())
                                {
                                    isDisbursementInDB = (from d in disbursements
                                                          where d.Year.FinancialYear == disbursement.Year && d.DisbursementType == disbursement.DisbursementType
                                                          select d).FirstOrDefault();
                                }

                                var isDisbursementInList = (from d in newDisbursementsList
                                                            where d.Year.FinancialYear == disbursement.Year && d.DisbursementType == disbursement.DisbursementType
                                                            select d).FirstOrDefault();

                                var financialYear = (from fy in financialYears
                                                     where fy.FinancialYear == disbursement.Year
                                                     select fy).FirstOrDefault();

                                if (isDisbursementInDB != null)
                                {
                                    isDisbursementInDB.Amount = disbursement.Amount;
                                    isDisbursementInDB.Currency = project.ProjectCurrency;
                                    isDisbursementInDB.ExchangeRate = project.ExchangeRate;
                                    unitWork.ProjectDisbursementsRepository.Update(isDisbursementInDB);
                                    await unitWork.SaveAsync();
                                }
                                else
                                {
                                    if (isDisbursementInList == null)
                                    {
                                        var newDisbursement = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = financialYear,
                                            Amount = disbursement.Amount,
                                            Currency = project.ProjectCurrency,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = disbursement.DisbursementType
                                        });
                                        newDisbursementsList.Add(newDisbursement);
                                    }
                                }
                            }

                            if (newDisbursementsList.Count > 0)
                            {
                                await unitWork.SaveAsync();
                            }
                            transaction.Commit();
                        }
                    });
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse AddProjectDocument(ProjectDocumentModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                    }

                    var documents = unitWork.ProjectDocumentRepository.GetManyQueryable(d => d.ProjectId == model.ProjectId);
                    var documentNames = (from d in documents
                                         select d.DocumentTitle).ToList<string>();
                    var documentUrls = (from d in documents
                                        select d.DocumentUrl).ToList<string>();

                    List<EFProjectDocuments> newDocuments = new List<EFProjectDocuments>();
                    int documentsUpdated = 0;
                    foreach (var document in model.Documents)
                    {
                        var isDocumentNameExists = (from d in documents
                                                    where d.DocumentTitle.Equals(document.DocumentTitle, StringComparison.OrdinalIgnoreCase)
                                                    select d).FirstOrDefault();

                        if (isDocumentNameExists != null)
                        {
                            isDocumentNameExists.DocumentUrl = document.DocumentUrl;
                            unitWork.ProjectDocumentRepository.Update(isDocumentNameExists);
                            ++documentsUpdated;
                        }
                        else
                        {
                            if (!documentNames.Contains(document.DocumentTitle, StringComparer.OrdinalIgnoreCase))
                            {
                                unitWork.ProjectDocumentRepository.Insert(new EFProjectDocuments()
                                {
                                    Project = project,
                                    DocumentTitle = document.DocumentTitle,
                                    DocumentUrl = document.DocumentUrl
                                });
                            }
                        }

                    }

                    if (newDocuments.Count > 0 || documentsUpdated > 0)
                    {
                        unitWork.Save();
                    }

                    project.DateUpdated = DateTime.Now;
                    unitWork.ProjectRepository.Update(project);
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse AddUpdateProjectMarker(ProjectMarkerModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var customField = unitWork.ProjectMarkersRepository.GetOne(p => p.ProjectId == model.ProjectId && p.MarkerId == model.MarkerId);
                    if (customField != null)
                    {
                        customField.Values = model.Values;
                    }
                    else
                    {
                        unitWork.ProjectMarkersRepository.Insert(new EFProjectMarkers()
                        {
                            ProjectId = model.ProjectId,
                            MarkerId = model.MarkerId,
                            FieldType = model.FieldType,
                            Values = model.Values
                        });
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                unitWork.Save();
                return response;
            }
        }

        public async Task<ActionResponse> UpdateAsync(int id, ProjectModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var project = unitWork.ProjectRepository.GetByID(id);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project");
                    return response;
                }

                var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => (f.FinancialYear == model.StartingFinancialYear || f.FinancialYear == model.EndingFinancialYear));
                var currency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.ProjectCurrency);
                if (currency == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Currency");
                    return response;
                }

                bool isCurrencyUpdated = false;
                if (project.ProjectCurrency != model.ProjectCurrency)
                {
                    isCurrencyUpdated = true;
                    var manualExcahngeRates = unitWork.ManualRatesRepository.GetManyQueryable(y => y.Year == model.StartingFinancialYear || y.Year == model.EndingFinancialYear || y.Year == DateTime.Now.Year);
                    if (model.ExchangeRate <= 0)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Exchange rate for project financial years");
                        return response;
                    }
                }

                int fyMonth = 1, fyDay = 1, startingMonth = model.StartDate.Month, startDay = model.StartDate.Day,
                        endingMonth = model.EndDate.Month, endingDay = model.EndDate.Day;
                var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
                if (fySettings != null)
                {
                    fyMonth = fySettings.Month;
                    fyDay = fySettings.Day;
                }

                if (fyMonth > 1)
                {
                    if (startingMonth < fyMonth)
                    {
                        model.StartingFinancialYear = (model.StartingFinancialYear - 1);
                    }
                    else if (startingMonth == fyMonth && fyDay < startDay)
                    {
                        model.StartingFinancialYear = (model.StartingFinancialYear - 1);
                    }

                    if (endingMonth < fyMonth)
                    {
                        model.EndingFinancialYear = (model.EndingFinancialYear - 1);
                    }
                    else if (endingMonth == fyMonth && endingDay < fyDay)
                    {
                        model.EndingFinancialYear = (model.EndingFinancialYear - 1);
                    }
                }

                if (model.StartDate > project.EndDate)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.InvalidProjectStartDate();
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var startingFinancialYear = (from fy in financialYears
                                                         where fy.FinancialYear == model.StartingFinancialYear
                                                         select fy).FirstOrDefault();
                            if (startingFinancialYear == null)
                            {
                                startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = model.StartingFinancialYear
                                });
                                await unitWork.SaveAsync();
                            }

                            var endingFinancialYear = (from fy in financialYears
                                                       where fy.FinancialYear == model.EndingFinancialYear
                                                       select fy).FirstOrDefault();
                            if (endingFinancialYear == null)
                            {
                                endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = model.EndingFinancialYear
                                });
                                await unitWork.SaveAsync();
                            }

                            project.Title = model.Title;
                            project.Description = model.Description;
                            project.StartingFinancialYear = startingFinancialYear;
                            project.StartDate = model.StartDate;
                            project.EndDate = model.EndDate;
                            project.EndingFinancialYear = endingFinancialYear;
                            project.ProjectValue = model.ProjectValue;
                            project.ExchangeRate = model.ExchangeRate;
                            project.ProjectCurrency = model.ProjectCurrency;
                            project.DateUpdated = DateTime.Now;
                            unitWork.ProjectRepository.Update(project);
                            await unitWork.SaveAsync();

                            var deleteDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == id &&
                                d.Year.FinancialYear < model.StartingFinancialYear || d.Year.FinancialYear > model.EndingFinancialYear);
                            foreach (var disbursement in deleteDisbursements)
                            {
                                unitWork.ProjectDisbursementsRepository.Delete(disbursement);
                            }
                            if (deleteDisbursements.Any())
                            {
                                await unitWork.SaveAsync();
                                decimal totalDisbursements = unitWork.ProjectDisbursementsRepository.GetProjection(d => d.ProjectId == project.Id, d => d.Amount).Sum();
                                if (totalDisbursements != project.ProjectValue)
                                {
                                    project.ProjectValue = totalDisbursements;
                                    unitWork.ProjectRepository.Update(project);
                                    await unitWork.SaveAsync();
                                }
                            }
                            if (isCurrencyUpdated)
                            {
                                var disbursements = unitWork.ProjectDisbursementsRepository.GetManyQueryable(d => d.Id != 0);
                                foreach(var disbursement in disbursements)
                                {
                                    disbursement.Currency = project.ProjectCurrency;
                                    disbursement.ExchangeRate = project.ExchangeRate;
                                    unitWork.ProjectDisbursementsRepository.Update(disbursement);
                                }
                                await unitWork.SaveAsync();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            response.Message = ex.Message;
                            response.Success = false;
                        }
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> DeleteProjectLocationAsync(int projectId, int locationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var location = unitWork.LocationRepository.GetByID(locationId);
                if (location == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Location");
                    response.Success = false;
                    return response;
                }

                var projectLocation = unitWork.ProjectLocationsRepository.Get(p => p.ProjectId == projectId && p.LocationId == locationId);
                if (projectLocation == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Location");
                    response.Success = false;
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        unitWork.ProjectLocationsRepository.Delete(projectLocation);
                        unitWork.Save();

                        if (!location.Location.Equals("Unattributed", StringComparison.OrdinalIgnoreCase))
                        {
                            decimal fundsPercentage = unitWork.ProjectLocationsRepository.GetProjection(p => p.ProjectId == projectId, p => p.FundsPercentage).Sum();
                            if (fundsPercentage < 100)
                            {
                                decimal leftPercentage = (100 - fundsPercentage);
                                var unAttributedLocation = unitWork.LocationRepository.GetOne(l => l.Location.Equals("Unattributed", StringComparison.OrdinalIgnoreCase));
                                if (unAttributedLocation == null)
                                {
                                    unAttributedLocation = unitWork.LocationRepository.Insert(new EFLocation()
                                    {
                                        Location = "Unattributed",
                                        Latitude = 0,
                                        Longitude = 0
                                    });
                                    unitWork.Save();
                                }

                                unitWork.ProjectLocationsRepository.Insert(new EFProjectLocations()
                                {
                                    ProjectId = projectId,
                                    Location = unAttributedLocation,
                                    FundsPercentage = leftPercentage
                                });
                                await unitWork.SaveAsync();
                            }
                        }
                        transaction.Commit();
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> DeleteProjectSectorAsync(int projectId, int sectorId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                var sector = unitWork.SectorRepository.GetOne(s => s.Id == sectorId);
                if (sector == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Sector");
                    response.Success = false;
                    return response;
                }

                var projectSector = unitWork.ProjectSectorsRepository.GetOne(p => p.ProjectId == projectId && p.SectorId == sectorId);
                if (projectSector == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Sector");
                    response.Success = false;
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        unitWork.ProjectSectorsRepository.Delete(projectSector);
                        await unitWork.SaveAsync();

                        if (!sector.SectorName.Equals(UNATTRIBUTED, StringComparison.OrdinalIgnoreCase))
                        {
                            var fundsPercentage = unitWork.ProjectSectorsRepository.GetProjection(s => s.ProjectId == projectId, s => s.FundsPercentage).Sum();
                            decimal leftPercentage = (100 - fundsPercentage);
                            if (leftPercentage > 0)
                            {
                                var defaultSectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsPrimary == true);
                                if (defaultSectorType != null)
                                {
                                    var notAvailableSector = unitWork.SectorRepository.GetOne(s => s.SectorTypeId == defaultSectorType.Id && s.SectorName.Equals(UNATTRIBUTED, StringComparison.OrdinalIgnoreCase));
                                    if (notAvailableSector == null)
                                    {
                                        notAvailableSector = unitWork.SectorRepository.Insert(new EFSector()
                                        {
                                            SectorType = defaultSectorType,
                                            SectorName = UNATTRIBUTED,
                                            ParentSector = null,
                                            IsUnAttributed = true
                                        });
                                        unitWork.Save();

                                        unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                                        {
                                            ProjectId = projectId,
                                            Sector = notAvailableSector,
                                            FundsPercentage = leftPercentage
                                        });
                                        unitWork.Save();
                                    }
                                    else
                                    {
                                        var sectorExists = unitWork.ProjectSectorsRepository.GetOne(s => s.SectorId == notAvailableSector.Id);
                                        if (sectorExists == null)
                                        {
                                            unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                                            {
                                                ProjectId = projectId,
                                                Sector = notAvailableSector,
                                                FundsPercentage = leftPercentage
                                            });
                                            unitWork.Save();
                                        }
                                        else
                                        {
                                            sectorExists.FundsPercentage = leftPercentage;
                                            unitWork.ProjectSectorsRepository.Update(sectorExists);
                                            unitWork.Save();
                                        }
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse DeleteProjectFunder(int projectId, int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectFunder = unitWork.ProjectFundersRepository.Get(f => f.ProjectId == projectId && f.FunderId == funderId);
                IMessageHelper mHelper;
                if (projectFunder == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Funder");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectFundersRepository.Delete(projectFunder);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse DeleteProjectImplementer(int projectId, int implementerId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectImplementer = unitWork.ProjectImplementersRepository.Get(i => i.ProjectId == projectId && i.ImplementerId == implementerId);
                IMessageHelper mHelper;
                if (projectImplementer == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Implementer");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectImplementersRepository.Delete(projectImplementer);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse DeleteProjectMarker(int projectId, int customFieldId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectMarker = unitWork.ProjectMarkersRepository.Get(f => f.ProjectId == projectId && f.MarkerId == customFieldId);
                IMessageHelper mHelper;
                if (projectMarker == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Custom Field");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectMarkersRepository.Delete(projectMarker);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse DeleteProjectDisbursement(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectDisbursement = unitWork.ProjectDisbursementsRepository.GetByID(id);
                    IMessageHelper mHelper;
                    if (projectDisbursement == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project Disbursement");
                        response.Success = false;
                        return response;
                    }

                    unitWork.ProjectDisbursementsRepository.Delete(projectDisbursement);
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse DeleteProjectDocument(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectDocument = unitWork.ProjectDocumentRepository.GetByID(id);
                IMessageHelper mHelper;
                if (projectDocument == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Document");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectDocumentRepository.Delete(projectDocument);
                unitWork.Save();
                return response;
            }
        }

        public async Task<ActionResponse> MergeProjectsAsync(MergeProjectsModel model, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                var createdBy = unitWork.UserRepository.GetByID(userId);
                if (createdBy == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetUnAuthorizedAccessMessage();
                    return response;
                }

                if (model.ProjectIds.Count < 2)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.InvalidProjectMerge();
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => (f.FinancialYear >= model.StartingFinancialYear && f.FinancialYear <= model.EndingFinancialYear));
                if (financialYears.Count() < 2)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Provided Financial Years");
                    return response;
                }

                var currency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.ProjectCurrency);
                if (currency == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Provided Currency");
                    return response;
                }

                var fundingType = unitWork.FundingTypeRepository.GetOne(f => f.Id == model.FundingTypeId);
                if (fundingType == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Funding Type");
                    return response;
                }

                IQueryable<EFOrganization> organizations = null;
                IQueryable<EFSector> sectors = null;
                IQueryable<EFLocation> locations = null;
                IQueryable<EFMarkers> markers = null;

                var orgIds = (model.FunderIds.Union(model.ImplementerIds));
                if (orgIds.Count() > 0)
                {
                    organizations = unitWork.OrganizationRepository.GetManyQueryable(o => orgIds.Contains(o.Id));
                }

                if (model.Sectors.Count > 0)
                {
                    var sectorIds = (from s in model.Sectors
                                     select s.SectorId);
                    sectors = unitWork.SectorRepository.GetManyQueryable(s => sectorIds.Contains(s.Id));
                }

                if (model.Locations.Count > 0)
                {
                    var locationIds = (from l in model.Locations
                                       select l.LocationId);
                    locations = unitWork.LocationRepository.GetManyQueryable(l => locationIds.Contains(l.Id));
                }

                if (model.Markers.Count > 0)
                {
                    var markerIds = (from m in model.Markers
                                     select m.MarkerId);
                    markers = unitWork.MarkerRepository.GetManyQueryable(m => markerIds.Contains(m.Id));
                }

                var startingFinancialYear = (from fy in financialYears
                                             where fy.FinancialYear == model.StartingFinancialYear
                                             select fy).FirstOrDefault();
                var endingFinancialYear = (from fy in financialYears
                                           where fy.FinancialYear == model.EndingFinancialYear
                                           select fy).FirstOrDefault();

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var newProject = unitWork.ProjectRepository.Insert(new EFProject()
                        {
                            Title = model.Title,
                            Description = model.Description,
                            StartingFinancialYear = startingFinancialYear,
                            EndingFinancialYear = endingFinancialYear,
                            ProjectCurrency = model.ProjectCurrency,
                            ProjectValue = model.ProjectValue,
                            ExchangeRate = model.ExchangeRate,
                            FundingType = fundingType,
                            DateUpdated = DateTime.Now,
                            CreatedBy = createdBy
                        });
                        await unitWork.SaveAsync();

                        unitWork.ProjectMembershipRepository.Insert(new EFProjectMembershipRequests()
                        {
                            ProjectId = newProject.Id,
                            UserId = userId,
                            Dated = DateTime.Now,
                            IsApproved = true
                        });
                        await unitWork.SaveAsync();

                        if (model.FunderIds.Count > 0)
                        {
                            List<EFProjectFunders> newFunders = new List<EFProjectFunders>();
                            EFOrganization funder = null;
                            foreach (var funderId in model.FunderIds)
                            {
                                if (organizations != null)
                                {
                                    funder = (from org in organizations
                                              where org.Id == funderId
                                              select org).FirstOrDefault();
                                }

                                var isFunderAdded = (from f in newFunders
                                                     where f.FunderId == funderId
                                                     select f).FirstOrDefault();

                                if (funder != null && isFunderAdded == null)
                                {
                                    newFunders.Add(new EFProjectFunders()
                                    {
                                        Project = newProject,
                                        Funder = funder
                                    });
                                }
                            }

                            if (newFunders.Count > 0)
                            {
                                unitWork.ProjectFundersRepository.InsertMultiple(newFunders);
                                await unitWork.SaveAsync();
                            }
                        }

                        if (model.ImplementerIds.Count > 0)
                        {
                            List<EFProjectImplementers> newImplementers = new List<EFProjectImplementers>();
                            EFOrganization implementer = null;
                            foreach (var implementerId in model.ImplementerIds)
                            {
                                if (organizations != null)
                                {
                                    implementer = (from org in organizations
                                                   where org.Id == implementerId
                                                   select org).FirstOrDefault();
                                }
                                var isImplementerAdded = (from i in newImplementers
                                                          where i.ImplementerId == implementerId
                                                          select i).FirstOrDefault();

                                if (implementer != null && isImplementerAdded == null)
                                {
                                    newImplementers.Add(new EFProjectImplementers()
                                    {
                                        Project = newProject,
                                        Implementer = implementer
                                    });
                                }
                            }

                            if (newImplementers.Count > 0)
                            {
                                unitWork.ProjectImplementersRepository.InsertMultiple(newImplementers);
                                await unitWork.SaveAsync();
                            }
                        }

                        if (model.Sectors.Count > 0)
                        {
                            List<EFProjectSectors> newSectors = new List<EFProjectSectors>();
                            EFSector sector = null;
                            foreach (var sectorModel in model.Sectors)
                            {
                                if (sectors != null)
                                {
                                    sector = (from s in sectors
                                              where s.Id == sectorModel.SectorId
                                              select s).FirstOrDefault();
                                }

                                var isSectorAdded = (from s in newSectors
                                                     where s.SectorId == sectorModel.SectorId
                                                     select s).FirstOrDefault();

                                if (sector != null && isSectorAdded == null)
                                {
                                    newSectors.Add(new EFProjectSectors()
                                    {
                                        Project = newProject,
                                        SectorId = sector.Id,
                                        Sector = sector,
                                        FundsPercentage = sectorModel.FundsPercentage,
                                    });
                                }
                                else if (isSectorAdded != null)
                                {
                                    isSectorAdded.FundsPercentage += sectorModel.FundsPercentage;
                                }
                            }

                            if (newSectors.Count > 0)
                            {
                                unitWork.ProjectSectorsRepository.InsertMultiple(newSectors);
                                await unitWork.SaveAsync();
                            }
                        }

                        if (model.Locations.Count > 0)
                        {
                            List<EFProjectLocations> newLocations = new List<EFProjectLocations>();
                            EFLocation location = null;
                            foreach (var locationModel in model.Locations)
                            {
                                if (locations != null)
                                {
                                    location = (from l in locations
                                                where l.Id == locationModel.LocationId
                                                select l).FirstOrDefault();
                                }

                                var isLocationAdded = (from l in newLocations
                                                       where l.LocationId == locationModel.LocationId && l.ProjectId == newProject.Id
                                                       select l).FirstOrDefault();

                                if (location != null && isLocationAdded == null)
                                {
                                    newLocations.Add(new EFProjectLocations()
                                    {
                                        Project = newProject,
                                        LocationId = location.Id,
                                        Location = location,
                                        FundsPercentage = locationModel.FundsPercentage,
                                    });
                                }
                                else if (isLocationAdded != null)
                                {
                                    isLocationAdded.FundsPercentage += locationModel.FundsPercentage;
                                }
                            }

                            if (newLocations.Count > 0)
                            {
                                unitWork.ProjectLocationsRepository.InsertMultiple(newLocations);
                                await unitWork.SaveAsync();
                            }
                        }

                        if (model.Disbursements.Count > 0)
                        {
                            List<EFProjectDisbursements> newDisbursements = new List<EFProjectDisbursements>();
                            EFFinancialYears financialYear = null;
                            foreach (var disbursement in model.Disbursements)
                            {
                                financialYear = (from y in financialYears
                                                 where y.FinancialYear == disbursement.Year
                                                 select y).FirstOrDefault();

                                var isDisbursementAdded = (from d in newDisbursements
                                                           where d.Year.FinancialYear == disbursement.Year && disbursement.DisbursementType == d.DisbursementType
                                                           select d).FirstOrDefault();

                                if (financialYear != null && isDisbursementAdded == null)
                                {
                                    newDisbursements.Add(new EFProjectDisbursements()
                                    {
                                        Project = newProject,
                                        DisbursementType = disbursement.DisbursementType,
                                        Year = financialYear,
                                        Amount = disbursement.Amount,
                                        Currency = newProject.ProjectCurrency,
                                        ExchangeRate = newProject.ExchangeRate
                                    });
                                }
                            }

                            if (newDisbursements.Count > 0)
                            {
                                unitWork.ProjectDisbursementsRepository.InsertMultiple(newDisbursements);
                                await unitWork.SaveAsync();
                            }
                        }

                        if (model.Documents.Count > 0)
                        {
                            List<EFProjectDocuments> newDocuments = new List<EFProjectDocuments>();
                            foreach (var document in model.Documents)
                            {
                                var isDocumentAdded = (from d in newDocuments
                                                       where d.DocumentTitle.Equals(document.DocumentTitle, StringComparison.OrdinalIgnoreCase)
                                                       select d).FirstOrDefault();

                                if (isDocumentAdded == null)
                                {
                                    newDocuments.Add(new EFProjectDocuments()
                                    {
                                        Project = newProject,
                                        DocumentTitle = document.DocumentTitle,
                                        DocumentUrl = document.DocumentUrl
                                    });
                                }
                            }

                            if (newDocuments.Count > 0)
                            {
                                unitWork.ProjectDocumentRepository.InsertMultiple(newDocuments);
                                await unitWork.SaveAsync();
                            }
                        }

                        if (model.Markers.Count > 0)
                        {
                            List<EFProjectMarkers> newMarkersList = new List<EFProjectMarkers>();
                            EFMarkers marker = null;
                            foreach (var mark in model.Markers)
                            {
                                marker = (from m in markers
                                          where m.Id == mark.MarkerId
                                          select m).FirstOrDefault();

                                var isMarkerAdded = (from m in newMarkersList
                                                     where m.MarkerId == mark.MarkerId
                                                     select m).FirstOrDefault();
                                if (isMarkerAdded == null)
                                {
                                    newMarkersList.Add(new EFProjectMarkers()
                                    {
                                        Project = newProject,
                                        Marker = marker,
                                        Values = mark.Values
                                    });
                                }
                            }
                            if (newMarkersList.Count > 0)
                            {
                                unitWork.ProjectMarkersRepository.InsertMultiple(newMarkersList);
                                await unitWork.SaveAsync();
                            }
                        }

                        //Now delete the old projects
                        if (model.ProjectIds.Count > 0)
                        {
                            var projectsToDelete = unitWork.ProjectRepository.GetManyQueryable(p => model.ProjectIds.Contains(p.Id));
                            foreach (var project in projectsToDelete)
                            {
                                unitWork.ProjectRepository.Delete(project);
                            }
                            await unitWork.SaveAsync();
                        }
                        transaction.Commit();
                        response.ReturnedId = newProject.Id;
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
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
