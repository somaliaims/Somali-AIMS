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
        public async Task<IActionResult> LoadLatestRates()
        {
            ExchangeRatesView ratesView = null;
            ratesView = await ratesService.GetLatestCurrencyRates();
            if (ratesView.Rates == null)
            {
                ratesView = await ratesHttpService.GetRatesAsync();
                ratesService.SaveCurrencyRates(ratesView.Rates);
            }
            return Ok(ratesView);
        }

        [HttpGet]
        [Route("GetExchangeRatesForDate/{dated}")]
        public async Task<IActionResult> GetExchangeRatesForDate(DateTime dated)
        {
            ExchangeRatesView ratesView = null;
            ratesView = await ratesService.GetCurrencyRatesForDate(dated);
            return Ok(ratesView);
        }
    }
}