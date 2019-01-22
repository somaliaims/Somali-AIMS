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
    public class ProjectImplementerController : ControllerBase
    {
        IProjectImplementerService projectImplementerService;

        public ProjectImplementerController(IProjectImplementerService service)
        {
            this.projectImplementerService = service;
        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var organizations = projectImplementerService.GetAll();
            return Ok(organizations);
        }

        [HttpGet]
        [Route("GetProjectImplementers/{id}")]
        public IActionResult GetProjectImplementers(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Implementer id provided");
            }
            var implementers = projectImplementerService.GetProjectImplementers(id);
            return Ok(implementers);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectImplementerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectImplementerService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("RemoveImplementer")]
        public IActionResult RemoveImplementer([FromBody] ProjectImplementerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectImplementerService.RemoveImplementer(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok("1");
        }
    }
}