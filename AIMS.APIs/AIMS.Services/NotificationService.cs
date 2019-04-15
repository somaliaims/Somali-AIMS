using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
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
                var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => (n.OrganizationId == organizationId && n.UserType == uType && n.TreatmentId != userId) || (uType == UserTypes.SuperAdmin));
                return mapper.Map<List<NotificationView>>(notifications);
            }
        }

        public IEnumerable<NotificationView> GetForManager()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => (n.UserType == UserTypes.Manager));
                return mapper.Map<List<NotificationView>>(notifications);
            }
        }

        public int GetCount(int userId, UserTypes uType, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var count = unitWork.NotificationsRepository.GetProjectionCount(n => (n.OrganizationId == organizationId && n.UserType == uType && n.TreatmentId != userId) || (uType == UserTypes.SuperAdmin), n => n.Id);
                return count;
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
