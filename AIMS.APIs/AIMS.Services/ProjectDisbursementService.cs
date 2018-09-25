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
    public interface IProjectDisbursementService
    {
        /// <summary>
        /// Gets all documents for the provided project id
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectDisbursementView> GetAll(int id);

        /// <summary>
        /// Gets all projectDisbursements async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectDisbursementView>> GetAllAsync(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectDisbursementModel model);

        /// <summary>
        /// Updates a projectDisbursement
        /// </summary>
        /// <param name="projectDisbursement"></param>
        /// <returns></returns>
        ActionResponse Update(int id, ProjectDisbursementModel model);
    }

    public class ProjectDisbursementService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectDisbursementService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectDisbursementView> GetAll(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectDisbursements = unitWork.ProjectDisbursementsRepository.Get(p => p.ProjectId.Equals(id));
                return mapper.Map<List<ProjectDisbursementView>>(projectDisbursements);
            }
        }

        public async Task<IEnumerable<ProjectDisbursementView>> GetAllAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectDisbursements = await unitWork.ProjectDisbursementsRepository.GetAsync(p => p.ProjectId.Equals(id));
                return await Task<IEnumerable<ProjectDisbursementView>>.Run(() => mapper.Map<List<ProjectDisbursementView>>(projectDisbursements)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(ProjectDisbursementModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Project Disbursement");
                        return response;
                    }


                    var newProjectDisbursement = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                    {
                        Project = project,
                    });
                    response.ReturnedId = newProjectDisbursement.ProjectId;
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

        public ActionResponse Update(int id, ProjectDisbursementModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectDisbursementObj = unitWork.ProjectDisbursementsRepository.Get(d => d.Id.Equals(id));
                if (projectDisbursementObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Disbursement");
                    return response;
                }

                unitWork.ProjectDisbursementsRepository.Update(projectDisbursementObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}
