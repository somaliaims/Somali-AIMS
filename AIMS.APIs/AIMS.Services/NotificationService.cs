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
        /// Sends notifications for new data, organizations, sectors
        /// </summary>
        /// <returns></returns>
        ActionResponse SendNotificationsForNewSectors(int newSectors);

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
                int count = unitWork.NotificationsRepository.GetProjectionCount(n => (n.OrganizationId == organizationId && n.UserType == uType && n.TreatmentId != userId) || (uType == UserTypes.SuperAdmin || uType == UserTypes.Manager), n => n.Id);
                var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(p => p.FunderId == organizationId, p => p.FunderId);
                var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ImplementerId == organizationId, p => p.ImplementerId);
                var userOwnedProjects = unitWork.ProjectRepository.GetProjection(p => p.CreatedById == userId, p => p.Id);
                var projectIds = funderProjectIds.Union(implementerProjectIds).ToList<int>();
                List<int> userOwnedProjectIds = new List<int>();
                foreach (var pid in userOwnedProjects)
                {
                    userOwnedProjectIds.Add((int)pid);
                }
                projectIds = projectIds.Union(userOwnedProjectIds).ToList<int>();
                int requestsCount = unitWork.ProjectMembershipRepository.GetProjectionCount(r => projectIds.Contains(r.ProjectId) && r.IsApproved == false, r => r.ProjectId);
                return (count + requestsCount);
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
                        smtpSettings.AdminEmail = smtpSettings.AdminEmail;
                    }

                    IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettingsModel);
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
                        emailHelper.SendEmailToUsers(emailAddresses, "New IATI Sectors", subject, message, footerMessage);
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
