using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        IProjectService projectService;
        IHostingEnvironment hostingEnvironment;

        public ProjectController(IProjectService service, IHostingEnvironment _hostingEnvironment)
        {
            this.hostingEnvironment = _hostingEnvironment;
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
        [Route("GetProjectsReport")]
        public async Task<IActionResult> GetProjectsReport()
        {
            var projects = projectService.GetProjectsReport();
            ExcelGeneratorService eService = new ExcelGeneratorService(this.hostingEnvironment);
            var response = await eService.GenerateProjectsReportAsync();
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetProjectProfileReport/{id}")]
        public async Task<IActionResult> GetProjectProfileReport(int id)
        {
            var projects = await projectService.GetProjectProfileReportAsync(id);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetProjectProfileReportBySector/{id}")]
        public async Task<IActionResult> GetProjectProfileReportBySector(int id)
        {
            var projects = await projectService.GetProjectsReportForSectorAsync(id);
            return Ok(projects);
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

        [HttpGet]
        [Route("GetFunders/{id}")]
        public IActionResult GetFunders(int id)
        {
            var funders = projectService.GetProjectFunders(id);
            return Ok(funders);
        }

        [HttpGet]
        [Route("GetImplementers/{id}")]
        public IActionResult GetImplementers(int id)
        {
            var funders = projectService.GetProjectImplementers(id);
            return Ok(funders);
        }

        [HttpGet]
        [Route("GetDisbursements/{id}")]
        public IActionResult GetDisbursements(int id)
        {
            var disbursements = projectService.GetProjectDisbursements(id);
            return Ok(disbursements);
        }

        [HttpGet]
        [Route("GetDocuments/{id}")]
        public IActionResult GetDocuments(int id)
        {
            var documents = projectService.GetProjectDocuments(id);
            return Ok(documents);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var projects = projectService.GetMatching(criteria);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetTitle/{id}")]
        public IActionResult GetTitle(int id)
        {
            var projectInfo = projectService.GetTitle(id);
            return Ok(projectInfo);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await projectService.AddAsync(project);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("ExtractProjectsByIds")]
        public async Task<IActionResult> ExtractProjectsByIds([FromBody] int[] ids)
        {
            if (ids.Count() == 0)
            {
                return Ok("[]");
            }
            var idsList = ids.ToList<int>();
            var projects = await projectService.GetProjectsByIdsAsync(idsList);
            return Ok(projects);
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

        [HttpPost]
        [Route("AddProjectFunder")]
        public IActionResult AddProjectFunder([FromBody] ProjectFunderModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectFunder(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("AddProjectImplementer")]
        public IActionResult AddProjectImplementer([FromBody] ProjectImplementerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectImplementer(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("AddProjectDisbursement")]
        public IActionResult AddProjectDisbursement([FromBody] ProjectDisbursementModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectDisbursement(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("AddProjectDocument")]
        public IActionResult AddProjectDocument([FromBody] ProjectDocumentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectDocument(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("SearchProjectsByCriteria")]
        public async Task<IActionResult> SearchProjectsByCriteria([FromBody] SearchProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projects = await projectService.SearchProjectsByCriteria(model);
            return Ok(projects);
        }

        [HttpPost]
        [Route("SearchProjectsViewByCriteria")]
        public async Task<IActionResult> SearchProjectsViewByCriteria([FromBody] SearchProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projects = await projectService.SearchProjectsViewByCriteria(model);
            return Ok(projects);
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
            return Ok(true);
        }

        [HttpDelete]
        [Route("DeleteProjectSector/{projectId}/{sectorId}")]
        public IActionResult DeleteProjectSector(int projectId, int sectorId)
        {
            if (projectId <= 0 || sectorId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            var response = projectService.DeleteProjectSector(projectId, sectorId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete]
        [Route("DeleteProjectFunder/{projectId}/{funderId}")]
        public IActionResult DeleteProjectFunder(int projectId, int funderId)
        {
            if (projectId <= 0 || funderId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            var response  = projectService.DeleteProjectFunder(projectId, funderId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete]
        [Route("DeleteProjectImplementer/{projectId}/{implementerId}")]
        public IActionResult DeleteProjectImplementer(int projectId, int implementerId)
        {
            if (projectId <= 0 || implementerId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }
            var response = projectService.DeleteProjectImplementer(projectId, implementerId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete]
        [Route("DeleteProjectDisbursement/{id}")]
        public IActionResult DeleteProjectDisbursement(int id)
        {
            DateTime dated = DateTime.Now;
            if (id <= 0)
            {
                return BadRequest("Invalid Id provided");
            }
            var response = projectService.DeleteProjectDisbursement(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete]
        [Route("DeleteProjectDocument/{id}")]
        public IActionResult DeleteProjectDocument(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id provided");
            }
            var response = projectService.DeleteProjectDocument(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}