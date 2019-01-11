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
    public class ProjectImplementorController : ControllerBase
    {
        IProjectImplementorService projectImplementorService;

        public ProjectImplementorController(IProjectImplementorService service)
        {
            this.projectImplementorService = service;
        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var organizations = projectImplementorService.GetAll();
            return Ok(organizations);
        }

        [HttpGet]
        [Route("GetProjectImplementors/{id}")]
        public IActionResult GetProjectImplementors(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Implementor id provided");
            }
            var implementors = projectImplementorService.GetProjectImplementors(id);
            return Ok(implementors);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectImplementorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectImplementorService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("RemoveImplementor")]
        public IActionResult RemoveImplementor([FromBody] ProjectImplementorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectImplementorService.RemoveImplementor(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok("1");
        }
    }
}