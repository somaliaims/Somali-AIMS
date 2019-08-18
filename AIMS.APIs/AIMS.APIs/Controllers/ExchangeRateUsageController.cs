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
            var response = service.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ExchangeRatesUsageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = service.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}