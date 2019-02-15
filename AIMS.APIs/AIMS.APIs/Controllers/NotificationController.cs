using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AIMS.DAL.EF;
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
    public class NotificationController : ControllerBase
    {
        INotificationService notificationService;

        public NotificationController(INotificationService service)
        {
            this.notificationService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            var userTypeVal = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(organizationIdVal) || string.IsNullOrEmpty(userTypeVal) || string.IsNullOrEmpty(userIdVal))
            {
                return Ok("[]");
            }

            UserTypes userType = (UserTypes)Convert.ToInt32(userTypeVal);
            int organizationId = Convert.ToInt32(organizationIdVal);
            int userId = Convert.ToInt32(userIdVal);
            var notifications = notificationService.Get(userId, userType, organizationId);
            return Ok(notifications);
        }

        [HttpGet]
        [Route("GetCount")]
        public IActionResult GetCount()
        {
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            var userTypeVal = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(organizationIdVal) || string.IsNullOrEmpty(userTypeVal) || string.IsNullOrEmpty(userIdVal))
            {
                return Ok("[]");
            }

            UserTypes userType = (UserTypes)Convert.ToInt32(userTypeVal);
            int organizationId = Convert.ToInt32(organizationIdVal);
            int userId = Convert.ToInt32(userIdVal);
            var count = notificationService.GetCount(userId, userType, organizationId);
            return Ok(count);
        }

        [HttpPost]
        [Route("MarkNotificationsRead")]
        public IActionResult MarkNotificationsRead([FromBody] NotificationUpdateModel model)
        {
            if (model.Ids.Count == 0)
            {
                return Ok(0);
            }

            var response = notificationService.MarkNotificationsRead(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("DeleteNotifications")]
        public IActionResult DeleteNotifications([FromBody] NotificationUpdateModel model)
        {
            if (model.Ids.Count == 0)
            {
                return Ok(0);
            }

            var response = notificationService.DeleteNotifications(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}