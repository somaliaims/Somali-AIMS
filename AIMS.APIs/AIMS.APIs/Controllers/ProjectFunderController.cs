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
    public class ProjectFunderController : ControllerBase
    {
        AIMSDbContext context;
        IProjectFunderService projectFunderService;

        public ProjectFunderController(AIMSDbContext cntxt, IProjectFunderService service)
        {
            this.context = cntxt;
            this.projectFunderService = service;
        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var organizations = projectFunderService.GetAll();
            return Ok(organizations);
        }

        [HttpGet]
        [Route("GetProjectFunders/{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid funder id provided");
            }
            var funders = projectFunderService.GetProjectFunders(id);
            return Ok(funders);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectFunderModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectFunderService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("RemoveFunder")]
        public IActionResult RemoveFunder([FromBody] ProjectFunderModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectFunderService.RemoveFunder(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok("1");
        }
    }
}