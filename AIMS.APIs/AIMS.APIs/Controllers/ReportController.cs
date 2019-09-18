using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        IReportService reportService;
        IExcelGeneratorService excelService;
        IConfiguration configuration;
        IExchangeRateService ratesService;
        IExchangeRateHttpService ratesHttpService;
        ICurrencyService currencyService;
        string clientUrl = "";

        public ReportController(IReportService service, IExcelGeneratorService eService, IConfiguration config,
            IExchangeRateHttpService exRatesHttpService, IExchangeRateService exRatesService, ICurrencyService curService)
        {
            reportService = service;
            excelService = eService;
            configuration = config;
            ratesService = exRatesService;
            ratesHttpService = exRatesHttpService;
            currencyService = curService;
            clientUrl = configuration["ClientUrl"]; 
        }

        

        [HttpGet]
        [Route("GetProjectsBudgetSummaryReport")]
        public async Task<IActionResult> GetProjectsBudgetSummaryReport()
        {
            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var report = await reportService.GetProjectsBudgetReportSummary(clientUrl, defaultCurrency, exchangeRate);
            var response = excelService.GenerateProjectBudgetReportSummary(report);
            if (response.Success)
            {
                report.ReportSettings.ExcelReportName = response.Message;
            }
            return Ok(report);
        }

        [HttpPost]
        [Route("GetEnvelopeReport")]
        public async Task<IActionResult> GetEnvelopeReport([FromBody] SearchEnvelopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }
            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var report = await reportService.GetEnvelopeReport(model, clientUrl, defaultCurrency, exchangeRate);
            return Ok(report);
        }

        [HttpPost]
        [Route("GetSectorWiseProjects")]
        public async Task<IActionResult> GetSectorWiseProjects([FromBody] SearchProjectsBySectorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var report = await reportService.GetProjectsBySectors(model, clientUrl, defaultCurrency, exchangeRate);
            var response = excelService.GenerateSectorProjectsReport(report);
            if (response.Success)
            {
                report.ReportSettings.ExcelReportName = response.Message;
            }
            return Ok(report);
        }

        [HttpPost]
        [Route("GetLocationWiseProjects")]
        public async Task<IActionResult> GetLocationWiseProjects([FromBody] SearchProjectsByLocationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var report = await reportService.GetProjectsByLocations(model, clientUrl, defaultCurrency, exchangeRate);
            var response = excelService.GenerateLocationProjectsReport(report);
            if (response.Success)
            {
                report.ReportSettings.ExcelReportName = response.Message;
            }
            return Ok(report);
        }

        [HttpPost]
        [Route("GetYearWiseProjects")]
        public async Task<IActionResult> GetYearWiseProjects([FromBody] SearchProjectsByYearModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = reportService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var report = await reportService.GetProjectsByYear(model, clientUrl, defaultCurrency, exchangeRate);
            var response = excelService.GenerateYearlyProjectsReport(report);
            if (response.Success)
            {
                report.ReportSettings.ExcelReportName = response.Message;
            }
            return Ok(report);
        }

    }
}