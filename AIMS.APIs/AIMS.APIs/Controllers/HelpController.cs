using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Services;
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

        [HttpGet("GetProjectDisbursementsFields")]
        public IActionResult GetProjectDisbursementsFields()
        {
            return Ok(service.GetHelpForProjectDisbursementFields());
        }

        [HttpGet("GetExpectedDisbursementsFields")]
        public IActionResult GetExpectedDisbursementsFields()
        {
            return Ok(service.GetHelpForProjectExpectedDisbursementFields());
        }

        [HttpGet("GetProjectDocumentsFields")]
        public IActionResult GetProjectDocumentsFields()
        {
            return Ok(service.GetHelpForProjectDocumentsFields());
        }
    }
}