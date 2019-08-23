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
    public interface IProjectFunderService
    {
        /// <summary>
        /// Gets all projectFunders
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectFunderView> GetAll();

        /// <summary>
        /// Gets all projectFunders async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectFunderView>> GetAllAsync();

        /// <summary>
        /// Gets funders list for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectFunderView> GetProjectFunders(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectFunderModel projectFunder);

        /// <summary>
        /// Updates a projectFunder
        /// </summary>
        /// <param name="projectFunder"></param>
        /// <returns></returns>
        ActionResponse RemoveFunder(ProjectFunderModel model);

        /// <summary>
        /// Deletes organization type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class ProjectFunderService : IProjectFunderService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectFunderService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectFunderView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectFunders = unitWork.ProjectFundersRepository.GetAll();
                return mapper.Map<List<ProjectFunderView>>(projectFunders);
            }
        }

        public async Task<IEnumerable<ProjectFunderView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectFunders = await unitWork.ProjectFundersRepository.GetAllAsync();
                return await Task<IEnumerable<ProjectFunderView>>.Run(() => mapper.Map<List<ProjectFunderView>>(projectFunders)).ConfigureAwait(false);
            }
        }

        public IEnumerable<ProjectFunderView> GetProjectFunders(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => f.ProjectId == id);
                return mapper.Map<List<ProjectFunderView>>(projectFunders);
            }
        }

        public ActionResponse Add(ProjectFunderModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper = new MessageHelper();
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Project");
                        return response;
                    }

                    var funder = unitWork.OrganizationRepository.GetByID(model.FunderId);
                    if (funder == null)
                    {
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Organization/Funder");
                        return response;
                    }

                    var newProjectFunder = unitWork.ProjectFundersRepository.Insert(new EFProjectFunders()
                    {
                        Project = project,
                        Funder = funder,
                    });
                    response.ReturnedId = newProjectFunder.FunderId;
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

        public ActionResponse RemoveFunder(ProjectFunderModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectFunder = unitWork.ProjectFundersRepository.Get(pf => pf.FunderId.Equals(model.FunderId) && pf.ProjectId.Equals(model.ProjectId));
                if (projectFunder == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Funder");
                    return response;
                }

                unitWork.ProjectFundersRepository.Delete(projectFunder);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectFunderObj = unitWork.ProjectFundersRepository.GetByID(id);
                if (projectFunderObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Funder");
                    return response;
                }

                unitWork.ProjectFundersRepository.Delete(projectFunderObj);
                unitWork.Save();
                return response;
            }
        }
    }
}
