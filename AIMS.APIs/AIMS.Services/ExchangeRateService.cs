using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IExchangeRateService
    {
        /// <summary>
        /// Saves new exchange rates for current date
        /// </summary>
        /// <param name="ratesJson"></param>
        /// <returns></returns>
        ActionResponse SaveCurrencyRates(List<CurrencyWithRates> ratesList);

        /// <summary>
        /// Saves new exchange rates for provided date
        /// </summary>
        /// <param name="ratesList"></param>
        /// <returns></returns>
        ActionResponse SaveCurrencyRatesManual(List<CurrencyWithRates> ratesList);

        /// <summary>
        /// Gets the latest currency rates list from DB
        /// </summary>
        /// <returns></returns>
        Task<ExchangeRatesView> GetLatestCurrencyRates();

        /// <summary>
        /// Get the currency rates for the specified date
        /// </summary>
        /// <param name="dated"></param>
        /// <returns></returns>
        Task<ExchangeRatesView> GetCurrencyRatesForDate(DateTime dated);

        /// <summary>
        /// Get apis calls count for current month
        /// </summary>
        /// <returns></returns>
        int GetAPIsCallsCount();
    }

    public class ExchangeRateService : IExchangeRateService
    {
        AIMSDbContext context;

        public ExchangeRateService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public ActionResponse SaveCurrencyRates(List<CurrencyWithRates> ratesList)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                DateTime dated = DateTime.Now;

                if (ratesList.Count > 0)
                {
                    string ratesJson = JsonConvert.SerializeObject(ratesList);
                    var exchangeRate = unitWork.ExchangeRatesRepository.GetOne(e => e.Dated.Date == dated.Date);
                    if (exchangeRate == null)
                    {
                        unitWork.ExchangeRatesRepository.Insert(new EFExchangeRates()
                        {
                            ExchangeRatesJson = ratesJson,
                            Dated = dated
                        });
                        unitWork.Save();
                    }

                    var apisCountObj = unitWork.ExchangeRatesAPIsRepository.GetOne(a => (a.Dated.Year == dated.Year && a.Dated.Month == dated.Month));
                    if (apisCountObj != null)
                    {
                        apisCountObj.Count++;
                        unitWork.ExchangeRatesAPIsRepository.Update(apisCountObj);
                    }
                    else
                    {
                        unitWork.ExchangeRatesAPIsRepository.Insert(new EFExchangeRatesAPIsCount()
                        {
                            Count = 1,
                            Dated = dated
                        });
                    }
                    unitWork.Save();
                }
                return response;
            }
        }

        public ActionResponse SaveCurrencyRatesManual(List<CurrencyWithRates> ratesList)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper msgHelper;
                ActionResponse response = new ActionResponse();
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(r => r.Id != 0);
                if (exRateSettings == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("Exchange Rates");
                    return response;
                }
                exRateSettings.ManualExchangeRates = JsonConvert.SerializeObject(ratesList);
                unitWork.ExRatesSettingsRepository.Update(exRateSettings);
                unitWork.Save();
                return response;
            }
        }

        public async Task<ExchangeRatesView> GetLatestCurrencyRates()
        {
            var unitWork = new UnitOfWork(context);
            var defaultCurrency = unitWork.CurrencyRepository.GetOne(c => c.IsDefault == true);
            ExchangeRatesView ratesView = new ExchangeRatesView();

            if (defaultCurrency != null)
            {
                ratesView.Base = defaultCurrency.Currency;
                List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
                DateTime dated = DateTime.Now;
                ratesView.Dated = dated.Date.ToString();
                var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
                ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;

                if (ratesView.Rates != null)
                {
                    int count = ratesView.Rates.Count;
                    var currencies = unitWork.CurrencyRepository.GetManyQueryable(c => c.Id != 0);
                    if (currencies.Count() < count)
                    {
                        var currencyNames = (from c in currencies
                                             select c.Currency).ToList<string>();

                        var currenciesFromAPI = ratesView.Rates;
                        List<EFCurrency> newCurrencies = new List<EFCurrency>();
                        foreach (var currency in currenciesFromAPI)
                        {
                            if (!currencyNames.Contains(currency.Currency))
                            {
                                newCurrencies.Add(new EFCurrency()
                                {
                                    Currency = currency.Currency
                                });
                            }
                        }
                        if (newCurrencies.Count > 0)
                        {
                            unitWork.CurrencyRepository.InsertMultiple(newCurrencies);
                            unitWork.Save();
                        }
                    }
                }
            }
            return await Task<ExchangeRatesView>.Run(() => ratesView).ConfigureAwait(false);
        }

        public int GetAPIsCallsCount()
        {
            int count = 0;
            var unitWork = new UnitOfWork(context);
            DateTime dated = DateTime.Now;
            var apisCountObj = unitWork.ExchangeRatesAPIsRepository.GetOne(a => (a.Dated.Year == dated.Year && a.Dated.Month == dated.Month));
            if (apisCountObj != null)
            {
                count = apisCountObj.Count;
            }
            return count;
        }

        public async Task<ExchangeRatesView> GetCurrencyRatesForDate(DateTime dated)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var defaultCurrency = unitWork.CurrencyRepository.GetOne(c => c.IsDefault == true);
                ExchangeRatesView ratesView = new ExchangeRatesView();

                if (defaultCurrency != null)
                {
                    ratesView.Base = defaultCurrency.Currency;
                    List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
                    var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
                    ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;
                }
                return await Task<ExchangeRatesView>.Run(() => ratesView).ConfigureAwait(false);
            }
        }
    }
}
