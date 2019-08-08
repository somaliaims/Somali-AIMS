using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IProjectDeletionService
    {
        /// <summary>
        /// Adds new request for project deletion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddRequest(ProjectDeletionRequestModel model);
    }

    public class ProjectDeletionService
    {
        AIMSDbContext context;

        public ProjectDeletionService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public ActionResponse AddRequest(ProjectDeletionRequestModel model)
        {
            using(var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project");
                    return response;
                }

                var user = unitWork.UserRepository.GetOne(u => u.Email == model.UserEmail);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("User");
                    return response;
                }

                var isRequestExists = unitWork.ProjectDeletionRepository.GetOne(p => p.ProjectId == project.Id && p.Status == ProjectDeletionStatus.Requested);
                if (isRequestExists != null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetProjectDeletionExistsMessage();
                    return response;
                }

                unitWork.ProjectDeletionRepository.Insert(new EFProjectDeletionRequests()
                {
                    Project = project,
                    RequestedBy = user,
                    RequestedOn = DateTime.Now,
                    StatusUpdatedOn = DateTime.Now,
                    Status = ProjectDeletionStatus.Requested
                });

                unitWork.Save();
                return response;
            }
        }
    }
}
