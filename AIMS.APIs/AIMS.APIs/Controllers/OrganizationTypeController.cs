using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationTypeController : ControllerBase
    {
        IOrganizationTypeService organizationTypeService;

        public OrganizationTypeController(IOrganizationTypeService service)
        {
            this.organizationTypeService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = organizationTypeService.GetAll();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var response = organizationTypeService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrganizationTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = organizationTypeService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrganizationTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = organizationTypeService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpDelete("{id}/{newId}")]
        public async Task<IActionResult> Delete(int id, int newId = 0)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = await organizationTypeService.DeleteAsync(id, newId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}