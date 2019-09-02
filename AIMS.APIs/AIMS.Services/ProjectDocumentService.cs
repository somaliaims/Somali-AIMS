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
    public interface IProjectDocumentService
    {
        /*/// <summary>
        /// Gets all documents for the provided project id
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectDocumentView> GetAll(int id);

        /// <summary>
        /// Gets all projectDocuments async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectDocumentView>> GetAllAsync(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectDocumentModel model);

        /// <summary>
        /// Updates a projectDocument
        /// </summary>
        /// <param name="projectDocument"></param>
        /// <returns></returns>
        ActionResponse Update(int id, ProjectDocumentModel model);*/
    }

    public class ProjectDocumentService : IProjectDocumentService
    {
        /*AIMSDbContext context;
        IMapper mapper;

        public ProjectDocumentService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectDocumentView> GetAll(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectDocuments = unitWork.ProjectDocumentRepository.Get(p => p.ProjectId.Equals(id));
                return mapper.Map<List<ProjectDocumentView>>(projectDocuments);
            }
        }

        public async Task<IEnumerable<ProjectDocumentView>> GetAllAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectDocuments = await unitWork.ProjectDocumentRepository.GetAsync(p => p.ProjectId.Equals(id));
                return await Task<IEnumerable<ProjectDocumentView>>.Run(() => mapper.Map<List<ProjectDocumentView>>(projectDocuments)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(ProjectDocumentModel model)
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
                        response.Message = mHelper.GetNotFound("Project");
                        return response;
                    }
                    

                    var newProjectDocument = unitWork.ProjectDocumentRepository.Insert(new EFProjectDocuments()
                    {
                        Project = project,
                        DocumentTitle = model.DocumentTitle,
                        DocumentUrl = model.DocumentUrl
                    });
                    response.ReturnedId = newProjectDocument.ProjectId;
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

        public ActionResponse Update(int id, ProjectDocumentModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectDocumentObj = unitWork.ProjectDocumentRepository.Get(f => f.Id.Equals(id));
                if (projectDocumentObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Document");
                    return response;
                }

                projectDocumentObj.DocumentTitle = model.DocumentTitle;
                projectDocumentObj.DocumentUrl = model.DocumentUrl;

                unitWork.ProjectDocumentRepository.Update(projectDocumentObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }*/
    }
}
