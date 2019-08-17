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
    public class ExchangeRateUsageController : ControllerBase
    {
        IExchangeRatesUsageService service;

        public ExchangeRateUsageController(IExchangeRatesUsageService _service)
        {
            service = _service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var exrateUsageList = service.GetAll();
            return Ok(exrateUsageList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var exrateUsage = service.Get(id);
            return Ok(exrateUsage);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExchangeRatesUsageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddOrUpdate(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{source}/{usageSection}")]
        public IActionResult Delete(ExchangeRateSources source, ExchangeRateUsageSection usageSection)
        {
            var response = service.Delete(source, usageSection);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}