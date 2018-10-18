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
        AIMSDbContext context;
        IOrganizationService organizationService;

        public OrganizationController(AIMSDbContext cntxt, IOrganizationService service)
        {
            this.context = cntxt;
            this.organizationService = service;
        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var organizations = organizationService.GetAll();
            return Ok(organizations);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var organizations = organizationService.GetMatching(criteria);
            return Ok(organizations);
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

    }
}