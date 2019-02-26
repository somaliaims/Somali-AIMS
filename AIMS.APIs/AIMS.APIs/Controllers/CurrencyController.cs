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
    public class CurrencyController : ControllerBase
    {
        ICurrencyService currencyService;

        public CurrencyController(ICurrencyService service)
        {
            this.currencyService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var currencys = currencyService.GetAll();
            return Ok(currencys);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var currency = currencyService.Get(id);
            return Ok(currency);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var currencys = currencyService.GetMatching(criteria);
            return Ok(currencys);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CurrencyModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = currencyService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = currencyService.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        
    }
}