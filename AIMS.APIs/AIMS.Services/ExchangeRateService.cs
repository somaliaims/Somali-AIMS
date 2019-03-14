using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    }
                }
                unitWork.Save();
                return response;
            }
        }

        public async Task<ExchangeRatesView> GetLatestCurrencyRates()
        {
            var unitWork = new UnitOfWork(context);
            ExchangeRatesView ratesView = new ExchangeRatesView() { Base = "USD" };
            List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
            DateTime dated = DateTime.Now;
            var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
            ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;
            return await Task<ExchangeRatesView>.Run(() => ratesView).ConfigureAwait(false);
        }

        public async Task<ExchangeRatesView> GetCurrencyRatesForDate(DateTime dated)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ExchangeRatesView ratesView = new ExchangeRatesView() { Base = "USD" };
                List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
                var exchangeRate = await unitWork.ExchangeRatesRepository.GetOneAsync(e => e.Dated.Date == dated.Date);
                ratesView.Rates = (exchangeRate != null) ? JsonConvert.DeserializeObject<List<CurrencyWithRates>>(exchangeRate.ExchangeRatesJson) : null;
                return await Task<ExchangeRatesView>.Run(() => ratesView).ConfigureAwait(false);
            }
        }
    }
}
