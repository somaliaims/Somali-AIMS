using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Linq;

namespace AIMS.Services
{
    public interface IReportSubscriptionService
    {
        /// <summary>
        /// Gets all report subscriptions
        /// </summary>
        /// <returns></returns>
        IEnumerable<ReportSubscriptionView> GetAll();

        /// <summary>
        /// Gets the report subscription for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        ReportSubscriptionView Get(int id);
        /// <summary>
        /// Gets all report subscriptions async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ReportSubscriptionView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ReportSubscriptionModel model);

        /// <summary>
        /// Updates a report subscription
        /// </summary>
        /// <param name="report subscription"></param>
        /// <returns></returns>
        ActionResponse Remove(ReportSubscriptionModel model);
    }

    public class ReportSubscriptionService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ReportSubscriptionService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ReportSubscriptionView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var reportSubscriptions = unitWork.ReportSubscriptionRepository.GetAll();
                return mapper.Map<List<ReportSubscriptionView>>(reportSubscriptions);
            }
        }

        public async Task<IEnumerable<ReportSubscriptionView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var reportSubscriptions = await unitWork.ReportSubscriptionRepository.GetAllAsync();
                return await Task<IEnumerable<ReportSubscriptionView>>.Run(() => mapper.Map<List<ReportSubscriptionView>>(reportSubscriptions)).ConfigureAwait(false);
            }
        }

        public ReportSubscriptionView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var reportSubscription = unitWork.ReportSubscriptionRepository.GetByID(id);
                return mapper.Map<ReportSubscriptionView>(reportSubscription);
            }
        }


        public ActionResponse Add(ReportSubscriptionModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                try
                {
                    var user = unitWork.UserRepository.GetByID(model.UserId);
                    if (user == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("User");
                        response.Success = false;
                        return response;
                    }

                    if (model.ReportIds.Count > 0)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var subscriptions = unitWork.ReportSubscriptionRepository.GetManyQueryable(s => s.UserId == model.UserId);
                            var reports = unitWork.ReportsRepository.GetManyQueryable(r => model.ReportIds.Contains(r.Id));
                            foreach(var report in reports)
                            {
                                var subscription = from sub in subscriptions
                                                   where sub.ReportId.Equals(report.Id)
                                                   select sub;

                                if (subscription != null)
                                {
                                    unitWork.ReportSubscriptionRepository.Insert(new EFReportSubscriptions()
                                    {
                                        Report = report,
                                        User = user
                                    });
                                }
                            }
                            scope.Complete();
                            unitWork.Save();
                        }
                            
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Remove(ReportSubscriptionModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                try
                {
                    var user = unitWork.UserRepository.GetByID(model.UserId);
                    if (user == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("User");
                        response.Success = false;
                        return response;
                    }

                    if (model.ReportIds.Count > 0)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var subscriptions = unitWork.ReportSubscriptionRepository.GetManyQueryable(s => model.ReportIds.Contains(s.ReportId) && s.UserId == model.UserId);

                            if (subscriptions != null)
                            {
                                foreach(var subscription in subscriptions)
                                {
                                    unitWork.ReportSubscriptionRepository.Delete(subscription);
                                }
                                scope.Complete();
                                unitWork.Save();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }
    }
}
