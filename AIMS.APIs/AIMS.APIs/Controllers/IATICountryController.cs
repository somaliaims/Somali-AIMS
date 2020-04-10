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
    public class IATICountryController : ControllerBase
    {
        ICountryService service;
        public IATICountryController(ICountryService srvc)
        {
            service = srvc;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(service.GetAll());
        }

        [HttpGet("GetActiveCountry")]
        public IActionResult GetActiveCountry()
        {
            return Ok(service.GetActiveCountry());
        }
       
        [HttpPost("SetActiveCountry/{code}")]
        public IActionResult SetActiveCountry(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid code provided");
            }
            var response = service.SetActiveCountry(code);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        public IActionResult Post([FromBody] List<IATICountryModel> model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddList(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}