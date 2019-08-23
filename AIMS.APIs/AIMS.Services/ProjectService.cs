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
        IEnumerable<ProjectView> GetAll();

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
        Task<IEnumerable<ProjectProfileView>> SearchProjectsByCriteria(SearchProjectModel model);

        /// <summary>
        /// Gets lighter version of projects for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectView>> SearchProjectsViewByCriteria(SearchProjectModel model);

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
        ActionResponse Update(int id, ProjectModel model);

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
        ActionResponse AddProjectLocation(ProjectLocationModel model);

        /// <summary>
        /// Adds sector to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectSector(ProjectSectorModel model);

        /// <summary>
        /// Adds funder to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectFunder(ProjectFunderModel model, int userOrganizationId);

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
        ActionResponse AddProjectDisbursement(ProjectDisbursementModel model);

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
        IEnumerable<ProjectDisbursementView> GetProjectDisbursements(int id);

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
        /// Deletes project location
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectLocation(int projectId, int locationId);

        /// <summary>
        /// Deletes project sector
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectSector(int projectId, int locationId);

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
        Task<ActionResponse> MergeProjectsAsync(MergeProjectsModel model);

        /// <summary>
        /// Gets count for active projects
        /// </summary>
        /// <returns></returns>
        int GetActiveProjectsCount();

        /// <summary>
        /// Gets current year disbursements
        /// </summary>
        /// <returns></returns>
        decimal GetCurrentYearDisbursements();
    }

    public class ProjectService : IProjectService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projects = unitWork.ProjectRepository.GetAll();
                projects = (from p in projects
                            orderby p.DateUpdated descending
                            select p);
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public async Task<IEnumerable<ProjectView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projects = await unitWork.ProjectRepository.GetAllAsync();
                projects = (from p in projects
                            orderby p.DateUpdated descending
                            select p);
                return await Task<IEnumerable<ProjectView>>.Run(() => mapper.Map<List<ProjectView>>(projects)).ConfigureAwait(false);
            }
        }

        public decimal GetCurrentYearDisbursements()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                Decimal disbursementValue = 0;
                var disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.Year.FinancialYear == DateTime.Now.Year, new string[] { "Year" });
                if (disbursements.Any())
                {
                    disbursementValue = (from d in disbursements
                                         select (d.Amount * d.ExchangeRate)).Sum();
                }
                return disbursementValue;
            }
        }

        public async Task<IEnumerable<ProjectAbstractView>> GetAllDetailAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectAbstractView> projectsList = new List<ProjectAbstractView>();
                var projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0, new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                foreach(var project in projects)
                {
                    IEnumerable<string> funderNames = (from f in project.Funders
                                                                    select f.Funder.OrganizationName);
                    IEnumerable<string> implementerNames = (from i in project.Implementers
                                                     select i.Implementer.OrganizationName);
                    IEnumerable<string> organizations = funderNames.Union(implementerNames);

                    List<OrganizationAbstractView> organizationsList = new List<OrganizationAbstractView>();
                    foreach(string org in organizations)
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
                        StartingFinancialYear = project.StartingFinancialYear.ToString(),
                        EndingFinancialYear = project.EndingFinancialYear.ToString(),
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
                var project = unitWork.ProjectRepository.GetByID(id);
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
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id.Equals(id), new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Disbursements.Year", "Funders", "Funders.Funder", "Funders.FundingType", "Implementers", "Implementers.Implementer", "Documents", "Markers.Marker" });
                ProjectProfileView profileView = new ProjectProfileView();

                if (projectProfileList != null)
                {
                    foreach (var project in projectProfileList)
                    {
                        var projectDisbursements = (from d in project.Disbursements
                                                    orderby d.Year.FinancialYear descending
                                                    select d);
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(projectDisbursements);
                        profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);
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

        public async Task<IEnumerable<ProjectProfileView>> SearchProjectsByCriteria(SearchProjectModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IQueryable<EFProject> projectProfileList = null;
                List<ProjectProfileView> projectsList = new List<ProjectProfileView>();

                try
                {
                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
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
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Markers.Marker" });
                    }

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
                        profileView.Markers = mapper.Map<List<ProjectMarkersView>>(project.Markers);

                        projectsList.Add(profileView);
                    }
                }
                catch (Exception)
                {
                }
                return await Task<ProjectProfileView>.Run(() => projectsList).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<ProjectView>> SearchProjectsViewByCriteria(SearchProjectModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IQueryable<EFProject> projectProfileList = null;
                List<ProjectView> projectsList = new List<ProjectView>();

                try
                {
                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase));
                    }

                    if (model.StartingYear != 0 && model.EndingYear != 0)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => (p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear));
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
                            projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => projectIds.Contains(p.Id));
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
                            projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => projectIds.Contains(p.Id));
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
                            projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => projectIds.Contains(p.Id));
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
                        projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => p.Title.Contains(model.Title));
                    }

                    foreach (var project in projectProfileList)
                    {
                        ProjectView profileView = new ProjectView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.ToString();
                        profileView.DateUpdated = project.DateUpdated.ToShortDateString();
                        projectsList.Add(profileView);
                    }
                }
                catch (Exception)
                {
                }
                return await Task<ProjectProfileView>.Run(() => projectsList).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<ProjectProfileView>> GetProjectsByIdsAsync(List<int> ids)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ids.Contains(p.Id), new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers.Marker" });
                List<ProjectProfileView> profileViewList = new List<ProjectProfileView>();

                if (projectProfileList != null)
                {
                    foreach (var project in projectProfileList)
                    {
                        profileViewList.Add(new ProjectProfileView()
                        {
                            Id = project.Id,
                            Title = project.Title,
                            Description = project.Description,
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
                return mapper.Map<List<LocationView>>(locations);
            }
        }

        public IEnumerable<ProjectSectorView> GetProjectSectors(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.ProjectSectorsRepository.GetWithInclude(s => s.ProjectId == id, new string[] { "Sector" });
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

        public IEnumerable<ProjectDisbursementView> GetProjectDisbursements(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var disbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == id, new string[] { "Year" });
                disbursements = (disbursements != null && disbursements.Count() > 0) ? 
                                    (from d in disbursements orderby d.Year.FinancialYear descending select d) : disbursements;
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
                var funderProjectsIds = unitWork.ProjectFundersRepository.GetProjection(f => f.FunderId == funderId, f => f.ProjectId).ToList();
                var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ImplementerId == funderId, i => i.ProjectId).ToList();
                var membershipProjectIds = unitWork.ProjectMembershipRepository.GetProjection(m => (m.UserId == userId && m.IsApproved == true), m => m.ProjectId);
                var combinedProjectIds = funderProjectsIds.Union(implementerProjectIds);
                combinedProjectIds = combinedProjectIds.Union(membershipProjectIds);
                List<UserProjectsView> projectIds = new List<UserProjectsView>();
                foreach(var id in combinedProjectIds)
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

                    var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => (f.FinancialYear == model.StartingFinancialYear || f.FinancialYear == model.EndingFinancialYear));
                    if (financialYears.Count() < 2)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Financial Year");
                        return response;
                    }

                    var currency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.ProjectCurrency);
                    if (currency == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Currency");
                        return response;
                    }

                    var startingFinancialYear = (from fy in financialYears
                                                 where fy.FinancialYear == model.StartingFinancialYear
                                                 select fy).FirstOrDefault();
                    var endingFinancialYears = (from fy in financialYears
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
                                EndingFinancialYear = endingFinancialYears,
                                ProjectValue = model.ProjectValue,
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

        public ActionResponse AddProjectLocation(ProjectLocationModel model)
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
                    var location = unitWork.LocationRepository.GetByID(model.LocationId);
                    if (location == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Location");
                        response.Success = false;
                    }

                    unitWork.ProjectLocationsRepository.Insert(new EFProjectLocations()
                    {
                        Project = project,
                        Location = location,
                        FundsPercentage = model.FundsPercentage,
                    });
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

        public ActionResponse AddProjectSector(ProjectSectorModel model)
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
                    var sector = unitWork.SectorRepository.GetByID(model.SectorId);
                    if (sector == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Sector");
                        response.Success = false;
                    }

                    var projectSector = unitWork.ProjectSectorsRepository.Get(s => (s.ProjectId.Equals(model.ProjectId) && (s.SectorId.Equals(model.SectorId))));

                    if (projectSector != null)
                    {
                        decimal percentage = projectSector.FundsPercentage + model.FundsPercentage;
                        if (percentage > 100)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.InvalidPercentage();
                            response.Success = false;
                            return response;
                        }

                        projectSector.FundsPercentage += model.FundsPercentage;
                        unitWork.ProjectSectorsRepository.Update(projectSector);
                    }
                    else
                    {
                        unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                        {
                            Project = project,
                            Sector = sector,
                            FundsPercentage = model.FundsPercentage,
                        });
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

        public ActionResponse AddProjectFunder(ProjectFunderModel model, int userOrganizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                try
                {
                    mHelper = new MessageHelper();
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                        return response;
                    }
                    var funder = unitWork.OrganizationRepository.GetByID(model.FunderId);
                    if (funder == null)
                    {
                        response.Message = mHelper.GetNotFound("Funder");
                        response.Success = false;
                        return response;
                    }

                    var projectFunder = unitWork.ProjectFundersRepository.GetOne(f => f.ProjectId == model.ProjectId && f.FunderId == model.FunderId);
                    if (projectFunder == null)
                    {
                        projectFunder = unitWork.ProjectFundersRepository.Insert(new EFProjectFunders()
                        {
                            Project = project,
                            Funder = funder,
                        });
                        project.DateUpdated = DateTime.Now;
                        unitWork.ProjectRepository.Update(project);
                        unitWork.Save();

                        var projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(f => f.ProjectId == model.ProjectId, f => f.FunderId);
                        var users = unitWork.UserRepository.GetManyQueryable(u => projectFunderIds.Contains(u.OrganizationId));
                        List<EmailAddress> emailAddresses = new List<EmailAddress>();

                        foreach(var user in users)
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
                            }

                            string subject = "", message = "", footerMessage = "";
                            var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewProjectToOrg);
                            if (emailMessage != null)
                            {
                                subject = emailMessage.Subject;
                                message = emailMessage.Message;
                                footerMessage = emailMessage.FooterMessage;
                            }
                            message += mHelper.ProjectToOrganizationMessage(funder.OrganizationName);
                            IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
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
        }

        public ActionResponse AddProjectImplementer(ProjectImplementerModel model)
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
                    var implementer = unitWork.OrganizationRepository.GetByID(model.ImplementerId);
                    if (implementer == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Implementer");
                        response.Success = false;
                    }

                    var isImplementerExists = unitWork.ProjectImplementersRepository.GetOne(i => i.ProjectId == model.ProjectId && i.ImplementerId == model.ImplementerId);

                    if (isImplementerExists != null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.AlreadyExists("Implementer");
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        unitWork.ProjectImplementersRepository.Insert(new EFProjectImplementers()
                        {
                            Project = project,
                            Implementer = implementer,
                        });
                        project.DateUpdated = DateTime.Now;
                        unitWork.ProjectRepository.Update(project);
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
        }

        public ActionResponse AddProjectDisbursement(ProjectDisbursementModel model)
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
                        return response;
                    }

                    var currency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.Currency);
                    if (currency == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Currency");
                        response.Success = false;
                        return response;
                    }

                    var financialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == model.Year);
                    if (financialYear == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Financial Year");
                        response.Success = false;
                        return response;
                    }

                    var funders = unitWork.ProjectFundersRepository.GetManyQueryable(f => f.ProjectId == model.ProjectId);
                    decimal totalFunds = project.ProjectValue;

                    var disbursements = unitWork.ProjectDisbursementsRepository.GetManyQueryable(d => d.ProjectId == model.ProjectId);
                    decimal totalDisbursement = ((from disbursement in disbursements
                                                  select (disbursement.Amount * (1 / disbursement.ExchangeRate))).Sum()) + (model.Amount * (1 / model.ExchangeRate));

                    if (totalDisbursement > totalFunds)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.InvalidDisbursement();
                        response.Success = false;
                        return response;
                    }

                    var newDisbursement = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                    {
                        Project = project,
                        Year = financialYear,
                        Amount = model.Amount,
                        Currency = model.Currency,
                        ExchangeRate = model.ExchangeRate
                    });
                    project.DateUpdated = DateTime.Now;
                    unitWork.ProjectRepository.Update(project);
                    unitWork.Save();
                    response.ReturnedId = newDisbursement.Id;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
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

                    unitWork.ProjectDocumentRepository.Insert(new EFProjectDocuments()
                    {
                        Project = project,
                        DocumentTitle = model.DocumentTitle,
                        DocumentUrl = model.DocumentUrl
                    });
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
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse Update(int id, ProjectModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var projectObj = unitWork.ProjectRepository.GetByID(id);
                if (projectObj == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project");
                    return response;
                }

                var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => (f.FinancialYear == model.StartingFinancialYear || f.FinancialYear == model.EndingFinancialYear));
                if (financialYears.Count() < 2)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Financial Year");
                    return response;
                }

                var currency = unitWork.CurrencyRepository.GetOne(c => c.Currency == model.ProjectCurrency);
                if (currency == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Currency");
                    return response;
                }

                var startingFinancialYear = (from fy in financialYears
                                             where fy.FinancialYear == model.StartingFinancialYear
                                             select fy).FirstOrDefault();
                var endingFinancialYears = (from fy in financialYears
                                            where fy.FinancialYear == model.EndingFinancialYear
                                            select fy).FirstOrDefault();

                projectObj.Title = model.Title;
                projectObj.Description = model.Description;
                projectObj.StartingFinancialYear = startingFinancialYear;
                projectObj.EndingFinancialYear = endingFinancialYears;
                projectObj.ProjectValue = model.ProjectValue;
                projectObj.ProjectCurrency = model.ProjectCurrency;
                projectObj.DateUpdated = DateTime.Now;

                unitWork.ProjectRepository.Update(projectObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }

        public ActionResponse DeleteProjectLocation(int projectId, int locationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectLocation = unitWork.ProjectLocationsRepository.Get(p => p.ProjectId == projectId && p.LocationId == locationId);
                IMessageHelper mHelper;
                if (projectLocation == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Location");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectLocationsRepository.Delete(projectLocation);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse DeleteProjectSector(int projectId, int sectorId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectSector = unitWork.ProjectSectorsRepository.Get(p => p.ProjectId == projectId && p.SectorId == sectorId);
                IMessageHelper mHelper;
                if (projectSector == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Sector");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectSectorsRepository.Delete(projectSector);
                unitWork.Save();
                return response;
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

        public async Task<ActionResponse> MergeProjectsAsync(MergeProjectsModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                if (model.ProjectsIds.Count < 2)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.InvalidProjectMerge();
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(f => (f.FinancialYear == model.StartingFinancialYear || f.FinancialYear == model.EndingFinancialYear));
                if (financialYears.Count() < 2)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Provided financial year");
                    return response;
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
                        //Add project
                        var newProject = unitWork.ProjectRepository.Insert(new EFProject()
                        {
                            Title = model.Title,
                            Description = model.Description,
                            StartingFinancialYear = startingFinancialYear,
                            EndingFinancialYear = endingFinancialYear
                        });
                        await unitWork.SaveAsync();

                        var projectsToBeMerged = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectsIds.Contains(p.Id),
                            new string[] { "Funders", "Implementers", "Sectors", "Locations", "Disbursements", "Disbursements.Year", "Documents" });

                        if (projectsToBeMerged != null)
                        {
                            List<EFProjectFunders> fundersList = new List<EFProjectFunders>();
                            List<EFProjectImplementers> implementersList = new List<EFProjectImplementers>();
                            List<EFProjectSectors> sectorsList = new List<EFProjectSectors>();
                            List<EFProjectLocations> locationsList = new List<EFProjectLocations>();
                            List<EFProjectDisbursements> disbursementsList = new List<EFProjectDisbursements>();
                            List<EFProjectDocuments> documentsList = new List<EFProjectDocuments>();

                            foreach (var project in projectsToBeMerged)
                            {
                                if (project.Funders.Count > 0)
                                {
                                    foreach (var funder in project.Funders)
                                    {
                                        var funderExists = (from f in fundersList
                                                            where f.ProjectId == newProject.Id && f.FunderId == funder.FunderId
                                                            select f).FirstOrDefault();

                                        if (funderExists == null)
                                        {
                                            fundersList.Add(new EFProjectFunders()
                                            {
                                                ProjectId = newProject.Id,
                                                FunderId = funder.FunderId,
                                            });
                                        }
                                    }
                                }

                                if (project.Implementers.Count > 0)
                                {
                                    foreach(var implementer in project.Implementers)
                                    {
                                        var implementerExists = (from i in implementersList
                                                                 where i.ImplementerId == implementer.ImplementerId && i.ProjectId == newProject.Id
                                                                 select i).FirstOrDefault();

                                        if (implementerExists == null)
                                        {
                                            implementersList.Add(new EFProjectImplementers()
                                            {
                                                ProjectId = newProject.Id,
                                                ImplementerId = implementer.ImplementerId
                                            });
                                        }
                                    }
                                }

                                if (project.Sectors.Count > 0)
                                {
                                    foreach(var sector in project.Sectors)
                                    {
                                        var sectorExists = (from s in sectorsList
                                                            where s.ProjectId == newProject.Id && s.SectorId == sector.SectorId
                                                            select s).FirstOrDefault();

                                        if (sectorExists == null)
                                        {
                                            sectorsList.Add(new EFProjectSectors()
                                            {
                                                ProjectId = newProject.Id,
                                                SectorId = sector.SectorId,
                                                FundsPercentage = sector.FundsPercentage
                                            });
                                        }
                                    }
                                }

                                if (project.Locations.Count > 0)
                                {
                                    foreach(var location in project.Locations)
                                    {
                                        var locationExists = (from l in locationsList
                                                              where l.LocationId == location.LocationId && l.ProjectId == newProject.Id
                                                              select l).FirstOrDefault();
      
                                        if (locationExists == null)
                                        {
                                            locationsList.Add(new EFProjectLocations()
                                            {
                                                ProjectId = newProject.Id,
                                                LocationId = location.LocationId,
                                                FundsPercentage = location.FundsPercentage
                                            });
                                        }
                                    }
                                }

                                if (project.Disbursements.Count > 0)
                                {
                                    foreach(var disbursement in project.Disbursements)
                                    {
                                        var disbursementExists = (from d in disbursementsList
                                                                  where d.ProjectId == newProject.Id &&
                                                                  d.Amount == disbursement.Amount &&
                                                                  d.Year.FinancialYear == disbursement.Year.FinancialYear
                                                                  select d).FirstOrDefault();

                                        if (disbursementExists == null)
                                        {
                                            disbursementsList.Add(new EFProjectDisbursements()
                                            {
                                                ProjectId = newProject.Id,
                                                Amount = disbursement.Amount,
                                                Currency = disbursement.Currency,
                                                ExchangeRate = disbursement.ExchangeRate,
                                                Year = disbursement.Year
                                            });
                                        }
                                        
                                    }
                                }

                                if (project.Documents.Count > 0)
                                {
                                    foreach(var document in project.Documents)
                                    {
                                        var documentExists = (from d in documentsList
                                                              where d.DocumentTitle == document.DocumentTitle &&
                                                              d.DocumentUrl == document.DocumentUrl &&
                                                              d.ProjectId == newProject.Id
                                                              select d).FirstOrDefault();

                                        if (documentExists == null)
                                        {
                                            documentsList.Add(new EFProjectDocuments()
                                            {
                                                ProjectId = newProject.Id,
                                                DocumentTitle = document.DocumentTitle,
                                                DocumentUrl = document.DocumentUrl
                                            });
                                        }
                                    }
                                }
                            }

                            if (fundersList.Count > 0)
                            {
                                unitWork.ProjectFundersRepository.InsertMultiple(fundersList);
                            }
                            
                            if (implementersList.Count > 0)
                            {
                                unitWork.ProjectImplementersRepository.InsertMultiple(implementersList);
                            }

                            if (sectorsList.Count > 0)
                            {
                                unitWork.ProjectSectorsRepository.InsertMultiple(sectorsList);
                            }

                            if (locationsList.Count > 0)
                            {
                                unitWork.ProjectLocationsRepository.InsertMultiple(locationsList);
                            }

                            if (disbursementsList.Count > 0)
                            {
                                unitWork.ProjectDisbursementsRepository.InsertMultiple(disbursementsList);
                            }

                            if (documentsList.Count > 0)
                            {
                                unitWork.ProjectDocumentRepository.InsertMultiple(documentsList);
                            }
                            await unitWork.SaveAsync();
                        }

                        
                        //Now delete the old projects
                        if (model.ProjectsIds.Count > 0)
                        {
                            var projectsToDelete = unitWork.ProjectRepository.GetManyQueryable(p => model.ProjectsIds.Contains(p.Id));

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
    }
}
