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
    public class GrantTypeController : ControllerBase
    {
        IGrantTypeService grantTypeService;
        public GrantTypeController(IGrantTypeService gService)
        {
            grantTypeService = gService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var grantTypes = grantTypeService.GetAll();
            return Ok(grantTypes);
        }

        [HttpGet("{criteria}")]
        public IActionResult SearchGrantType(string criteria)
        {
            var grantTypes = grantTypeService.GetMatching(criteria);
            return Ok(grantTypes);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GrantTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = grantTypeService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GrantTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = grantTypeService.Update(id, model);
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
            var response = await grantTypeService.DeleteAsync(id, newId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}