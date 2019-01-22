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
    public interface IProjectImplementerService
    {
        /// <summary>
        /// Gets all projectImplementers
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectImplementerView> GetAll();

        /// <summary>
        /// Gets all projectImplementers async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectImplementerView>> GetAllAsync();

        /// <summary>
        /// Gets list of implementers for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectImplementerView> GetProjectImplementers(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectImplementerModel projectImplementer);

        /// <summary>
        /// Updates a projectImplementer
        /// </summary>
        /// <param name="projectImplementer"></param>
        /// <returns></returns>
        ActionResponse RemoveImplementer(ProjectImplementerModel model);

        /// <summary>
        /// Deletes organization type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class ProjectImplementerService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectImplementerService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectImplementerView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectImplementers = unitWork.ProjectImplementersRepository.GetAll();
                return mapper.Map<List<ProjectImplementerView>>(projectImplementers);
            }
        }

        public async Task<IEnumerable<ProjectImplementerView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectImplementers = await unitWork.ProjectImplementersRepository.GetAllAsync();
                return await Task<IEnumerable<ProjectImplementerView>>.Run(() => mapper.Map<List<ProjectImplementerView>>(projectImplementers)).ConfigureAwait(false);
            }
        }

        public IEnumerable<ProjectImplementerView> GetProjectImplementers(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(i => i.ProjectId == id);
                return mapper.Map<List<ProjectImplementerView>>(projectImplementers);
            }
        }

        public ActionResponse Add(ProjectImplementerModel model)
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

                    var implementer = unitWork.OrganizationRepository.GetByID(model.ImplementerId);
                    if (implementer == null)
                    {
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Organization/Implementer");
                        return response;
                    }

                    var newProjectImplementer = unitWork.ProjectImplementersRepository.Insert(new EFProjectImplementers()
                    {
                        Project = project,
                        Implementer = implementer,
                    });
                    response.ReturnedId = newProjectImplementer.ImplementerId;
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

        public ActionResponse RemoveImplementer(ProjectImplementerModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectImplementer = unitWork.ProjectImplementersRepository.Get(pf => pf.ImplementerId.Equals(model.ImplementerId) && pf.ProjectId.Equals(model.ProjectId));
                if (projectImplementer == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Implementer");
                    return response;
                }

                unitWork.ProjectImplementersRepository.Delete(projectImplementer);
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
                var projectImplementerObj = unitWork.ProjectImplementersRepository.GetByID(id);
                if (projectImplementerObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Implementer");
                    return response;
                }

                unitWork.ProjectImplementersRepository.Delete(projectImplementerObj);
                unitWork.Save();
                return response;
            }
        }
    }
}
