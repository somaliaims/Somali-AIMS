using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
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
        /// Adds implementor to a project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddProjectImplementor(ProjectImplementorModel model);

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
        /// Gets implementors for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectImplementorView> GetProjectImplementors(int id);

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
        /// Deletes project implementor
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="implementorId"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectImplementor(int projectId, int implementorId);

        /// <summary>
        /// Deletes the disbursement with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse DeleteProjectDisbursement(int id, int startingYear);

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
                var projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id.Equals(id), new string[] { "Sectors", "Locations", "Disbursements", "Funders", "Implementors", "Documents" });
                ProjectProfileView profileView = new ProjectProfileView();

                if (projectProfileList != null)
                {
                    foreach(var project in projectProfileList)
                    {
                        profileView.Id = project.Id;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();

                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementorView>>(project.Implementors);
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
                var projects = unitWork.ProjectRepository.GetMany(p => p.Title.Contains(criteria));
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

        public IEnumerable<ProjectImplementorView> GetProjectImplementors(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var implementors = unitWork.ProjectImplementorsRepository.GetWithInclude(s => s.ProjectId == id, new string[] { "Implementor" });
                return mapper.Map<List<ProjectImplementorView>>(implementors);
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
                    response.ReturnedId = newProject.Id;
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
                catch(Exception ex)
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

                    unitWork.ProjectSectorsRepository.Insert(new EFProjectSectors()
                    {
                        Project = project,
                        Sector = sector,
                        FundsPercentage = model.FundsPercentage,
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

        public ActionResponse AddProjectImplementor(ProjectImplementorModel model)
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
                    var implementor = unitWork.OrganizationRepository.GetByID(model.ImplementorId);
                    if (implementor == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Implementor");
                        response.Success = false;
                    }

                    unitWork.ProjectImplementorsRepository.Insert(new EFProjectImplementors()
                    {
                        Project = project,
                        Implementor = implementor,
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
                        StartingYear = model.StartingYear,
                        StartingMonth = model.StartingMonth,
                        EndingYear = model.EndingYear,
                        EndingMonth = model.EndingMonth,
                        Percentage = model.Percentage,
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

        public ActionResponse DeleteProjectImplementor(int projectId, int implementorId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectImplementor = unitWork.ProjectImplementorsRepository.Get(i => i.ProjectId == projectId && i.ImplementorId == implementorId);
                IMessageHelper mHelper;
                if (projectImplementor == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project Implementor");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectImplementorsRepository.Delete(projectImplementor);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse DeleteProjectDisbursement(int projectId, int startingYear)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectDisbursement = unitWork.ProjectDisbursementsRepository.GetSingle(d => d.ProjectId == projectId && d.StartingYear == startingYear);
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
