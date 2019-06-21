using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IProjectMembershipService
    {
        /// <summary>
        /// Gets list of membership requests for provided projects
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        IEnumerable<ProjectMembershipRequestView> GetRequestForProject(List<int> projectIds);

        /// <summary>
        /// Adds new project membership request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddMembershipRequest(ProjectMembershipRequestModel model);

        /// <summary>
        /// Approves membership request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse ApproveMembershipRequest(ProjectMembershipRequestModel model);

        /// <summary>
        /// Un-Approves membership request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse UnApproveMembershipRequest(ProjectMembershipRequestModel model);
    }
    public class ProjectMembershipService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectMembershipService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectMembershipRequestView> GetRequestForProject(List<int> projectIds)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var requests = unitWork.ProjectMembershipRepository.GetWithInclude(r => projectIds.Contains(r.ProjectId) && r.IsApproved == false, new string[] { "Project", "Organization" });
                return mapper.Map<List<ProjectMembershipRequestView>>(requests);
            }
        }

        public ActionResponse AddMembershipRequest(ProjectMembershipRequestModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var project = unitWork.ProjectMembershipRepository.GetByID(model.ProjectId);
                return response;
            }
        }
    }
}
