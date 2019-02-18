using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportSubscriptionController : ControllerBase
    {
        IReportSubscriptionService reportSubscriptionService;

        public ReportSubscriptionController(IReportSubscriptionService service)
        {
            this.reportSubscriptionService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdVal))
            {
                return BadRequest("Invalid attempt");
            }
            int userId = Convert.ToInt32(userIdVal);
            var reportSubscriptions = reportSubscriptionService.GetAll(userId);
            return Ok(reportSubscriptions);
        }

        [HttpPost]
        [Route("Subscribe")]
        public IActionResult Subscribe([FromBody] ReportSubscriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdVal))
            {
                return BadRequest("Invalid attempt");
            }
            int userId = Convert.ToInt32(userIdVal);
            var response = reportSubscriptionService.Add(userId, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("Unsubscribe")]
        public IActionResult UnSubscribe([FromBody] ReportSubscriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdVal))
            {
                return BadRequest("Invalid attempt");
            }
            int userId = Convert.ToInt32(userIdVal);
            var response = reportSubscriptionService.Add(userId, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}