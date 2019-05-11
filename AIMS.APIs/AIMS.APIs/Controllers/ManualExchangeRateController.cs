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
    public class ManualExchangeRateController : ControllerBase
    {
        IManualExchangeRatesService service;

        public ManualExchangeRateController(IManualExchangeRatesService exRateService)
        {
            service = exRateService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var rates = service.GetAll();
            return Ok(rates);
        }

        [HttpGet("GetByDate/{dated}")]
        public IActionResult GetByDated(DateTime dated)
        {
            var rate = service.GetByDate(dated);
            return Ok(rate);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ManualRateModel model)
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
        public IActionResult Put(int id, [FromBody] ManualRateModel model)
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
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = service.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}