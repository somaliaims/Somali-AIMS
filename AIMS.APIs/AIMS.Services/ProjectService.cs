using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
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
        ActionResponse Add(ProjectModel project);

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
        ActionResponse AddProjectFunder(ProjectFunderModel model);

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
        /// Adds marker to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectMarker(ProjectMarkersModel model);

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
        /// Gets project markers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectMarkersView> GetProjectMarkers(int id);

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
        /// Deletes the disbursement with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectDisbursement(int id, int year, int month);

        /// <summary>
        /// Deletes a document for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectDocument(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectMarker(int id);
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
                return mapper.Map<List<ProjectView>>(projects);
            }
        }

        public async Task<IEnumerable<ProjectView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projects = await unitWork.ProjectRepository.GetAllAsync();
                return await Task<IEnumerable<ProjectView>>.Run(() => mapper.Map<List<ProjectView>>(projects)).ConfigureAwait(false);
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
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id.Equals(id), new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents" });
                ProjectProfileView profileView = new ProjectProfileView();

                if (projectProfileList != null)
                {
                    foreach (var project in projectProfileList)
                    {
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);
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
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.StartDate.Year >= model.StartingYear && p.EndDate.Year <= model.EndingYear)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from project in projectProfileList
                                                  where project.StartDate.Year >= model.StartingYear &&
                                                  project.EndDate.Year <= model.EndingYear
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
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    foreach (var project in projectProfileList)
                    {
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);

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
                            projectProfileList = await unitWork.ProjectRepository.GetManyQueryableAsync(p => (p.StartDate.Year >= model.StartingYear && p.EndDate.Year <= model.EndingYear));
                        }
                        else
                        {
                            projectProfileList = (from project in projectProfileList
                                                  where project.StartDate.Year >= model.StartingYear &&
                                                  project.EndDate.Year <= model.EndingYear
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
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
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
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ids.Contains(p.Id), new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents" });
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
                            StartDate = project.StartDate.ToLongDateString(),
                            EndDate = project.EndDate.ToLongDateString(),
                            Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors),
                            Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations),
                            Funders = mapper.Map<List<ProjectFunderView>>(project.Funders),
                            Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers),
                            Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements),
                            Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents)
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
                            profileView.StartDate = project.StartDate.ToLongDateString();
                            profileView.EndDate = project.EndDate.ToLongDateString();
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
                var disbursements = unitWork.ProjectDisbursementsRepository.GetMany(s => s.ProjectId == id);
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

        public IEnumerable<ProjectMarkersView> GetProjectMarkers(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var markers = unitWork.ProjectMarkersRepository.GetMany(d => d.ProjectId == id);
                return mapper.Map<List<ProjectMarkersView>>(markers);
            }
        }

        public ActionResponse Add(ProjectModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newProject = unitWork.ProjectRepository.Insert(new EFProject()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    });
                    unitWork.Save();
                    response.ReturnedId = newProject.Id;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
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

        public ActionResponse AddProjectFunder(ProjectFunderModel model)
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
                    var funder = unitWork.OrganizationRepository.GetByID(model.FunderId);
                    if (funder == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Funder");
                        response.Success = false;
                    }

                    unitWork.ProjectFundersRepository.Insert(new EFProjectFunders()
                    {
                        Project = project,
                        Funder = funder,
                        Amount = model.Amount,
                        Currency = model.Currency,
                        ExchangeRate = model.ExchangeRate
                    });

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

                    unitWork.ProjectImplementersRepository.Insert(new EFProjectImplementers()
                    {
                        Project = project,
                        Implementer = implementer,
                    });
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
                    }

                    unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                    {
                        Project = project,
                        Year = model.Year,
                        Month = model.Month,
                        Amount = model.Amount,
                    });
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

        public ActionResponse AddProjectMarker(ProjectMarkersModel model)
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

                    unitWork.ProjectMarkersRepository.Insert(new EFProjectMarkers()
                    {
                        Project = project,
                        Marker = model.Marker,
                        Percentage = model.Percentage
                    });
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

                projectObj.Title = model.Title;
                projectObj.Description = model.Description;
                projectObj.StartDate = model.StartDate;
                projectObj.EndDate = model.EndDate;

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

        public ActionResponse DeleteProjectDisbursement(int projectId, int year, int month)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectDisbursement = unitWork.ProjectDisbursementsRepository.GetSingle(d => d.ProjectId == projectId && d.Year == year && d.Month == month);
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

        public ActionResponse DeleteProjectMarker(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectMarker = unitWork.ProjectMarkersRepository.GetByID(id);
                IMessageHelper mHelper;
                if (projectMarker == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Marker");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectMarkersRepository.Delete(projectMarker);
                unitWork.Save();
                return response;
            }
        }
    }
}
