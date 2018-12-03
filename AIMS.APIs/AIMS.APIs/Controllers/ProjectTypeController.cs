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
    public class ProjectTypeController : ControllerBase
    {
        AIMSDbContext context;
        IProjectTypesService projectTypeService;

        public ProjectTypeController(AIMSDbContext cntxt, IProjectTypesService service)
        {
            this.context = cntxt;
            this.projectTypeService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var projectTypes = projectTypeService.GetAll();
            return Ok(projectTypes);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var projectType = projectTypeService.Get(id);
            return Ok(projectType);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var projectTypes = projectTypeService.GetMatching(criteria);
            return Ok(projectTypes);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectTypesModel projectType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectTypeService.Add(projectType);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProjectTypesModel projectType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectTypeService.Update(id, projectType);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }
    }
}