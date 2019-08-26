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
            var currencies = currencyService.GetAll();
            return Ok(currencies);
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
            var currencies = currencyService.GetMatching(criteria);
            return Ok(currencies);
        }

        [HttpGet]
        [Route("GetDefault")]
        public IActionResult GetDefault()
        {
            var currency = currencyService.GetDefaultCurrency();
            return Ok(currency);
        }

        [HttpGet("GetForUser")]
        public IActionResult GetForUser()
        {
            return Ok(currencyService.GetForUser());
        }

        [HttpGet]
        [Route("GetNational")]
        public IActionResult GetNational()
        {
            var currency = currencyService.GetNationalCurrency();
            return Ok(currency);
        }

        [HttpPost("SetDefault/{id}")]
        public IActionResult SetDefault(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var response = currencyService.SetDefault(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("SetNational/{id}")]
        public IActionResult SetNational(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var response = currencyService.SetNational(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
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

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CurrencyModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = currencyService.Update(id, model);
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

            var response = currencyService.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        
    }
}