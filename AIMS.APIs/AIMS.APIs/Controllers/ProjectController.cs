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
    public class ProjectController : ControllerBase
    {
        AIMSDbContext context;
        IProjectService projectService;

        public ProjectController(AIMSDbContext cntxt, IProjectService service)
        {
            this.context = cntxt;
            this.projectService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var projects = projectService.GetAll();
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var project = projectService.Get(id);
            return Ok(project);
        }

        [HttpGet]
        [Route("GetLocations/{id}")]
        public IActionResult GetLocations(int id)
        {
            var locations = projectService.GetProjectLocations(id);
            return Ok(locations);
        }

        [HttpGet]
        [Route("GetSectors/{id}")]
        public IActionResult GetSectors(int id)
        {
            var sectors = projectService.GetProjectSectors(id);
            return Ok(sectors);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var projects = projectService.GetMatching(criteria);
            return Ok(projects);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.Add(project);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("AddProjectLocation")]
        public IActionResult AddProjectLocation([FromBody] ProjectLocationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectLocation(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("AddProjectSector")]
        public IActionResult AddProjectSector([FromBody] ProjectSectorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectSector(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.Update(id, project);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpDelete]
        [Route("DeleteProjectLocation/{projectId}/{locationId}")]
        public IActionResult DeleteProjectLocation(int projectId, int locationId)
        {
            if (projectId <= 0 || locationId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            projectService.DeleteProjectLocation(projectId, locationId);
            return Ok();
        }
    }
}