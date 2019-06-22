using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Linq;
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
        IEnumerable<ProjectMembershipRequestView> GetRequestsForFunder(int funderId);

        /// <summary>
        /// Gets list of projects for which membership is approved
        /// </summary>
        /// <param name="funderId"></param>
        /// <returns></returns>
        IEnumerable<int> GetProjectsMembership(int funderId);

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
        ActionResponse ApproveMembershipRequest(ProjectMembershipRequestModel model, int funderId);

        /// <summary>
        /// Un-Approves membership request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse UnApproveMembershipRequest(ProjectMembershipRequestModel model, int funderId);
    }
    public class ProjectMembershipService : IProjectMembershipService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectMembershipService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectMembershipRequestView> GetRequestsForFunder(int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var funderProjects = unitWork.ProjectFundersRepository.GetManyQueryable(p => p.FunderId == funderId);
                List<int> funderProjectIds = (from f in funderProjects
                                        select f.ProjectId).ToList<int>();
                var implementerProjects = unitWork.ProjectImplementersRepository.GetManyQueryable(p => p.ImplementerId == funderId);
                List<int> implementerProjectIds = (from i in implementerProjects
                                            select i.ProjectId).ToList<int>();
                List<int> projectIds = funderProjectIds.Union(implementerProjectIds).ToList<int>();
                var requests = unitWork.ProjectMembershipRepository.GetWithInclude(r => projectIds.Contains(r.ProjectId) && r.IsApproved == false, new string[] { "Project", "User", "User.Organization" });
                return mapper.Map<List<ProjectMembershipRequestView>>(requests);
            }
        }

        public IEnumerable<int> GetProjectsMembership(int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<int> approvedProjectIds = new List<int>();
                var funderProjects = unitWork.ProjectFundersRepository.GetManyQueryable(p => p.FunderId == funderId);
                List<int> projectIds = (from f in funderProjects
                                        select f.ProjectId).ToList<int>();
                var requests = unitWork.ProjectMembershipRepository.GetManyQueryable(r => projectIds.Contains(r.ProjectId) && r.IsApproved == true);
                if (requests != null)
                {
                    approvedProjectIds = (from r in requests
                                          select r.ProjectId).ToList<int>();
                }
                return approvedProjectIds;
            }
        }

        public ActionResponse AddMembershipRequest(ProjectMembershipRequestModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                
                try
                {
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                        return response;
                    }

                    var user = unitWork.UserRepository.GetOne(u => u.Email == model.UserEmail);
                    if (user == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("User");
                        response.Success = false;
                        return response;
                    }

                    var isRequestExists = unitWork.ProjectMembershipRepository.GetOne(r => (r.ProjectId == model.ProjectId && r.UserId == user.Id));
                    if (isRequestExists != null)
                    {
                        isRequestExists.Dated = DateTime.Now;
                        unitWork.ProjectMembershipRepository.Update(isRequestExists);
                    }
                    else
                    {
                        unitWork.ProjectMembershipRepository.Insert(new EFProjectMembershipRequests()
                        {
                            Project = project,
                            User = user,
                            Dated = DateTime.Now,
                            IsApproved = false
                        });
                    }
                    unitWork.Save();

                    //Send email to all members of the funder
                    var projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(p => p.ProjectId == model.ProjectId, p => p.FunderId);
                    var users = unitWork.UserRepository.GetManyQueryable(u => projectFunderIds.Contains(u.OrganizationId));
                    var requestingUser = unitWork.UserRepository.GetWithInclude(u => u.Id == user.Id, new string[] { "Organization" }).FirstOrDefault();
                    string requestingOrganization = requestingUser != null ? requestingUser.Organization.OrganizationName : null;

                    List<EmailAddress> usersEmailList = new List<EmailAddress>();
                    foreach (var u in users)
                    {
                        usersEmailList.Add(new EmailAddress()
                        {
                            Email = u.Email,
                        });

                        if (usersEmailList.Count > 0)
                        {
                            //Send emails
                            ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                            var smtpSettings = smtpService.GetPrivate();
                            SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                            if (smtpSettings != null)
                            {
                                smtpSettingsModel.Host = smtpSettings.Host;
                                smtpSettingsModel.Port = smtpSettings.Port;
                                smtpSettingsModel.Username = smtpSettings.Username;
                                smtpSettingsModel.Password = smtpSettings.Password;
                                smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                            }

                            string message = "", subject = "";
                            var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewOrgToProject);
                            if (emailMessage != null)
                            {
                                subject = emailMessage.Subject;
                                message = emailMessage.Message;
                            }

                            mHelper = new MessageHelper();
                            message += mHelper.NewOrganizationForProject(requestingOrganization);
                            IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(usersEmailList, subject, "Dear user,", message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
            }
        }

        public ActionResponse ApproveMembershipRequest(ProjectMembershipRequestModel model, int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                var isFunderOwner = unitWork.ProjectFundersRepository.GetOne(f => f.FunderId == funderId && f.ProjectId == model.ProjectId);
                if (isFunderOwner == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetInvalidFunderApprovalMessage();
                    response.Success = false;
                    return response;
                }

                var user = unitWork.UserRepository.GetOne(u => u.Email == model.UserEmail);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }
                var isRequestExists = unitWork.ProjectMembershipRepository.GetOne(r => r.ProjectId == model.ProjectId && r.UserId == user.Id);
                if (isRequestExists == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Membership Request");
                    response.Success = false;
                    return response;
                }
                isRequestExists.IsApproved = true;
                unitWork.ProjectMembershipRepository.Update(isRequestExists);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse UnApproveMembershipRequest(ProjectMembershipRequestModel model, int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                var isFunderOwner = unitWork.ProjectFundersRepository.GetOne(f => f.FunderId == funderId && f.ProjectId == model.ProjectId);
                if (isFunderOwner == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetInvalidFunderApprovalMessage();
                    response.Success = false;
                    return response;
                }

                var user = unitWork.UserRepository.GetOne(u => u.Email == model.UserEmail);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }
                var isRequestExists = unitWork.ProjectMembershipRepository.GetOne(r => r.ProjectId == model.ProjectId && r.UserId == user.Id);
                if (isRequestExists == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Membership Request");
                    response.Success = false;
                    return response;
                }
                unitWork.ProjectMembershipRepository.Delete(isRequestExists);
                unitWork.Save();
                return response;
            }
        }
    }
}
