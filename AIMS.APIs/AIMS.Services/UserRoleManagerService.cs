using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IUserRoleManagerService
    {
        /// <summary>
        /// Adds new request for user role settlement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddRoleSettlementRequest(UserRoleSettlementRequest model);

        ActionResponse ApproveRoleSettlement(UserRoleSettlementRequest model);
    }

    public class UserRoleManagerService
    {
        AIMSDbContext context;
        public UserRoleManagerService(AIMSDbContext dbContext)
        {
            context = dbContext;
        }

        public ActionResponse AddRoleSettlementRequest(UserRoleSettlementRequest model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var user = unitWork.UserRepository.GetByID(model.UserId);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("User");
                    return response;
                }

                var requestExists = unitWork.UserRoleSettlementRequestsRepository.GetOne(r => r.UserId == model.UserId);
                if (requestExists != null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.AlreadyExists("User role settlement request");
                    return response;
                }

                var newRequest = unitWork.UserRoleSettlementRequestsRepository.Insert(new EFUserRoleSettlementRequests()
                {
                    User = user,
                    RequestedUserType = model.RequestedRole,
                    Dated = DateTime.Now,
                    Status = false
                });
                unitWork.Save();
                response.ReturnedId = newRequest.Id;
                return response;
            }
        }
    }
}
