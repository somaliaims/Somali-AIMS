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
    public interface IProjectTypesService
    {
        /// <summary>
        /// Gets all projectTypes
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectTypesView> GetAll();

        /// <summary>
        /// Gets sector type for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProjectTypesView Get(int id);

        /// <summary>
        /// Gets matching sector types for the provided criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<ProjectTypesView> GetMatching(string criteria);

        /// <summary>
        /// Gets all projectTypes async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectTypesView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectTypesModel projectType);

        /// <summary>
        /// Updates a projectType
        /// </summary>
        /// <param name="projectType"></param>
        /// <returns></returns>
        ActionResponse Update(int id, ProjectTypesModel projectType);
    }

    public class ProjectTypesService : IProjectTypesService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectTypesService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectTypesView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectTypes = unitWork.ProjectTypesRepository.GetAll();
                return mapper.Map<List<ProjectTypesView>>(projectTypes);
            }
        }

        public ProjectTypesView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectType = unitWork.ProjectTypesRepository.GetByID(id);
                return mapper.Map<ProjectTypesView>(projectType);
            }
        }

        public async Task<IEnumerable<ProjectTypesView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectTypes = await unitWork.ProjectTypesRepository.GetAllAsync();
                return await Task<IEnumerable<ProjectTypesView>>.Run(() => mapper.Map<List<ProjectTypesView>>(projectTypes)).ConfigureAwait(false);
            }
        }

        public IEnumerable<ProjectTypesView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectTypesView> projectTypesList = new List<ProjectTypesView>();
                var projectTypes = unitWork.ProjectTypesRepository.GetMany(o => o.TypeName.Contains(criteria));
                return mapper.Map<List<ProjectTypesView>>(projectTypes);
            }
        }

        public ActionResponse Add(ProjectTypesModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newProjectTypes = unitWork.ProjectTypesRepository.Insert(new EFProjectTypes() { TypeName = model.TypeName });
                    response.ReturnedId = newProjectTypes.Id;
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

        public ActionResponse Update(int id, ProjectTypesModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectTypeObj = unitWork.ProjectTypesRepository.GetByID(id);
                if (projectTypeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Types");
                    return response;
                }

                projectTypeObj.TypeName = model.TypeName;
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}
