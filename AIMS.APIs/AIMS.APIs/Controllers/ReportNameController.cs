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
    public class ReportNameController : ControllerBase
    {
        IReportNamesService reportNameService;

        public ReportNameController(IReportNamesService service)
        {
            this.reportNameService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var reportNames = reportNameService.GetAll();
            return Ok(reportNames);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var reportName = reportNameService.Get(id);
            return Ok(reportName);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var reportNames = reportNameService.GetMatching(criteria);
            return Ok(reportNames);
        }
    }
}