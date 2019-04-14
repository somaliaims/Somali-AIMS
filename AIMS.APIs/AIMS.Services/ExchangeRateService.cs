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
        ActionResponse SaveCurrencyRates(List<CurrencyWithRates> ratesList, DateTime dated);

        /// <summary>
        /// Saves new exchange rates for provided date
        /// </summary>
        /// <param name="ratesList"></param>
        /// <returns></returns>
        ActionResponse SaveCurrencyRatesManual(List<CurrencyWithRates> ratesList);

        /// <summary>
        /// Saves api key for open exchange
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ActionResponse SetAPIKeyForOpenExchange(string key);

        /// <summary>
        /// Gets api key for open exchange
        /// </summary>
        /// <returns></returns>
        string GetAPIKeyForOpenExchange();

        /// <summary>
        /// Sets settings for auto exchange rates
        /// </summary>
        /// <param name="autoExchangeRates"></param>
        /// <returns></returns>
        ActionResponse SetExchangeRatesAutoSettings(bool autoExchangeRates);

        /// <summary>
        /// Gets the latest currency rates list from DB
        /// </summary>
        /// <returns></returns>
        Task<ExchangeRatesView> GetLatestCurrencyRates();

        /// <summary>
        /// Gets the value for exchange rate setting for manual or auto
        /// </summary>
        /// <returns></returns>
        ExchangeRatesSettingsView GetExRateSettings();

        /// <summary>
        /// Gets list of manual exchange rates
        /// </summary>
        /// <returns></returns>
        IEnumerable<CurrencyWithRates> GetManualExchangeRates();

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

        public ActionResponse SaveCurrencyRates(List<CurrencyWithRates> ratesList, DateTime dated)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();

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

        public ActionResponse SetAPIKeyForOpenExchange(string key)
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
                exRateSettings.APIKeyOpenExchangeRates = key;
                unitWork.ExRatesSettingsRepository.Update(exRateSettings);
                unitWork.Save();
                return response;
            }
        }

        public string GetAPIKeyForOpenExchange()
        {
            var unitWork = new UnitOfWork(context);
            string apiKey = "";
            var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(r => r.Id != 0);
            if (exRateSettings != null)
            {
                apiKey = exRateSettings.APIKeyOpenExchangeRates;
            }
            return apiKey;
        }

        public async Task<ExchangeRatesView> GetLatestCurrencyRates()
        {
            var unitWork = new UnitOfWork(context);

            ExchangeRatesView ratesView = new ExchangeRatesView();
            bool isExRateAuto = false;

            try
            {
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(r => r.Id != 0);
                if (exRateSettings != null)
                {
                    isExRateAuto = exRateSettings.IsAutomatic;
                }

                if (isExRateAuto)
                {
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
                else if (exRateSettings != null)
                {
                    string exRatesManual = exRateSettings.ManualExchangeRates;
                    if (!string.IsNullOrEmpty(exRatesManual))
                    {
                        ratesView.Rates = JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exRatesManual);
                    }
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
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
            var unitWork = new UnitOfWork(context);
            //{
                ExchangeRatesView ratesView = new ExchangeRatesView();
                bool isExRateAuto = false;
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(r => r.Id != 0);

                if (exRateSettings != null)
                {
                    isExRateAuto = exRateSettings.IsAutomatic;
                }

                if (isExRateAuto == true)
                {
                    List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
                    var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
                    ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;
                }
                else if (exRateSettings != null)
                {
                    string exRatesManual = exRateSettings.ManualExchangeRates;
                    if (!string.IsNullOrEmpty(exRatesManual))
                    {
                        ratesView.Rates = JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exRatesManual);
                    }
                }
                return await Task<ExchangeRatesView>.Run(() => ratesView).ConfigureAwait(false);
            //}
        }

        public ExchangeRatesSettingsView GetExRateSettings()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(e => e.Id != 0);
                ExchangeRatesSettingsView settingsView = new ExchangeRatesSettingsView();
                if (exRateSettings != null)
                {
                    settingsView.IsAutomatic = exRateSettings.IsAutomatic;
                    settingsView.IsOpenExchangeKeySet = !string.IsNullOrEmpty(exRateSettings.APIKeyOpenExchangeRates) ? true : false;
                    settingsView.ManualCurrencyRates = exRateSettings.ManualExchangeRates == null ? null : JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exRateSettings.ManualExchangeRates);
                }
                return settingsView;
            }
        }

        public IEnumerable<CurrencyWithRates> GetManualExchangeRates()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(e => e.Id != 0);
                List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
                if (exRateSettings != null)
                {
                    ratesList = exRateSettings.ManualExchangeRates == null ? null : JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exRateSettings.ManualExchangeRates);
                }
                return ratesList;
            }
        }

        public ActionResponse SetExchangeRatesAutoSettings(bool autoExchangeRates)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(e => e.Id != 0);
                if (exRateSettings == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Exchange Rate Settings");
                    response.Success = false;
                    return response;
                }
                exRateSettings.IsAutomatic = autoExchangeRates;
                unitWork.ExRatesSettingsRepository.Update(exRateSettings);
                unitWork.Save();
                return response;
            }
        }
    }
}
