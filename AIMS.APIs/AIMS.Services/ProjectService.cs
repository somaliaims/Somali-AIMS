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
        ActionResponse Update(int id, ProjectModel project);
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

        public IEnumerable<ProjectView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectView> sectorTypesList = new List<ProjectView>();
                var projects = unitWork.ProjectRepository.GetMany(p => p.Title.Contains(criteria));
                return mapper.Map<List<ProjectView>>(projects);
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
                        Objective = model.Objective,
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

        public ActionResponse Update(int id, ProjectModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectObj = unitWork.ProjectRepository.GetByID(id);
                if (projectObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project");
                    return response;
                }

                projectObj.Title = model.Title;
                projectObj.Objective = model.Objective;
                projectObj.StartDate = model.StartDate;
                projectObj.EndDate = model.EndDate;

                unitWork.ProjectRepository.Update(projectObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}
