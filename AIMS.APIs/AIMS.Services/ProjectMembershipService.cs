using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AIMS.Services
{
    public interface IProjectMembershipService
    {
        /// <summary>
        /// Gets list of membership requests for provided projects
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        IEnumerable<ProjectMembershipRequestView> GetRequestsForFunder(int funderId, int userId, UserTypes userType = UserTypes.Standard);

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
        Task<ActionResponse> ApproveMembershipRequestAsync(int userId, int projectId, int funderId, int ownerId, MembershipTypes membershipType);

        /// <summary>
        /// Un-Approves membership request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse UnApproveMembershipRequest(int userId, int projectId, int funderId, int ownerId, MembershipTypes membershipType);
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

        public IEnumerable<ProjectMembershipRequestView> GetRequestsForFunder(int funderId, int userId, UserTypes userType = UserTypes.Standard)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IQueryable<EFProjectMembershipRequests> requests = null;
                if (userType != UserTypes.Manager && userType != UserTypes.SuperAdmin)
                {
                    var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(p => p.FunderId == funderId, p => p.ProjectId);
                    var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ImplementerId == funderId, p => p.ProjectId);
                    var userOwnedProjects = unitWork.ProjectRepository.GetProjection(p => p.CreatedById == userId, p => p.Id);
                    var projectIds = funderProjectIds.Union(implementerProjectIds).ToList<int>();
                    List<int> userOwnedProjectIds = new List<int>();
                    foreach (var pid in userOwnedProjects)
                    {
                        userOwnedProjectIds.Add((int)pid);
                    }
                    projectIds = projectIds.Union(userOwnedProjectIds).ToList<int>();
                    requests = unitWork.ProjectMembershipRepository.GetWithInclude(r => projectIds.Contains(r.ProjectId) && r.UserId != userId && r.IsApproved == false, new string[] { "Project", "User", "User.Organization" });
                }
                else
                {
                    requests = unitWork.ProjectMembershipRepository.GetWithInclude(r => r.UserId != userId && r.IsApproved == false, new string[] { "Project", "User", "User.Organization" });
                }
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

                    int membershipConstant = (int)model.MembershipType;
                    var requestExists = unitWork.ProjectMembershipRepository.GetOne(r => (r.ProjectId == model.ProjectId && r.UserId == user.Id && r.MembershipType == (MembershipTypes)model.MembershipType));
                    if (requestExists != null)
                    {
                        mHelper = new MessageHelper();
                        requestExists.Dated = DateTime.Now;
                        unitWork.ProjectMembershipRepository.Update(requestExists);
                        response.Message = mHelper.AlreadyExists("Membership request");
                        return response;
                    }
                    else
                    {
                        requestExists = unitWork.ProjectMembershipRepository.Insert(new EFProjectMembershipRequests()
                        {
                            Project = project,
                            User = user,
                            OrganizationId = user.OrganizationId,
                            Dated = DateTime.Now,
                            MembershipType = model.MembershipType,
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
                    }
                    if (usersEmailList.Count == 0)
                    {
                        var projectOwner = unitWork.UserRepository.GetOne(o => o.Id == project.CreatedById);
                        if (projectOwner != null)
                        {
                            usersEmailList.Add(new EmailAddress()
                            {
                                Email = projectOwner.Email
                            });
                        }
                    }

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
                            smtpSettingsModel.SenderName = smtpSettings.SenderName;
                        }

                        string message = "", subject = "", footerMessage = "";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewOrgToProject);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }

                        mHelper = new MessageHelper();
                        string role = model.MembershipType.ToString();
                        message += mHelper.NewOrganizationForProject(requestingOrganization, project.Title, role);
                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                        emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
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

        public async Task<ActionResponse> ApproveMembershipRequestAsync(int userId, int projectId, int funderId, int ownerId, MembershipTypes membershipType)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                UserTypes userType = UserTypes.Standard;

                var user = unitWork.UserRepository.GetOne(u => u.Id == userId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }

                var actionUser = unitWork.UserRepository.GetOne(u => u.Id == ownerId);
                if (actionUser != null)
                {
                    userType = actionUser.UserType;
                }

                if (userType != UserTypes.Manager && userType != UserTypes.SuperAdmin)
                {
                    var isUserOwner = unitWork.ProjectRepository.GetOne(p => (p.Id == projectId && p.CreatedById == ownerId));
                    if (isUserOwner == null)
                    {
                        var isFunderOwner = unitWork.ProjectFundersRepository.GetOne(f => f.FunderId == funderId && f.ProjectId == projectId);
                        var isImplementerOwner = unitWork.ProjectImplementersRepository.GetOne(i => i.ImplementerId == funderId && i.ProjectId == projectId);

                        if (isFunderOwner == null && isImplementerOwner == null)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.GetInvalidFunderApprovalMessage();
                            response.Success = false;
                            return response;
                        }
                    }
                }

                var project = unitWork.ProjectRepository.GetOne(p => p.Id == projectId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project");
                    response.Success = false;
                    return response;
                }

                var userOrganization = unitWork.OrganizationRepository.GetOne(o => o.Id == user.OrganizationId);
                if (userOrganization == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User Organization");
                    response.Success = false;
                    return response;
                }

                IQueryable<EFProjectMembershipRequests> requests = null;
                if (userType == UserTypes.Manager || userType == UserTypes.SuperAdmin)
                {
                    requests = unitWork.ProjectMembershipRepository.GetManyQueryable(r => r.ProjectId == projectId && r.MembershipType == membershipType);
                }
                else
                {
                    requests = unitWork.ProjectMembershipRepository.GetManyQueryable(r => r.ProjectId == projectId && r.OrganizationId == user.OrganizationId && r.MembershipType == membershipType);
                    if (requests.Count() == 0)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Membership Request");
                        response.Success = false;
                        return response;
                    }
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var approvalRequest = (from r in requests
                                       select r).FirstOrDefault();

                        foreach (var request in requests)
                        {
                            request.IsApproved = true;
                            unitWork.ProjectMembershipRepository.Update(request);
                        }
                        await unitWork.SaveAsync();

                        if (approvalRequest.MembershipType == MembershipTypes.Funder)
                        {
                            var projectFunder = unitWork.ProjectFundersRepository.GetOne(f => f.ProjectId == projectId && f.FunderId == user.OrganizationId);
                            if (projectFunder == null)
                            {
                                unitWork.ProjectFundersRepository.Insert(new EFProjectFunders()
                                {
                                    ProjectId = projectId,
                                    FunderId = user.OrganizationId
                                });
                                await unitWork.SaveAsync();
                            }
                        }
                        else if(approvalRequest.MembershipType == MembershipTypes.Implementer)
                        {
                            var projectImplementer = unitWork.ProjectImplementersRepository.GetOne(i => i.ProjectId == projectId && i.ImplementerId == user.OrganizationId);
                            if (projectImplementer == null)
                            {
                                unitWork.ProjectImplementersRepository.Insert(new EFProjectImplementers()
                                {
                                    ProjectId = projectId,
                                    ImplementerId = user.OrganizationId
                                });
                                await unitWork.SaveAsync();
                            }
                        }
                        transaction.Commit();
                    }
                });

                //Send status email
                string requestedProject = project.Title;
                List<EmailAddress> usersEmailList = new List<EmailAddress>();
                usersEmailList.Add(new EmailAddress()
                {
                    Email = user.Email,
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
                        smtpSettingsModel.SenderName = smtpSettings.SenderName;
                    }

                    string message = "", subject = "", footerMessage = "";
                    var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ProjectPermissionGranted);
                    if (emailMessage != null)
                    {
                        subject = emailMessage.Subject;
                        message = emailMessage.Message;
                        footerMessage = emailMessage.FooterMessage;
                    }

                    mHelper = new MessageHelper();
                    message += mHelper.ProjectPermissionGranted(requestedProject);
                    IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                    emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                }
                return response;
            }
        }

        public ActionResponse UnApproveMembershipRequest(int userId, int projectId, int funderId, int ownerId, MembershipTypes membershipType)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                UserTypes userType = UserTypes.Standard;

                var actionUser = unitWork.UserRepository.GetOne(u => u.Id == ownerId);
                if (actionUser != null)
                {
                    userType = actionUser.UserType;
                }

                if (userType != UserTypes.Manager && userType != UserTypes.SuperAdmin)
                {
                    var isUserOwner = unitWork.ProjectRepository.GetOne(p => (p.Id == projectId && p.CreatedById == ownerId));
                    if (isUserOwner == null)
                    {
                        var isFunderOwner = unitWork.ProjectFundersRepository.GetOne(f => f.FunderId == funderId && f.ProjectId == projectId);
                        var isImplementerOwner = unitWork.ProjectImplementersRepository.GetOne(i => i.ImplementerId == funderId && i.ProjectId == projectId);

                        if (isFunderOwner == null && isImplementerOwner == null)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.GetInvalidFunderApprovalMessage();
                            response.Success = false;
                            return response;
                        }
                    }
                }

                var user = unitWork.UserRepository.GetOne(u => u.Id == userId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }

                var project = unitWork.ProjectRepository.GetOne(p => p.Id == projectId);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project");
                    response.Success = false;
                    return response;
                }

                var isRequestExists = unitWork.ProjectMembershipRepository.GetOne(r => r.ProjectId == projectId && r.UserId == user.Id && r.MembershipType == membershipType);
                if (isRequestExists == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Membership Request");
                    response.Success = false;
                    return response;
                }
                unitWork.ProjectMembershipRepository.Delete(isRequestExists);
                unitWork.Save();

                //Send status email
                string requestedProject = project.Title;
                List<EmailAddress> usersEmailList = new List<EmailAddress>();
                usersEmailList.Add(new EmailAddress()
                {
                    Email = user.Email,
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
                        smtpSettingsModel.SenderName = smtpSettings.SenderName;
                    }

                    string message = "", subject = "", footerMessage = "";
                    var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ProjectPermissionDenied);
                    if (emailMessage != null)
                    {
                        subject = emailMessage.Subject;
                        message = emailMessage.Message;
                        footerMessage = emailMessage.FooterMessage;
                    }

                    mHelper = new MessageHelper();
                    message += mHelper.ProjectPermissionDenied(requestedProject);
                    IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                    emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                }
                return response;
            }
        }
    }
}
