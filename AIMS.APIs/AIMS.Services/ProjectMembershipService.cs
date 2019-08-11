﻿using AIMS.DAL.EF;
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
        IEnumerable<ProjectMembershipRequestView> GetRequestsForFunder(int funderId, int userId);

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
        ActionResponse ApproveMembershipRequest(int userId, int projectId, int funderId, int ownerId);

        /// <summary>
        /// Un-Approves membership request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse UnApproveMembershipRequest(int userId, int projectId, int funderId, int ownerId);
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

        public IEnumerable<ProjectMembershipRequestView> GetRequestsForFunder(int funderId, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(p => p.FunderId == funderId, p => p.FunderId);
                var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ImplementerId == funderId, p => p.ImplementerId);
                var userOwnedProjects = unitWork.ProjectRepository.GetProjection(p => p.CreatedById == userId, p => p.Id);
                var projectIds = funderProjectIds.Union(implementerProjectIds).ToList<int>();
                List<int> userOwnedProjectIds = new List<int>(); 
                foreach(var pid in userOwnedProjects)
                {
                    userOwnedProjectIds.Add((int)pid);
                }
                projectIds = projectIds.Union(userOwnedProjectIds).ToList<int>();
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
                            OrganizationId = user.OrganizationId,
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
                        message += mHelper.NewOrganizationForProject(requestingOrganization);
                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
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

        public ActionResponse ApproveMembershipRequest(int userId, int projectId, int funderId, int ownerId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

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

                var user = unitWork.UserRepository.GetOne(u => u.Id == userId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }

                var project = unitWork.ProjectRepository.GetOne(p => p.Id == projectId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project");
                    response.Success = false;
                    return response;
                }

                var requests = unitWork.ProjectMembershipRepository.GetManyQueryable(r => r.ProjectId == projectId && r.OrganizationId == user.OrganizationId);
                if (requests == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Membership Request");
                    response.Success = false;
                    return response;
                }

                foreach(var request in requests)
                {
                    request.IsApproved = true;
                    unitWork.ProjectMembershipRepository.Update(request);
                }
                
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
                    IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                    emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                }
                return response;
            }
        }

        public ActionResponse UnApproveMembershipRequest(int userId, int projectId, int funderId, int ownerId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

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

                var isRequestExists = unitWork.ProjectMembershipRepository.GetOne(r => r.ProjectId == projectId && r.UserId == user.Id);
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
                    IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettingsModel);
                    emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                }
                return response;
            }
        }
    }
}
