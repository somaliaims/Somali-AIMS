using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AIMS.Services.Helpers;

namespace AIMS.Services
{
    public interface IManualExchangeRatesService
    {
        /// <summary>
        /// Gets list of all manual exchange rates
        /// </summary>
        /// <returns></returns>
        IEnumerable<ManualRatesView> GetAll();

        /// <summary>
        /// Gets the rate for the specified date
        /// </summary>
        /// <returns></returns>
        ManualRatesView GetByDate(DateTime dated);

        /// <summary>
        /// Adds new manual rate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Add(ManualRateModel model);

        /// <summary>
        /// Updates the manual rate
        /// </summary>
        /// <param name="id"></param>
        /// <param name=""></param>
        /// <returns></returns>
        ActionResponse Update(int id, ManualRateModel model);

        /// <summary>
        /// Deletes the manual rate for provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
        
    }

    public class ManualExchangeRatesService : IManualExchangeRatesService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ManualExchangeRatesService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ManualRatesView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var rates = unitWork.ManualRatesRepository.GetAll();
                if (rates != null)
                {
                    rates = (from r in rates
                                  orderby r.Dated descending
                                  select r);
                }
                return mapper.Map<List<ManualRatesView>>(rates);
            }
        }

        public ManualRatesView GetByDate(DateTime dated)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var manualRate = unitWork.ManualRatesRepository.GetOne(r => r.Dated.Date == dated.Date);
                return mapper.Map<ManualRatesView>(manualRate);
            }
        }

        public ActionResponse Add(ManualRateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var manualRate = unitWork.ManualRatesRepository.GetOne(r => r.Dated.Date == model.Dated.Date && r.DefaultCurrency == model.DefaultCurrency
                && r.NationalCurrency == model.NationalCurrency);

                if (manualRate != null)
                {
                    manualRate.ExchangeRate = model.ExchangeRate;
                    unitWork.ManualRatesRepository.Update(manualRate);
                }
                else
                {
                    unitWork.ManualRatesRepository.Insert(new EFManualExchangeRates()
                    {
                        DefaultCurrency = model.DefaultCurrency,
                        NationalCurrency = model.NationalCurrency,
                        Dated = model.Dated,
                        ExchangeRate = model.ExchangeRate
                    });
                }

                try
                {
                    unitWork.Save();
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, ManualRateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var manualRate = unitWork.ManualRatesRepository.GetOne(r => r.Id == id);

                if (manualRate ==  null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Manual exchange rate");
                    return response;
                }

                if (manualRate != null)
                {
                    manualRate.ExchangeRate = model.ExchangeRate;
                    manualRate.DefaultCurrency = model.DefaultCurrency;
                    manualRate.NationalCurrency = model.NationalCurrency;
                    unitWork.ManualRatesRepository.Update(manualRate);
                }

                try
                {
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var manualRate = unitWork.ManualRatesRepository.GetByID(id);
                if (manualRate == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Manual exchange rate");
                    return response;
                }

                unitWork.ManualRatesRepository.Delete(manualRate);
                unitWork.Save();
                return response;
            }
        }
    }
}
