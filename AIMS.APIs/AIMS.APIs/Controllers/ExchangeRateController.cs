using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        IExchangeRateService ratesService;
        IExchangeRateHttpService ratesHttpService;

        public ExchangeRateController(IExchangeRateService service, IExchangeRateHttpService exRatesHttpService)
        {
            this.ratesService = service;
            this.ratesHttpService = exRatesHttpService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestRates()
        {
            ExchangeRatesView ratesView = null;
            int count = ratesService.GetAPIsCallsCount();
            if (count >= 999)
            {
                return Ok(null);
            }
            ratesView = await ratesService.GetLatestCurrencyRates();
            if (ratesView.Rates == null)
            {
                ratesView = await ratesHttpService.GetRatesAsync();
                ratesService.SaveCurrencyRates(ratesView.Rates);
            }
            return Ok(ratesView);
        }

        [HttpGet]
        [Route("GetRatesForDate/{dated}")]
        public async Task<IActionResult> GetRatesForDate(DateTime dated)
        {
            ExchangeRatesView ratesView = null;
            int count = ratesService.GetAPIsCallsCount();
            if (count >= 999)
            {
                return Ok(null);
            }
            string datedStr = dated.Year + "-" + dated.Month + "-" + dated.Day;
            ratesView = await ratesService.GetCurrencyRatesForDate(dated);
            if (ratesView.Rates == null)
            {
                ratesView = await ratesHttpService.GetRatesForDateAsync(datedStr);
                ratesService.SaveCurrencyRates(ratesView.Rates);
            }
            return Ok(ratesView);
        }

        [HttpGet]
        [Route("GetExchangeRatesForDate/{dated}")]
        public async Task<IActionResult> GetExchangeRatesForDate(DateTime dated)
        {
            ExchangeRatesView ratesView = null;
            int count = ratesService.GetAPIsCallsCount();
            if (count >= 999)
            {
                return Ok(null);
            }
            ratesView = await ratesService.GetCurrencyRatesForDate(dated);
            return Ok(ratesView);
        }
    }
}