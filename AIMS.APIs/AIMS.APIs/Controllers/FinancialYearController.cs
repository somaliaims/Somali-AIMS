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
    public class FinancialYearController : ControllerBase
    {
        IFinancialYearService financialYearService;

        public FinancialYearController(IFinancialYearService service)
        {
            this.financialYearService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(financialYearService.GetAll());
        }

        [HttpGet("GetForEnvelope")]
        public IActionResult GetForEnvelope()
        {
            return Ok(financialYearService.GetForEnvelope());
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var financialYear = financialYearService.Get(id);
            return Ok(financialYear);
        }

        [HttpGet("AmendLabels")]
        public IActionResult AmendLabels()
        {
            financialYearService.AmendLabels();
            return Ok(true);
        }

        [HttpPost]
        public IActionResult Post([FromBody] FinancialYearModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = financialYearService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost("AddRange")]
        public IActionResult AddRange([FromBody] FinancialYearRangeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = financialYearService.AddRange(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] FinancialYearModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = financialYearService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}