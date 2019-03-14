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
    public interface IExchangeRateService
    {
        /// <summary>
        /// Gets latest list of the rates from open exchange rates
        /// </summary>
        /// <returns></returns>
        Task<ExchangeRatesView> GetRatesAsync();

        /// <summary>
        /// Converts rates str 
        /// </summary>
        /// <param name="ratesStr"></param>
        /// <returns></returns>
        List<CurrencyWithRates> GetRatesList(string ratesStr);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient client;

        public ExchangeRateService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://openexchangerates.org/api/");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "ExchangeRates");
            client = httpClient;
        }

        public async Task<ExchangeRatesView> GetRatesAsync()
        {
            var response = await client.GetStringAsync("latest.json?app_id=ce2f27af4d414969bfe05b7285a01dec");
            var ratesJson = JsonConvert.DeserializeObject<dynamic>(response);
            string ratesStr = ratesJson != null ? JsonConvert.SerializeObject(ratesJson.rates) : "";
            ExchangeRatesView ratesView = new ExchangeRatesView();
            ratesStr = ratesStr.Replace("\\", "").Replace("\"", "");
            ratesStr = ratesStr.Replace("{", "");
            ratesStr = ratesStr.Replace("}", "");
            ratesView.Rates = this.GetRatesList(ratesStr);
            return ratesView;
        }

        public List<CurrencyWithRates> GetRatesList(string ratesStr)
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
