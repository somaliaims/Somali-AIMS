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
    public class FundingTypeController : ControllerBase
    {
        IFundingTypeService fundingTypeService;
        public FundingTypeController(IFundingTypeService gService)
        {
            fundingTypeService = gService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var fundingTypes = fundingTypeService.GetAll();
            return Ok(fundingTypes);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var fundingType = fundingTypeService.Get(id);
            return Ok(fundingType);
        }

        [HttpPost]
        public IActionResult Post([FromBody] FundingTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = fundingTypeService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] FundingTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = fundingTypeService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{id}/{newId}")]
        public async Task<IActionResult> DeleteAsync(int id, int newId)
        {
            if (id <= 0 || newId < 0)
            {
                return BadRequest("Invalid id/s provided");
            }
            var response = await fundingTypeService.DeleteAsync(id, newId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}