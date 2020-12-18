using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using Microsoft.EntityFrameworkCore;
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
        Task<ActionResponse> SaveCurrencyRatesAsync(List<CurrencyWithRates> ratesList, DateTime dated);

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
        /// Sets label for manual ex rate source
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        ActionResponse SetLabelForManualExRates(string label);

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
        /// Gets an average rate for the provided currency and date
        /// </summary>
        /// <param name="dated"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        Task<IEnumerable<CurrencyWithRates>> GetAverageCurrencyRatesForDate(DateTime dated);

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

        public async Task<ActionResponse> SaveCurrencyRatesAsync(List<CurrencyWithRates> ratesList, DateTime dated)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();

            if (ratesList.Count > 0)
            {
                string ratesJson = JsonConvert.SerializeObject(ratesList);

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
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
                        int year = dated.Year;
                        var exchangeRates = unitWork.ManualRatesRepository.GetManyQueryable(r => r.Year == year);
                        string defaultCurrency = unitWork.CurrencyRepository.GetProjection(c => c.IsDefault == true, c => c.Currency).FirstOrDefault();
                        if (exchangeRates.Any())
                        {
                            var projectsInYear = unitWork.ProjectRepository.GetManyQueryable(p => p.StartDate.Year == year);
                            foreach (var rate in ratesList)
                            {
                                var currency = (from r in exchangeRates
                                                where r.Currency == rate.Currency
                                                select r).FirstOrDefault();

                                if (currency == null)
                                {
                                    unitWork.ManualRatesRepository.Insert(new EFManualExchangeRates()
                                    {
                                        Year = year,
                                        ExchangeRate = rate.Rate,
                                        Currency = rate.Currency
                                    });
                                    await unitWork.SaveAsync();
                                }
                                else
                                {
                                    var manualExRate = (from r in exchangeRates
                                                        where r.Currency == rate.Currency && r.IsEditedByUser == false
                                                        select r).FirstOrDefault();

                                    if (manualExRate != null)
                                    {
                                        decimal averageRate = (from r in exchangeRates
                                                               where r.Currency == rate.Currency
                                                               select r.ExchangeRate).Average();
                                        averageRate = ((averageRate + rate.Rate) / 2);
                                        var manualRate = (from r in exchangeRates
                                                          where r.Currency == rate.Currency
                                                          select r).FirstOrDefault();

                                        if (manualRate != null)
                                        {
                                            manualRate.ExchangeRate = averageRate;
                                            unitWork.ManualRatesRepository.Update(manualRate);
                                        }
                                        await unitWork.SaveAsync();

                                        var projects = (from p in projectsInYear
                                                        where p.ProjectCurrency == rate.Currency
                                                        select p);

                                        foreach (var project in projects)
                                        {
                                            project.ExchangeRate = averageRate;
                                            unitWork.ProjectRepository.Update(project);
                                        }
                                        await unitWork.SaveAsync();
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                    }
                });
            }
            return response;
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

        public ActionResponse SetLabelForManualExRates(string label)
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
                exRateSettings.ManualExchangeRateSource = label;
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

        public async Task<IEnumerable<CurrencyWithRates>> GetAverageCurrencyRatesForDate(DateTime dated)
        {
            var unitWork = new UnitOfWork(context);
            List<CurrencyWithRates> exchangeRates = new List<CurrencyWithRates>();
            try
            {
                int proposedYear = dated.Year;
                IQueryable<EFManualExchangeRates> exRates = unitWork.ManualRatesRepository.GetManyQueryable(r => r.Year == dated.Year || r.Year == DateTime.Now.Year);
                var currencies = unitWork.CurrencyRepository.GetManyQueryable(c => c.Id != 0);
                if (exRates.Any())
                {
                    foreach(var exRate in exRates)
                    {
                        string currencyName = (from cur in currencies
                                               where cur.Currency.Equals(exRate.Currency)
                                               select cur.CurrencyName).FirstOrDefault();

                        exchangeRates.Add(new CurrencyWithRates()
                        {
                            Currency = exRate.Currency,
                            CurrencyName = currencyName,
                            Rate = exRate.ExchangeRate
                        });
                    }
                }
                else
                {
                    exRates = unitWork.ManualRatesRepository.GetManyQueryable(e => e.Year >= proposedYear);
                    if (exRates.Any())
                    {
                        foreach (var exRate in exRates)
                        {
                            string currencyName = (from cur in currencies
                                                   where cur.Currency.Equals(exRate.Currency)
                                                   select cur.CurrencyName).FirstOrDefault();
                            exchangeRates.Add(new CurrencyWithRates()
                            {
                                Currency = exRate.Currency,
                                CurrencyName = currencyName,
                                Rate = exRate.ExchangeRate
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return await Task<ExchangeRateForCurrency>.Run(() => exchangeRates).ConfigureAwait(false);
        }

        public async Task<ExchangeRatesView> GetLatestCurrencyRates()
        {
            var unitWork = new UnitOfWork(context);

            ExchangeRatesView ratesView = new ExchangeRatesView();
            IQueryable<EFCurrency> currencies = null;
            try
            {
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(r => r.Id != 0);
                
                    List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
                    DateTime dated = DateTime.Now;
                    ratesView.Dated = dated.Date.ToString();
                    var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
                    ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;

                    if (ratesView.Rates != null)
                    {
                        int count = ratesView.Rates.Count;
                        currencies = unitWork.CurrencyRepository.GetManyQueryable(c => c.Id != 0);
                        if (currencies.Count() < count)
                        {
                            var currencyCodes = (from c in currencies
                                                 select c.Currency).ToList<string>();
                            var currencyNames = (from c in currencies
                                                 select c.CurrencyName).ToList<string>();

                            var currenciesFromAPI = ratesView.Rates;
                            List<EFCurrency> newCurrencies = new List<EFCurrency>();
                            foreach (var currency in currenciesFromAPI)
                            {
                                if (!currencyCodes.Contains(currency.Currency))
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

                if (currencies != null && currencies.Count() > 0)
                {
                    foreach (var rate in ratesView.Rates)
                    {
                        rate.CurrencyName = (from c in currencies
                                             where c.Currency == rate.Currency
                                             select c.CurrencyName).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
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
            ExchangeRatesView ratesView = new ExchangeRatesView();
            List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
            /*var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
            ratesView.Dated = dated.ToShortDateString();
            ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;

            var currencies = unitWork.CurrencyRepository.GetManyQueryable(c => c.Id != 0);
            if (currencies.Count() > 0)
            {
                if (ratesView.Rates != null)
                {
                    foreach (var rate in ratesView.Rates)
                    {
                        rate.CurrencyName = (from c in currencies
                                             where c.Currency == rate.Currency
                                             select c.CurrencyName).FirstOrDefault();
                    }
                }
            }*/
            var exchangeRates = await unitWork.ManualRatesRepository.GetManyQueryableAsync(r => r.Year == dated.Year);
            if (exchangeRates.Any())
            {
                foreach(var exRate in exchangeRates)
                {
                    ratesList.Add(new CurrencyWithRates()
                    {
                        Currency = exRate.Currency,
                        Rate = exRate.ExchangeRate,
                        CurrencyName = exRate.Currency
                    });
                }
                ratesView.Rates = ratesList;
            }
            return await Task<ExchangeRatesView>.Run(() => ratesView).ConfigureAwait(false);
        }

        public ExchangeRatesSettingsView GetExRateSettings()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var exRateSettings = unitWork.ExRatesSettingsRepository.GetOne(e => e.Id != 0);
                ExchangeRatesSettingsView settingsView = new ExchangeRatesSettingsView();
                if (exRateSettings != null)
                {
                    settingsView.IsOpenExchangeKeySet = !string.IsNullOrEmpty(exRateSettings.APIKeyOpenExchangeRates) ? true : false;
                    settingsView.ManualCurrencyRates = exRateSettings.ManualExchangeRates == null ? null : JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exRateSettings.ManualExchangeRates);
                    settingsView.ManualExchangeRateSource = exRateSettings.ManualExchangeRateSource;
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

                /*var currencies = unitWork.CurrencyRepository.GetManyQueryable(c => c.Id != 0);
                if (currencies.Count() > 0)
                {
                    foreach(var rate in ratesList)
                    {
                        rate.CurrencyName = (from c in currencies
                                             where c.Currency == rate.Currency
                                             select c.CurrencyName).FirstOrDefault();
                    }
                }*/
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
                unitWork.ExRatesSettingsRepository.Update(exRateSettings);
                unitWork.Save();
                return response;
            }
        }
    }
}
