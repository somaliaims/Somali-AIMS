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
    public class OrganizationController : ControllerBase
    {
        IOrganizationService organizationService;

        public OrganizationController(IOrganizationService service)
        {
            this.organizationService = service;
        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(organizationService.GetAll());
        }

        [HttpGet("GetOrganizationsCount")]
        public IActionResult GetOrganizationsCount()
        {
            return Ok(organizationService.GetOrganizationsCount());
        }

        [HttpGet("GetWithTypes")]
        public IActionResult GetOrganizationsWithTypes()
        {
            return Ok(organizationService.GetWithType());
        }

        [HttpPost("CheckIfOrganizationsHaveUsers")]
        public IActionResult CheckIfOrganizationsHaveUsers([FromBody] IdsModel model)
        {
            return Ok(organizationService.CheckIfOrganizationsHaveUsers(model.Ids));
        }

        [HttpGet("GetUserOrganizations")]
        public IActionResult GetUserOrganizations()
        {
            return Ok(organizationService.GetUserOrganizations());
        }

        [HttpGet("GetOrganizationsHavingEnvelope")]
        public IActionResult GetOrganizationsHavingEnvelope()
        {
            return Ok(organizationService.GetOrganizationsHavingEnvelope());
        }

        [HttpGet("GetIATIOrganizations")]
        public IActionResult GetIATIOrganizations()
        {
            return Ok(organizationService.GetIATIOrganizations());
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var organization = organizationService.Get(id);
            return Ok(organization);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var organizations = organizationService.GetMatching(criteria);
            return Ok(organizations);
        }

        [HttpGet("GetOrganizationsForType/{id}")]
        public IActionResult GetOrganizationsForType(int id)
        {
            return Ok(organizationService.GetOrganizationsForType(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrganizationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = organizationService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("Merge")]
        public async Task<IActionResult> Merge([FromBody] MergeOrganizationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await organizationService.MergeOrganizations(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut]
        [Route("Rename/{id}")]
        public async Task<IActionResult> RenameOrganization(int id, [FromBody] OrganizationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await organizationService.RenameOrganization(id, model.Name);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrganizationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = organizationService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("Approve/{id}")]
        public IActionResult Post(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid organization id provided");
            }

            var response = organizationService.Approve(id);
            if (response.Success)
            {
                return Ok(true);
            }
            return BadRequest(response.Message);
        }

        [HttpDelete]
        [Route("Delete/{id}/{newId}")]
        public async Task<IActionResult> Delete(int id, int newId = 0)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = await organizationService.DeleteAsync(id, newId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

    }
}