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
        /// Gets help for list of fields under project expected disbursement
        /// </summary>
        /// <returns></returns>
        ExpectedDisbursementHelp GetHelpForProjectExpectedDisbursementFields();

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
        /// Adds help for project expected disbusements
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddHelpForProjectExpectedDisbursements(ExpectedDisbursementHelp model);

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

        public ExpectedDisbursementHelp GetHelpForProjectExpectedDisbursementFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ExpectedDisbursementHelp help = new ExpectedDisbursementHelp();
                var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectExpectedDisbursements);
                if (projectHelp != null)
                {
                    if (!string.IsNullOrEmpty(projectHelp.HelpInfoJson))
                    {
                        help = JsonConvert.DeserializeObject<ExpectedDisbursementHelp>(projectHelp.HelpInfoJson);
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

        public ActionResponse AddHelpForProjectExpectedDisbursements(ExpectedDisbursementHelp model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var projectHelp = unitWork.HelpRepository.GetOne(h => h.Entity == HelpForEntity.ProjectExpectedDisbursements);
                    if (projectHelp != null)
                    {
                        projectHelp.HelpInfoJson = JsonConvert.SerializeObject(model);
                    }
                    else
                    {
                        unitWork.HelpRepository.Insert(new EFHelp()
                        {
                            Entity = HelpForEntity.ProjectExpectedDisbursements,
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
