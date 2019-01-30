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

        [HttpGet]
        [Route("GetSectorWiseProjects")]
        public async Task<IActionResult> GetSectorWiseProjects()
        {
            var report = await reportService.GetProjectsBySector();
            return Ok(report);
        }
    }
}