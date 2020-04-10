using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase
    {
        IHelpService service;

        public HelpController(IHelpService helpService)
        {
            service = helpService;
        }

        [HttpGet("GetProjectFields")]
        public IActionResult GetProjectFields()
        {
            return Ok(service.GetHelpForProjectFields());
        }

        [HttpGet("GetProjectFunderFields")]
        public IActionResult GetProjectFunderFields()
        {
            return Ok(service.GetHelpForProjectFunderFields());
        }

        [HttpGet("GetProjectImplementerFields")]
        public IActionResult GetProjectImplementerFields()
        {
            return Ok(service.GetHelpForProjectImpelenterFields());
        }

        [HttpGet("GetProjectSectorFields")]
        public IActionResult GetProjectSectorFields()
        {
            return Ok(service.GetHelpForProjectSectorFields());
        }

        [HttpGet("GetProjectLocationFields")]
        public IActionResult GetProjectLocationFields()
        {
            return Ok(service.GetHelpForProjectLocationFields());
        }

        [HttpGet("GetProjectDisbursementsFields")]
        public IActionResult GetProjectDisbursementsFields()
        {
            return Ok(service.GetHelpForProjectDisbursementFields());
        }

        [HttpGet("GetProjectDocumentsFields")]
        public IActionResult GetProjectDocumentsFields()
        {
            return Ok(service.GetHelpForProjectDocumentsFields());
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectHelp")]
        public IActionResult AddProjectHelp([FromBody] ProjectHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProject(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectFunderHelp")]
        public IActionResult AddProjectFunderHelp([FromBody] ProjectFunderHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectFunder(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectImplementerHelp")]
        public IActionResult AddProjectImplementerHelp([FromBody] ProjectImplementerHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectImplementer(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectSectorHelp")]
        public IActionResult AddProjectSectorHelp([FromBody] ProjectSectorHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectSector(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectLocationHelp")]
        public IActionResult AddProjectLocationHelp([FromBody] ProjectLocationHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectLocation(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectDisbursementHelp")]
        public IActionResult AddProjectDisbursementHelp([FromBody] ProjectDisbursementHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectDisbursement(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddProjectDocumentHelp")]
        public IActionResult AddProjectDocumentHelp([FromBody] ProjectDocumentHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectDocument(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}