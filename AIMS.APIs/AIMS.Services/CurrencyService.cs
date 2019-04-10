using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ICurrencyService
    {
        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <returns></returns>
        IEnumerable<CurrencyView> GetAll();

        /// <summary>
        /// Get matching currencies for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<CurrencyView> GetMatching(string criteria);

        /// <summary>
        /// Gets default currency
        /// </summary>
        /// <returns></returns>
        string GetDefaultCurrency();

        /// <summary>
        /// Gets the currency for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        CurrencyView Get(int id);
        /// <summary>
        /// Gets all currencies async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CurrencyView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(CurrencyModel currency);

        /// <summary>
        /// Sets currency to default
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        ActionResponse SetDefault(int currencyId);

        /// <summary>
        /// Updates a currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        ActionResponse Update(int id, CurrencyModel currency);

        /// <summary>
        /// Deletes a currency
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class CurrencyService : ICurrencyService
    {
        AIMSDbContext context;
        IMapper mapper;

        public CurrencyService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<CurrencyView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var currencys = unitWork.CurrencyRepository.GetAll();
                return mapper.Map<List<CurrencyView>>(currencys);
            }
        }

        public async Task<IEnumerable<CurrencyView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var currencys = await unitWork.CurrencyRepository.GetAllAsync();
                return await Task<IEnumerable<CurrencyView>>.Run(() => mapper.Map<List<CurrencyView>>(currencys)).ConfigureAwait(false);
            }
        }

        public string GetDefaultCurrency()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var defaultCurrency = unitWork.CurrencyRepository.GetOne(c => c.IsDefault == true);
                return defaultCurrency == null ? null : defaultCurrency.Currency;
            }
        }

        public CurrencyView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var currency = unitWork.CurrencyRepository.GetByID(id);
                return mapper.Map<CurrencyView>(currency);
            }
        }

        public IEnumerable<CurrencyView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<CurrencyView> currencysList = new List<CurrencyView>();
                var currencys = unitWork.CurrencyRepository.GetMany(o => o.Currency.Contains(criteria));
                return mapper.Map<List<CurrencyView>>(currencys);
            }
        }

        public ActionResponse Add(CurrencyModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isCurrencyCreated = unitWork.CurrencyRepository.GetOne(l => l.Currency.ToLower() == model.Currency.ToLower());
                    if (isCurrencyCreated != null)
                    {
                        unitWork.CurrencyRepository.Update(isCurrencyCreated);
                        response.ReturnedId = isCurrencyCreated.Id;
                    }
                    else
                    {
                        var newCurrency = unitWork.CurrencyRepository.Insert(new EFCurrency()
                        {
                            Currency = model.Currency,
                            IsDefault = false
                        });
                        unitWork.Save();
                        response.ReturnedId = newCurrency.Id;
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

        public ActionResponse SetDefault(int currencyId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var currencies = unitWork.CurrencyRepository.GetManyQueryable(c => (c.IsDefault == true || c.Id == currencyId));
                var currency = (from c in currencies
                                where c.Id == currencyId
                                select c).FirstOrDefault();

                if (currency == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Currency");
                    return response;
                }

                var defaultCurrencies = (from c in currencies
                                         where c.IsDefault == true
                                         select c);
                
                foreach(var cur in defaultCurrencies)
                {
                    cur.IsDefault = false;
                    unitWork.CurrencyRepository.Update(cur);
                }
                currency.IsDefault = true;
                unitWork.CurrencyRepository.Update(currency);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse Update(int id, CurrencyModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var currencyObj = unitWork.CurrencyRepository.GetByID(id);
                if (currencyObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Currency");
                    return response;
                }

                currencyObj.Currency = model.Currency;
                unitWork.CurrencyRepository.Update(currencyObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var currencyObj = unitWork.CurrencyRepository.GetByID(id);
                if (currencyObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Currency");
                    return response;
                }

                unitWork.CurrencyRepository.Delete(currencyObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }
    }
}
