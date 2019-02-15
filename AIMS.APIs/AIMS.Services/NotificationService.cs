using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
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
        /// Get notifications count for the logged in user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uType"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        int GetCount(int userId, UserTypes uType, int organizationId);

        /// <summary>
        /// Sets notifications as read
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse MarkNotificationsRead(NotificationUpdateModel model);

        /// <summary>
        /// Deletes notifications
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse DeleteNotifications(NotificationUpdateModel model);
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

        public int GetCount(int userId, UserTypes uType, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var count = unitWork.NotificationsRepository.GetProjectionCount(n => (n.OrganizationId == organizationId && n.UserType == uType && n.TreatmentId != userId) || (uType == UserTypes.SuperAdmin), n => n.Id);
                return count;
            }
        }

        public ActionResponse MarkNotificationsRead(NotificationUpdateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();

                try
                {
                    var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => model.Ids.Contains(n.Id));
                    if (notifications != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            foreach (var notification in notifications)
                            {
                                notification.IsSeen = true;
                                unitWork.NotificationsRepository.Update(notification);
                            }

                            unitWork.Save();
                            scope.Complete();
                        }
                    }
                    response.ReturnedId = model.Ids.Count;
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse DeleteNotifications(NotificationUpdateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => model.Ids.Contains(n.Id));
                    if (notifications != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            foreach (var notification in notifications)
                            {
                                unitWork.NotificationsRepository.Delete(notification);
                            }

                            unitWork.Save();
                            scope.Complete();
                        }
                    }
                    response.ReturnedId = model.Ids.Count;
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }
    }
}
