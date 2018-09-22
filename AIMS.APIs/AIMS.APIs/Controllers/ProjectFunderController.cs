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
        IProjectFundersService projectFunderService;

        public ProjectFunderController(AIMSDbContext cntxt, IProjectFundersService service)
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