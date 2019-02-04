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
        public async Task<IActionResult> GetSectorWiseProjects([FromBody] ReportModelForProjectSectors model)
        {
            var report = await reportService.GetProjectsBySector(model);
            return Ok(report);
        }
    }
}