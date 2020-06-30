using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.SqlServer.Management.Smo;

namespace AIMS.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Gets notifications for the logged in user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uType"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<NotificationView> Get(int userId, UserTypes uType, int organizationId);

        /// <summary>
        /// Gets list of notifications for manager
        /// </summary>
        /// <returns></returns>
        IEnumerable<NotificationView> GetForManager();

        /// <summary>
        /// Get notifications count for the logged in user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uType"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        int GetCount(int userId, UserTypes uType, int organizationId);

        /// <summary>
        /// Adds new notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Add(NotificationModel model);

        /// <summary>
        /// Sends notifications for new IATI data for sectors
        /// </summary>
        /// <returns></returns>
        ActionResponse SendNotificationsForNewSectors(int newSectors);

        /// <summary>
        /// Sends notifications for new iati data for organizations
        /// </summary>
        /// <param name="newOrganizations"></param>
        /// <param name="orgnazationsWithoutType"></param>
        /// <returns></returns>
        ActionResponse SendNotificationsForNewOrganizations(int newOrganizations, int orgnazationsWithoutType);

        /// <summary>
        /// Sets notifications as read
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> MarkNotificationsReadAsync(NotificationUpdateModel model);

        /// <summary>
        /// Deletes notifications
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteNotificationsAsync(NotificationUpdateModel model);
    }

    public class NotificationService : INotificationService
    {
        AIMSDbContext context;
        IMapper mapper;

        public NotificationService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<NotificationView> Get(int userId, UserTypes uType, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var notifications = unitWork.NotificationsRepository.GetWithInclude(n => (n.OrganizationId == organizationId && n.UserType == uType && n.TreatmentId != userId) || (uType == UserTypes.SuperAdmin || uType == UserTypes.Manager), new string[] { "Organization" });
                return mapper.Map<List<NotificationView>>(notifications);
            }
        }

        public IEnumerable<NotificationView> GetForManager()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var notifications = unitWork.NotificationsRepository.GetWithInclude(n => (n.UserType == UserTypes.Manager), new string[] { "Organization" });
                return mapper.Map<List<NotificationView>>(notifications);
            }
        }

        public int GetCount(int userId, UserTypes uType, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                int deletionsCount = 0;
                int count = unitWork.NotificationsRepository.GetProjection(n => (n.OrganizationId == organizationId && n.UserType == uType && n.TreatmentId != userId) || (uType == UserTypes.SuperAdmin || uType == UserTypes.Manager), n => n.Id).Count();
                var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(p => p.FunderId == organizationId, p => p.ProjectId);
                var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ImplementerId == organizationId, p => p.ProjectId);
                var userOwnedProjects = unitWork.ProjectRepository.GetProjection(p => p.CreatedById == userId, p => p.Id);
                var projectIds = funderProjectIds.Union(implementerProjectIds).ToList<int>();
                List<int> userOwnedProjectIds = new List<int>();
                foreach (var pid in userOwnedProjects)
                {
                    userOwnedProjectIds.Add((int)pid);
                }
                projectIds = projectIds.Union(userOwnedProjectIds).ToList<int>();
                int projectMembershipRequestsCount = 0;
                if (uType == UserTypes.Manager || uType == UserTypes.SuperAdmin)
                {
                    projectMembershipRequestsCount = unitWork.ProjectMembershipRepository.GetProjectionCount(r => r.UserId != userId && r.IsApproved == false, r => r.ProjectId);
                }
                else
                {
                    projectMembershipRequestsCount = unitWork.ProjectMembershipRepository.GetProjectionCount(r => projectIds.Contains(r.ProjectId) && r.UserId != userId && r.IsApproved == false, r => r.ProjectId);
                }
                
                if (uType == UserTypes.Standard)
                {
                    deletionsCount = unitWork.ProjectDeletionRepository.GetProjectionCount(d => (d.Status == ProjectDeletionStatus.Requested && d.RequestedById != userId && projectIds.Contains(d.ProjectId)), d => d.ProjectId);
                }
                else if (uType == UserTypes.Manager || uType == UserTypes.SuperAdmin)
                {
                    //deletionsCount = unitWork.ProjectDeletionRepository.GetProjectionCount(d => (d.Status == ProjectDeletionStatus.Approved || (d.RequestedOn <= DateTime.Now.AddDays(-7) && d.Status == ProjectDeletionStatus.Requested && d.RequestedById != userId) || (projectIds.Contains(d.ProjectId) && userId != d.RequestedById)), d => d.ProjectId);
                    deletionsCount = unitWork.ProjectDeletionRepository.GetProjectionCount(d => (userId != d.RequestedById), d => d.ProjectId);
                }
                var orgsInRequests = unitWork.OrganizationMergeRequestsRepository.GetProjection(r => r.RequestedById != userId, r => r.Id);
                var userRelatedRequests = unitWork.OrganizationsToMergeRepository.GetProjection(o => orgsInRequests.Contains(o.RequestId) && o.OrganizationId == organizationId, o => o.OrganizationId);
                return (count + projectMembershipRequestsCount + deletionsCount + userRelatedRequests.Count());
            }
        }

        public ActionResponse Add(NotificationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var organization = unitWork.OrganizationRepository.GetByID(model.OrganizationId);
                if (organization == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization");
                    response.Success = false;
                    return response;
                }

                unitWork.NotificationsRepository.Insert(new EFUserNotifications()
                {
                    UserType = model.UserType,
                    Organization = organization,
                    Message = model.Message,
                    TreatmentId = model.TreatmentId,
                    Dated = DateTime.Now,
                    IsSeen = false,
                    NotificationType = model.NotificationType
                });
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse SendNotificationsForNewSectors(int newSectors)
        {
            var unitWork = new UnitOfWork(context);
                ActionResponse response = new ActionResponse();
                var messages = unitWork.EmailMessagesRepository.GetAll();

                if (newSectors > 0)
                {
                    string subject = "", message = "", footerMessage = "";
                    var emailMessage = (from m in messages
                                      where m.MessageType == EmailMessageType.NewIATISector
                                      select m).FirstOrDefault();
                if (emailMessage != null)
                {
                    subject = emailMessage.Subject;
                    message = emailMessage.Message;
                    footerMessage = emailMessage.FooterMessage;
                }

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

                    IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                    var users = unitWork.UserRepository.GetManyQueryable(u => u.UserType == UserTypes.Manager);
                    var emails = (from u in users
                                  select u.Email);
                    List<EmailAddress> emailAddresses = new List<EmailAddress>();
                    foreach(var email in emails)
                    {
                        emailAddresses.Add(new EmailAddress()
                        {
                            Email = email
                        });
                    }

                    IMessageHelper mHelper = new MessageHelper();
                    message += mHelper.NewIATISectorsAdded(newSectors);
                    if (emailAddresses.Count > 0)
                    {
                        emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                    }
                }
                return response;
        }

        public ActionResponse SendNotificationsForNewOrganizations(int newOrganizations, int organizationsWithoutType)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            var messages = unitWork.EmailMessagesRepository.GetAll();

            if (newOrganizations > 0)
            {
                string subject = "", message = "", footerMessage = "";
                var emailMessage = (from m in messages
                                    where m.MessageType == EmailMessageType.NewIATIOrganization
                                    select m).FirstOrDefault();
                if (emailMessage != null)
                {
                    subject = emailMessage.Subject;
                    message = emailMessage.Message;
                    footerMessage = emailMessage.FooterMessage;
                }

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

                IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                var users = unitWork.UserRepository.GetManyQueryable(u => u.UserType == UserTypes.Manager);
                var emails = (from u in users
                              select u.Email);
                List<EmailAddress> emailAddresses = new List<EmailAddress>();
                foreach (var email in emails)
                {
                    emailAddresses.Add(new EmailAddress()
                    {
                        Email = email
                    });
                }

                IMessageHelper mHelper = new MessageHelper();
                message += mHelper.NewIATIOrganizationsMessage(newOrganizations, organizationsWithoutType);
                if (emailAddresses.Count > 0)
                {
                    emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                }
            }
            return response;
        }

        public async Task<ActionResponse> MarkNotificationsReadAsync(NotificationUpdateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();

                try
                {
                    var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => model.Ids.Contains(n.Id));
                    if (notifications != null)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                foreach (var notification in notifications)
                                {
                                    notification.IsSeen = true;
                                    unitWork.NotificationsRepository.Update(notification);
                                }
                                await unitWork.SaveAsync();
                                transaction.Commit();
                            }
                        });
                    }
                    response.ReturnedId = model.Ids.Count;
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> DeleteNotificationsAsync(NotificationUpdateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => model.Ids.Contains(n.Id));
                    if (notifications != null)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                foreach (var notification in notifications)
                                {
                                    unitWork.NotificationsRepository.Delete(notification);
                                }
                                await unitWork.SaveAsync();
                                transaction.Commit();
                            }
                        });
                    }
                    response.ReturnedId = model.Ids.Count;
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
