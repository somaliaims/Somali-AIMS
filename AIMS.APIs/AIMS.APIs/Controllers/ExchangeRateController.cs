using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        IExchangeRateService exchangeRateService;

        public ExchangeRateController(IExchangeRateService service)
        {
            this.exchangeRateService = service;
        }

        [HttpGet]
        public async Task<IActionResult> LoadLatestRates()
        {
            var response = await exchangeRateService.GetRatesAsync();
            
            return Ok(response);
        }
    }
}