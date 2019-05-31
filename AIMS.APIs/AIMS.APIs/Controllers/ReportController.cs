using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        IReportService reportService;

        public ReportController(IReportService service)
        {
            this.reportService = service;
        }

        [HttpPost]
        [Route("GetSectorWiseProjects")]
        public async Task<IActionResult> GetSectorWiseProjects([FromBody] SearchProjectsBySectorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await reportService.GetProjectsBySectors(model);
            return Ok(report);
        }

        /*[HttpPost]
        [Route("SearchProjectsBySector")]
        public async Task<IActionResult> SearchProjectsBySector([FromBody] ReportModelForProjectSectors model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projects = await reportService.GetProjectsBySector(model);
            return Ok(projects);
        }*/
    }
}