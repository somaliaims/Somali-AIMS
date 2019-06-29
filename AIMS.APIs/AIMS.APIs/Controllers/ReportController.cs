using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        IReportService reportService;
        IExcelGeneratorService excelService;
        IConfiguration configuration;
        string clientUrl = "";

        public ReportController(IReportService service, IExcelGeneratorService eService, IConfiguration config)
        {
            reportService = service;
            excelService = eService;
            configuration = config;
            clientUrl = configuration["ClientUrl"]; 
        }

        [HttpGet]
        [Route("GetProjectsBudgetReport")]
        public async Task<IActionResult> GetProjectsBudgetReport()
        {
            var report = await reportService.GetProjectsBudgetReport(clientUrl);
            return Ok(report);
        }

        [HttpPost]
        [Route("GetSectorWiseProjects")]
        public async Task<IActionResult> GetSectorWiseProjects([FromBody] SearchProjectsBySectorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await reportService.GetProjectsBySectors(model, clientUrl);
            var response = excelService.GenerateSectorProjectsReport(report);
            if (response.Success)
            {
                report.ReportSettings.ExcelReportName = response.Message;
            }
            return Ok(report);
        }

        [HttpPost]
        [Route("GetLocationWiseProjects")]
        public async Task<IActionResult> GetLocationWiseProjects([FromBody] SearchProjectsByLocationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await reportService.GetProjectsByLocations(model, clientUrl);
            var response = excelService.GenerateLocationProjectsReport(report);
            if (response.Success)
            {
                report.ReportSettings.ExcelReportName = response.Message;
            }
            return Ok(report);
        }

    }
}