﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
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
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                ratesView = await ratesHttpService.GetRatesAsync(apiKey);
                if (ratesView.Rates != null)
                {
                    await ratesService.SaveCurrencyRatesAsync(ratesView.Rates, DateTime.Now);
                }
            }
            return Ok(ratesView);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> FixExchangeRatesForProjects()
        {
            var response = await ratesService.ApplyAverageRates();
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("GetAverageCurrencyRateForDate")]
        public async Task<IActionResult> GetAverageCurrencyRateForDate([FromBody] ExRateFinderModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExchangeRatesView ratesView = null;
            int count = ratesService.GetAPIsCallsCount();
            if (count >= 999)
            {
                return Ok(null);
            }

            if (model.Dated > DateTime.Now)
            {
                ratesView = await ratesService.GetCurrencyRatesForDate(DateTime.Now);
            }
            else
            {
                ratesView = await ratesService.GetCurrencyRatesForDate(model.Dated);
            }
            
            if (ratesView.Rates == null)
            {
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                ratesView = await ratesHttpService.GetRatesAsync(apiKey);
                if (ratesView.Rates != null)
                {
                    await ratesService.SaveCurrencyRatesAsync(ratesView.Rates, DateTime.Now);
                }
            }

            var rates = await ratesService.GetAverageCurrencyRatesForDate(model.Dated);
            return Ok(rates);
        }

        [HttpGet("GetManualExchangeRates")]
        public IActionResult GetManualExchangeRates()
        {
            var rates = ratesService.GetManualExchangeRates();
            return Ok(rates);
        }


        [HttpGet]
        [Route("GetSettings")]
        public IActionResult GetSettings()
        {
            var settings = ratesService.GetExRateSettings();
            return Ok(settings);
        }

        [HttpGet]
        [Route("GetRatesForDate/{dated}")]
        public async Task<IActionResult> GetRatesForDate(DateTime dated)
        {
            bool isTodaysDate = false;
            if (dated.Date > DateTime.Now.Date)
            {
                dated = DateTime.Now;
                isTodaysDate = true;
            }
            else if(dated.Date == DateTime.Now.Date)
            {
                isTodaysDate = true;
            }

            ExchangeRatesView ratesView = null;
            int count = ratesService.GetAPIsCallsCount();
            if (count >= 999)
            {
                return Ok(null);
            }
            ratesView = await ratesService.GetCurrencyRatesForDate(dated);
            if (ratesView.Rates == null)
            {
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                string monthStr = dated.Month < 10 ? "0" + dated.Month : dated.Month.ToString();
                string dateStr = dated.Day < 10 ? "0" + dated.Day : dated.Day.ToString();
                string datedStr = dated.Year + "-" + monthStr + "-" + dateStr;
                ratesView = (isTodaysDate) ? await ratesHttpService.GetRatesAsync(apiKey) : await ratesHttpService.GetRatesForDateAsync(datedStr, apiKey);
                if (ratesView.Rates != null)
                {
                    await ratesService.SaveCurrencyRatesAsync(ratesView.Rates, dated);
                }
                else
                {
                    if (dated >= (dated.AddDays(-14)))
                    dated = (dated.AddDays(-15));
                    monthStr = dated.Month < 10 ? "0" + dated.Month : dated.Month.ToString();
                    dateStr = dated.Day < 10 ? "0" + dated.Day : dated.Day.ToString();
                    datedStr = dated.Year + "-" + monthStr + "-" + dateStr;
                    ratesView.Dated = dated.ToShortDateString();
                    ratesView = await ratesHttpService.GetRatesForDateAsync(datedStr, apiKey);
                }
            }
            return Ok(ratesView);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SetExchangeRateAutoSetting")]
        public IActionResult SetExchangeRatesAutoSettings([FromBody] ExRateAutoSetting model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = ratesService.SetExchangeRatesAutoSettings(model.IsAutomatic);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SaveManualCurrencyRates")]
        public IActionResult SaveManualCurrencyRates(ManualCurrencyRateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = ratesService.SaveCurrencyRatesManual(model.Rates);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SaveAPIKeyForOpenExchange")]
        public IActionResult SaveAPIKeyForOpenExchange([FromBody] ExchangeRateAPIKeyModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = ratesService.SetAPIKeyForOpenExchange(model.Key);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("SetLabelForManualExRates")]
        public IActionResult SetLabelForManualExRates([FromBody] ManualExRateSourceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = ratesService.SetLabelForManualExRates(model.Label);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}