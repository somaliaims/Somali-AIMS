using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IHelpService
    {
        /// <summary>
        /// Gets help for list of fields under project
        /// </summary>
        /// <returns></returns>
        ProjectHelp GetHelpForProjectFields();

        /// <summary>
        /// Gets help for list of fields under project funder
        /// </summary>
        /// <returns></returns>
        ProjectFunderHelp GetHelpForProjectFunderFields();

        /// <summary>
        /// Gets help for project locaitons
        /// </summary>
        /// <returns></returns>
        ProjectLocationHelp GetHelpForProjectLocations();

        /// <summary>
        /// Get help for project sectors
        /// </summary>
        /// <returns></returns>
        ProjectSectorHelp GetHelpForProjectSectors();

        /// <summary>
        /// Gets help for list of fields under project implementer
        /// </summary>
        /// <returns></returns>
        ProjectImplementerHelp GetHelpForProjectImpelenterFields();

        /// <summary>
        /// Gets help for list of fields under project disbursements
        /// </summary>
        /// <returns></returns>
        ProjectDisbursementHelp GetHelpForProjectDisbursementFields();

        /// <summary>
        /// Gets help for list of fields under project documents
        /// </summary>
        /// <returns></returns>
        ProjectDocumentHelp GetHelpForProjectDocumentsFields();

        /// <summary>
        /// Adds help for project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProject(ProjectHelp model);

        /// <summary>
        /// Adds help for project funder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectFunder(ProjectFunderHelp model);

        /// <summary>
        /// Add help for project sector
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectSector(ProjectSectorHelp model);

        /// <summary>
        /// Add help for project location
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectLocation(ProjectLocationHelp model);

        /// <summary>
        /// Adds help for project implementer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectImplementer(ProjectImplementerHelp model);

        /// <summary>
        /// Adds help for project disbursements
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectDisbursement(ProjectDisbursementHelp model);

        /// <summary>
        /// Adds help for project document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectDocument(ProjectDocumentHelp model);
    }

    public class HelpService : IHelpService
    {
        AIMSDbContext context;
        IMapper mapper;

        public HelpService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public ProjectHelp GetHelpForProjectFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectHelp help = new ProjectHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.Project);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ProjectFunderHelp GetHelpForProjectFunderFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectFunderHelp help = new ProjectFunderHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectFunders);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectFunderHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ProjectLocationHelp GetHelpForProjectLocations()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectLocationHelp help = new ProjectLocationHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectLocations);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectLocationHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ProjectSectorHelp GetHelpForProjectSectors()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectSectorHelp help = new ProjectSectorHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectSectors);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectSectorHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ProjectImplementerHelp GetHelpForProjectImpelenterFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectImplementerHelp help = new ProjectImplementerHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectImplementers);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectImplementerHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ProjectDisbursementHelp GetHelpForProjectDisbursementFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectDisbursementHelp help = new ProjectDisbursementHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectDisbursements);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectDisbursementHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ProjectDocumentHelp GetHelpForProjectDocumentsFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectDocumentHelp help = new ProjectDocumentHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectExpectedDisbursements);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ProjectDocumentHelp>(projectHelp.HelpInfoJson);
                    }
                }
                return help;
            }
        }

        public ActionResponse AddHelpForProject(ProjectHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.Project);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.Project,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

        public ActionResponse AddHelpForProjectFunder(ProjectFunderHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectFunders);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectFunders,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

        public ActionResponse AddHelpForProjectSector(ProjectSectorHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectSectors);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectSectors,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

        public ActionResponse AddHelpForProjectLocation(ProjectLocationHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectLocations);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectLocations,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

        public ActionResponse AddHelpForProjectImplementer(ProjectImplementerHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectImplementers);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectImplementers,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

        public ActionResponse AddHelpForProjectDisbursement(ProjectDisbursementHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectDisbursements);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectDisbursements,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

        public ActionResponse AddHelpForProjectDocument(ProjectDocumentHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectDocuments);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectDocuments,
                            HelpInfoJson = JsonConvert.SerializeObject(model)
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

    }
}
