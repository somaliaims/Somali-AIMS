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
    public class FinancialYearSettingsController : ControllerBase
    {
        IFinancialYearSettingsService service;
        public FinancialYearSettingsController(IFinancialYearSettingsService srvc)
        {
            service = srvc;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(service.Get());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FinancialYearSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await service.AddAsync(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}