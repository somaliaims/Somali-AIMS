﻿using System;
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
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                ratesView = await ratesHttpService.GetRatesAsync(apiKey);
                if (ratesView.Rates != null)
                {
                    ratesService.SaveCurrencyRates(ratesView.Rates, DateTime.Now);
                }
            }
            return Ok(ratesView);
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
                return BadRequest("Invalid date provided. Currency rates cannot be fetched for future dates");
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
                ratesService.SaveCurrencyRates(ratesView.Rates, dated);
            }
            return Ok(ratesView);
        }

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
    }
}