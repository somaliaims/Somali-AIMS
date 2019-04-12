using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IExchangeRateHttpService
    {
        /// <summary>
        /// Gets latest list of the rates from open exchange rates
        /// </summary>
        /// <returns></returns>
        Task<ExchangeRatesView> GetRatesAsync(string apiToken);

        /// <summary>
        /// Gets rates for the provided date
        /// </summary>
        /// <returns></returns>
        Task<ExchangeRatesView> GetRatesForDateAsync(string dated, string apiToken);
    }

    public class ExchangeRateHttpService : IExchangeRateHttpService
    {
        private readonly HttpClient client;

        public ExchangeRateHttpService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://openexchangerates.org/api/");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "ExchangeRates");
            client = httpClient;
        }

        public async Task<ExchangeRatesView> GetRatesAsync(string apiToken)
        {
            ExchangeRatesView ratesView = new ExchangeRatesView();
            try
            {
                //var response = await client.GetStringAsync("latest.json?app_id=ce2f27af4d414969bfe05b7285a01dec");
                var response = await client.GetStringAsync("latest.json?app_id=" + apiToken);
                var ratesJson = JsonConvert.DeserializeObject<dynamic>(response);
                string ratesStr = ratesJson != null ? JsonConvert.SerializeObject(ratesJson.rates) : "";
                ratesStr = ratesStr.Replace("\\", "").Replace("\"", "");
                ratesStr = ratesStr.Replace("{", "");
                ratesStr = ratesStr.Replace("}", "");
                ratesView.Rates = this.GetRatesList(ratesStr);
            }
            catch(Exception)
            {
            }
            return await Task<List<CurrencyWithRates>>.Run(() => ratesView).ConfigureAwait(false);
        }

        public async Task<ExchangeRatesView> GetRatesForDateAsync(string dated, string apiToken)
        {
            ExchangeRatesView ratesView = new ExchangeRatesView();
            try
            {
                var response = await client.GetStringAsync(dated + ".json?app_id=" + apiToken);
                var ratesJson = JsonConvert.DeserializeObject<dynamic>(response);
                string ratesStr = ratesJson != null ? JsonConvert.SerializeObject(ratesJson.rates) : "";
                ratesStr = ratesStr.Replace("\\", "").Replace("\"", "");
                ratesStr = ratesStr.Replace("{", "");
                ratesStr = ratesStr.Replace("}", "");
                ratesView.Rates = this.GetRatesList(ratesStr);
                string ratesJsonStr = JsonConvert.SerializeObject(ratesView.Rates);
            }
            catch(Exception)
            {
            }
            return await Task<List<CurrencyWithRates>>.Run(() => ratesView).ConfigureAwait(false);
        }

        private List<CurrencyWithRates> GetRatesList(string ratesStr)
        {
            List<CurrencyWithRates> ratesList = new List<CurrencyWithRates>();
            if (ratesStr.Length > 0)
            {
                string[] rateRows = ratesStr.Split(",");
                for(int row=0; row < rateRows.Length; row++)
                {
                    string[] currencyRate = rateRows[row].Split(":");
                    if (currencyRate.Length > 1)
                    {
                        string currency = currencyRate[0];
                        decimal rate = Convert.ToDecimal(currencyRate[1]);

                        ratesList.Add(new CurrencyWithRates()
                        {
                            Currency = currency,
                            Rate = rate
                        });
                    }
                }
            }
            return ratesList;
        }
        
    }
}
