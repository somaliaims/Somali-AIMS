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
    public interface IProjectDeletionService
    {
        /// <summary>
        /// Adds new request for project deletion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddRequest(ProjectDeletionRequestModel model);

        /// <summary>
        /// Gets list of project deletion requests
        /// </summary>
        /// <returns></returns>
        ICollection<ProjectDeletionRequestView> GetDeletionRequests(UserTypes uType, int userId, int orgId);

        /// <summary>
        /// Gets list of project ids only that are active for deletion request
        /// </summary>
        /// <returns></returns>
        ICollection<int> GetActiveProjectIds();

        /// <summary>
        /// Cancels deletion request
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        ActionResponse CancelRequest(int projectId, int userId);

        /// <summary>
        /// Approve deletion request
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ActionResponse ApproveRequest(int projectId, int userId);

        /// <summary>
        /// Deletes a project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ActionResponse DeleteProject(int projectId, int userId);
    }

    public class ProjectDeletionService : IProjectDeletionService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectDeletionService(AIMSDbContext cntxt, IMapper _mapper)
        {
            context = cntxt;
            mapper = _mapper;
        }

        public ICollection<ProjectDeletionRequestView> GetDeletionRequests(UserTypes uType, int userId, int orgId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IEnumerable<EFProjectDeletionRequests> projectRequests = null;
                if (uType == UserTypes.Standard)
                {
                    var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(p => p.FunderId == orgId, p => p.ProjectId);
                    var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ImplementerId == orgId, p => p.ProjectId);
                    var userOwnedProjects = unitWork.ProjectRepository.GetProjection(p => p.CreatedById == userId, p => p.Id);
                    var projectIds = funderProjectIds.Union(implementerProjectIds).ToList<int>();
                    List<int> userOwnedProjectIds = new List<int>();
                    foreach (var pid in userOwnedProjects)
                    {
                        userOwnedProjectIds.Add((int)pid);
                    }
                    projectIds = projectIds.Union(userOwnedProjectIds).ToList<int>();
                    projectRequests = unitWork.ProjectDeletionRepository.GetWithInclude(p => (p.Status == ProjectDeletionStatus.Requested && p.UserId != userId && projectIds.Contains(p.ProjectId)), new string[] { "RequestedBy", "Project", "RequestedBy.Organization" });
                }
                else if (uType == UserTypes.Manager || uType == UserTypes.SuperAdmin)
                {
                    projectRequests = unitWork.ProjectDeletionRepository.GetWithInclude(p => (p.Status == ProjectDeletionStatus.Approved || (p.Status == ProjectDeletionStatus.Requested && p.RequestedOn <= DateTime.Now.AddDays(-7))), new string[] { "RequestedBy", "Project", "RequestedBy.Organization" });
                }
                return mapper.Map<List<ProjectDeletionRequestView>>(projectRequests);
            }
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

                var user = unitWork.UserRepository.GetOne(u => u.Id == model.UserId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("User");
                    return response;
                }

                var isRequestExists = unitWork.ProjectDeletionRepository.GetOne(p => p.ProjectId == project.Id);
                if (isRequestExists != null && isRequestExists.Status == ProjectDeletionStatus.Requested)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetProjectDeletionExistsMessage();
                    return response;
                }

                if (isRequestExists != null && isRequestExists.Status == ProjectDeletionStatus.Approved)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetProjectDeletionApprovedMessage();
                    return response;
                }

                if (isRequestExists != null && isRequestExists.Status == ProjectDeletionStatus.Cancelled)
                {
                    isRequestExists.Status = ProjectDeletionStatus.Requested;
                    isRequestExists.RequestedOn = DateTime.Now;
                    isRequestExists.StatusUpdatedOn = DateTime.Now;
                    isRequestExists.RequestedBy = user;
                    unitWork.ProjectDeletionRepository.Update(isRequestExists);
                }
                else
                {
                    unitWork.ProjectDeletionRepository.Insert(new EFProjectDeletionRequests()
                    {
                        Project = project,
                        RequestedBy = user,
                        RequestedOn = DateTime.Now,
                        StatusUpdatedOn = DateTime.Now,
                        Status = ProjectDeletionStatus.Requested
                    });
                }
                unitWork.Save();

                var projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(f => f.ProjectId == project.Id, f => f.FunderId);
                var projectImplementerIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ProjectId == project.Id, i => i.ImplementerId);
                var orgIds = projectFunderIds.Union(projectImplementerIds).ToList();
                if (!orgIds.Contains(user.OrganizationId))
                {
                    orgIds.Add(user.OrganizationId);
                }

                var userEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Standard && orgIds.Contains(u.OrganizationId), u => u.Email);
                var adminEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Manager, u => u.Email);
                var allEmails = userEmails.Union(adminEmails);

                List<EmailAddress> usersEmailList = new List<EmailAddress>();
                foreach(var email in allEmails)
                {
                    usersEmailList.Add(new EmailAddress() { Email = email });
                }

                if (usersEmailList.Count > 0)
                {
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

                    string message = "", subject = "", footerMessage = "";
                    string projectTitle = "<h5>Project title: " + project.Title + "</h5>";
                    var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ProjectDeletionRequest);
                    if (emailMessage != null)
                    {
                        subject = emailMessage.Subject;
                        message = projectTitle + emailMessage.Message;
                        footerMessage = emailMessage.FooterMessage;
                    }
                    IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                    emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                }
                return response;
            }
        }

        public ICollection<int> GetActiveProjectIds()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectIds = unitWork.ProjectDeletionRepository.GetProjection(p => p.Status == ProjectDeletionStatus.Requested, p => p.ProjectId);
                return new List<int>(projectIds);
            }
        }

        public ActionResponse ApproveRequest(int projectId, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var project = unitWork.ProjectRepository.GetByID(projectId);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project");
                    return response;
                }

                var userOrganizationsList = unitWork.UserRepository.GetProjection(u => u.Id == userId, u => u.OrganizationId);
                var userOrganizationId = (from orgId in userOrganizationsList
                                          select orgId).FirstOrDefault();

                int projectOrganizationId = 0;
                EFProjectDeletionRequests deletionRequest = null;
                var deletionRequestArr = unitWork.ProjectDeletionRepository.GetWithInclude(d => d.ProjectId == projectId, new string[] { "RequestedBy" });
                foreach (var delRequest in deletionRequestArr)
                {
                    projectOrganizationId = delRequest.RequestedBy.OrganizationId;
                    deletionRequest = delRequest;
                }

                bool canEditProject = false;
                if (projectOrganizationId == userOrganizationId)
                {
                    canEditProject = true;
                }
                else
                {
                    var funderProjectsIds = unitWork.ProjectFundersRepository.GetProjection(f => f.FunderId == userOrganizationId, f => f.ProjectId).ToList();
                    var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ImplementerId == userOrganizationId, i => i.ProjectId).ToList();
                    var membershipProjectIds = unitWork.ProjectMembershipRepository.GetProjection(m => (m.UserId == userId && m.IsApproved == true), m => m.ProjectId);
                    var combinedProjectIds = funderProjectsIds.Union(implementerProjectIds);
                    combinedProjectIds = combinedProjectIds.Union(membershipProjectIds);
                    if (!combinedProjectIds.Contains(projectId))
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetInvalidProjectEdit();
                        return response;
                    }
                    canEditProject = true;
                }

                if (canEditProject)
                {
                    deletionRequest.Status = ProjectDeletionStatus.Approved;
                    unitWork.ProjectDeletionRepository.Update(deletionRequest);
                    unitWork.Save();

                    var projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(f => f.ProjectId == project.Id, f => f.FunderId);
                    var projectImplementerIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ProjectId == project.Id, i => i.ImplementerId);
                    var orgIds = projectFunderIds.Union(projectImplementerIds).ToList();
                    if (!orgIds.Contains(userOrganizationId))
                    {
                        orgIds.Add(userOrganizationId);
                    }

                    var userEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Standard && orgIds.Contains(u.OrganizationId), u => u.Email);
                    var adminEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Manager, u => u.Email);
                    var allEmails = userEmails.Union(adminEmails);

                    List<EmailAddress> usersEmailList = new List<EmailAddress>();
                    foreach (var email in allEmails)
                    {
                        usersEmailList.Add(new EmailAddress() { Email = email });
                    }

                    if (usersEmailList.Count > 0)
                    {
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

                        string message = "", subject = "", footerMessage = "";
                        string projectTitle = "<h5>Project title: " + project.Title + "</h5>";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ProjectDeletionApproved);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = projectTitle + emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }
                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                        emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                    }
                }
                return response;
            }
        }

        public ActionResponse CancelRequest(int projectId, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var project = unitWork.ProjectRepository.GetByID(projectId);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project");
                    return response;
                }

                var userOrganizationsList = unitWork.UserRepository.GetProjection(u => u.Id == userId, u => u.OrganizationId);
                var userOrganizationId = (from orgId in userOrganizationsList
                                          select orgId).FirstOrDefault();

                int projectOrganizationId = 0;
                EFProjectDeletionRequests deletionRequest = null;
                var deletionRequestArr = unitWork.ProjectDeletionRepository.GetWithInclude(d => d.ProjectId == projectId, new string[] { "RequestedBy" });
                foreach(var delRequest in deletionRequestArr)
                {
                    projectOrganizationId = delRequest.RequestedBy.OrganizationId;
                    deletionRequest = delRequest;
                }

                bool canEditProject = false;
                if (projectOrganizationId == userOrganizationId)
                {
                    canEditProject = true;
                }
                else
                {
                    var funderProjectsIds = unitWork.ProjectFundersRepository.GetProjection(f => f.FunderId == userOrganizationId, f => f.ProjectId).ToList();
                    var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ImplementerId == userOrganizationId, i => i.ProjectId).ToList();
                    var membershipProjectIds = unitWork.ProjectMembershipRepository.GetProjection(m => (m.UserId == userId && m.IsApproved == true), m => m.ProjectId);
                    var combinedProjectIds = funderProjectsIds.Union(implementerProjectIds);
                    combinedProjectIds = combinedProjectIds.Union(membershipProjectIds);
                    if (!combinedProjectIds.Contains(projectId))
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetInvalidProjectEdit();
                        return response;
                    }
                    canEditProject = true;
                }

                if (canEditProject)
                {
                    deletionRequest.Status = ProjectDeletionStatus.Cancelled;
                    unitWork.ProjectDeletionRepository.Update(deletionRequest);
                    unitWork.Save();

                    var projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(f => f.ProjectId == project.Id, f => f.FunderId);
                    var projectImplementerIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ProjectId == project.Id, i => i.ImplementerId);
                    var orgIds = projectFunderIds.Union(projectImplementerIds).ToList();
                    if (!orgIds.Contains(userOrganizationId))
                    {
                        orgIds.Add(userOrganizationId);
                    }

                    var userEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Standard && orgIds.Contains(u.OrganizationId), u => u.Email);
                    var adminEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Manager, u => u.Email);
                    var allEmails = userEmails.Union(adminEmails);

                    List<EmailAddress> usersEmailList = new List<EmailAddress>();
                    foreach (var email in allEmails)
                    {
                        usersEmailList.Add(new EmailAddress() { Email = email });
                    }

                    if (usersEmailList.Count > 0)
                    {
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

                        string message = "", subject = "", footerMessage = "";
                        string projectTitle = "<h5>Project title: " + project.Title + "</h5>";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ProjectDeletionCancelled);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = projectTitle + emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }
                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                        emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                    }
                }
                return response;
            }
        }

        public ActionResponse DeleteProject(int projectId, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var user = unitWork.UserRepository.GetOne(u => (u.UserType == UserTypes.Manager || u.UserType == UserTypes.SuperAdmin) && u.Id == userId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetInvalidAccountForProject();
                    response.Success = false;
                    return response;
                }

                var project = unitWork.ProjectRepository.GetByID(projectId);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project");
                    response.Success = false;
                    return response;
                }

                unitWork.ProjectRepository.Delete(project);
                unitWork.Save();

                int userOrganizationId = 0;
                var deletionRequest = unitWork.ProjectDeletionRepository.GetWithInclude(p => p.ProjectId == projectId, new string[] { "RequestedBy" }).FirstOrDefault();
                if (deletionRequest != null)
                {
                    userOrganizationId = deletionRequest.RequestedBy.OrganizationId;
                }
                var projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(f => f.ProjectId == project.Id, f => f.FunderId);
                var projectImplementerIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ProjectId == project.Id, i => i.ImplementerId);
                var orgIds = projectFunderIds.Union(projectImplementerIds).ToList();
                if (!orgIds.Contains(userOrganizationId) && userOrganizationId != 0)
                {
                    orgIds.Add(userOrganizationId);
                }

                var userEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Standard && orgIds.Contains(u.OrganizationId), u => u.Email);
                var adminEmails = unitWork.UserRepository.GetProjection(u => u.UserType == UserTypes.Manager, u => u.Email);
                var allEmails = userEmails.Union(adminEmails);

                List<EmailAddress> usersEmailList = new List<EmailAddress>();
                foreach (var email in allEmails)
                {
                    usersEmailList.Add(new EmailAddress() { Email = email });
                }

                if (usersEmailList.Count > 0)
                {
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

                    string message = "", subject = "", footerMessage = "";
                    var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ProjectDeleted);
                    if (emailMessage != null)
                    {
                        subject = emailMessage.Subject;
                        message = emailMessage.Message;
                        footerMessage = emailMessage.FooterMessage;
                    }
                    mHelper = new MessageHelper();
                    message += mHelper.ProjectDeletionMessage(project.Title);
                    IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                    emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                }
                return response;
            }
        }
    }
}
