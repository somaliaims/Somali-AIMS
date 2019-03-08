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
using Microsoft.EntityFrameworkCore;

namespace AIMS.Services
{
    public interface IReportSubscriptionService
    {
        /// <summary>
        /// Gets all report subscriptions
        /// </summary>
        /// <returns></returns>
        IEnumerable<ReportSubscriptionView> GetAll(int userId);

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
        Task<ActionResponse> AddAsync(int userId, ReportSubscriptionModel model);
    }

    public class ReportSubscriptionService : IReportSubscriptionService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ReportSubscriptionService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ReportSubscriptionView> GetAll(int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var reportSubscriptions = unitWork.ReportSubscriptionRepository.GetManyQueryable(s => s.UserId == userId);
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


        public async Task<ActionResponse> AddAsync(int userId, ReportSubscriptionModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                try
                {
                    var user = unitWork.UserRepository.GetByID(userId);
                    if (user == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("User");
                        response.Success = false;
                        return response;
                    }

                    if (model.ReportIds.Count > 0)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                var reports = await unitWork.ReportsRepository.GetManyQueryableAsync(r => model.ReportIds.Contains(r.Id));
                                var deleteSubscriptions = await unitWork.ReportSubscriptionRepository.GetManyQueryableAsync(s => s.UserId == userId);

                                foreach (var sub in deleteSubscriptions)
                                {
                                    unitWork.ReportSubscriptionRepository.Delete(sub);
                                }
                                await unitWork.SaveAsync();

                                foreach (var report in reports)
                                {

                                    unitWork.ReportSubscriptionRepository.Insert(new EFReportSubscriptions()
                                    {
                                        Report = report,
                                        User = user
                                    });
                                }
                                await unitWork.SaveAsync();
                                transaction.Commit();
                            }
                        });
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
