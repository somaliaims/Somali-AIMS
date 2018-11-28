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
    public interface IProjectImplementorService
    {
        /// <summary>
        /// Gets all projectImplementors
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectImplementorView> GetAll();

        /// <summary>
        /// Gets all projectImplementors async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectImplementorView>> GetAllAsync();

        /// <summary>
        /// Gets list of implementors for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ProjectImplementorView> GetProjectImplementors(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectImplementorModel projectImplementor);

        /// <summary>
        /// Updates a projectImplementor
        /// </summary>
        /// <param name="projectImplementor"></param>
        /// <returns></returns>
        ActionResponse RemoveImplementor(ProjectImplementorModel model);

        /// <summary>
        /// Deletes organization type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class ProjectImplementorService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectImplementorService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectImplementorView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectImplementors = unitWork.ProjectImplementorsRepository.GetAll();
                return mapper.Map<List<ProjectImplementorView>>(projectImplementors);
            }
        }

        public async Task<IEnumerable<ProjectImplementorView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectImplementors = await unitWork.ProjectImplementorsRepository.GetAllAsync();
                return await Task<IEnumerable<ProjectImplementorView>>.Run(() => mapper.Map<List<ProjectImplementorView>>(projectImplementors)).ConfigureAwait(false);
            }
        }

        public IEnumerable<ProjectImplementorView> GetProjectImplementors(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectImplementors = unitWork.ProjectImplementorsRepository.GetMany(i => i.ProjectId == id);
                return mapper.Map<List<ProjectImplementorView>>(projectImplementors);
            }
        }

        public ActionResponse Add(ProjectImplementorModel model)
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

                    var implementor = unitWork.OrganizationRepository.GetByID(model.ImplementorId);
                    if (implementor == null)
                    {
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Organization/Implementor");
                        return response;
                    }

                    var newProjectImplementor = unitWork.ProjectImplementorsRepository.Insert(new EFProjectImplementors()
                    {
                        Project = project,
                        Implementor = implementor,
                    });
                    response.ReturnedId = newProjectImplementor.ImplementorId;
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

        public ActionResponse RemoveImplementor(ProjectImplementorModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectImplementor = unitWork.ProjectImplementorsRepository.Get(pf => pf.ImplementorId.Equals(model.ImplementorId) && pf.ProjectId.Equals(model.ProjectId));
                if (projectImplementor == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Implementor");
                    return response;
                }

                unitWork.ProjectImplementorsRepository.Delete(projectImplementor);
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
                var projectImplementorObj = unitWork.ProjectImplementorsRepository.GetByID(id);
                if (projectImplementorObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Implementor");
                    return response;
                }

                unitWork.ProjectImplementorsRepository.Delete(projectImplementorObj);
                unitWork.Save();
                return response;
            }
        }
    }
}
