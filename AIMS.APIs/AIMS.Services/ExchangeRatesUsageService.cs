using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using AIMS.Services.Helpers;

namespace AIMS.Services
{
    public interface IExchangeRatesUsageService
    {
        /// <summary>
        /// Gets list of exchange rates usage settings
        /// </summary>
        /// <returns></returns>
        IEnumerable<ExchangeRatesUsageView> GetAll();

        /// <summary>
        /// Gets ex rate usage settings for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ExchangeRatesUsageView Get(int id);

        /// <summary>
        /// Adds ex rate usage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse AddOrUpdate(ExchangeRatesUsageModel model);

        /// <summary>
        /// Deletes ex rates usage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(ExchangeRateSources source, ExchangeRateUsageSection usageSection);
    }

    public class ExchangeRatesUsageService : IExchangeRatesUsageService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ExchangeRatesUsageService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ExchangeRatesUsageView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var exRates = unitWork.ExRatesUsageRepository.GetAll();
                return mapper.Map<List<ExchangeRatesUsageView>>(exRates);
            }
        }

        public ExchangeRatesUsageView Get(int id)
        {
            using(var unitWork = new UnitOfWork(context))
            {
                var exRateUsage = unitWork.ExRatesUsageRepository.GetByID(id);
                return mapper.Map<ExchangeRatesUsageView>(exRateUsage);
            }
        }

        public ActionResponse AddOrUpdate(ExchangeRatesUsageModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var exRatesUsageList = unitWork.ExRatesUsageRepository.GetAll();

                var isExRateUsageExists = (from e in exRatesUsageList
                                where e.Source == model.Source && e.UsageSection == model.UsageSection
                                select e).FirstOrDefault();

                var isOrderExists = (from e in exRatesUsageList
                                     where e.Order == model.Order
                                     select e).FirstOrDefault();

                if (isOrderExists != null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetExRateOrderExistsMessage();
                    return response;
                }

                if (isExRateUsageExists != null)
                {
                    isExRateUsageExists.Order = model.Order;
                    unitWork.ExRatesUsageRepository.Update(isExRateUsageExists);
                }
                else
                {
                    unitWork.ExRatesUsageRepository.Insert(new EFExchangeRatesUsageSettings()
                    {
                        Source = model.Source,
                        UsageSection = model.UsageSection,
                        Order = model.Order
                    });
                }
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse Delete(ExchangeRateSources source, ExchangeRateUsageSection usageSection)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var exRateUsageExist = unitWork.ExRatesUsageRepository.GetOne(e => e.Source == source && e.UsageSection == usageSection);
                if (exRateUsageExist == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Exchange rate usage");
                    return response;
                }

                unitWork.ExRatesUsageRepository.Delete(exRateUsageExist);
                return response;
            }
        }

    }
}
