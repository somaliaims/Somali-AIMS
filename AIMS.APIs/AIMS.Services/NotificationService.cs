using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface INotificationService
    {
        IEnumerable<NotificationView> Get(UserTypes uType, int organizationId);
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

        public IEnumerable<NotificationView> Get(UserTypes uType, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var notifications = unitWork.NotificationsRepository.GetMany(n => n.OrganizationId == organizationId && n.UserType == uType);
                return mapper.Map<List<NotificationView>>(notifications);
            }
        }
    }
}
