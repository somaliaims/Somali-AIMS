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
    public class ProjectDeletionRequestController : ControllerBase
    {
        IProjectDeletionService service;

        public ProjectDeletionRequestController(IProjectDeletionService dService)
        {
            service = dService;
        }

        [HttpGet("GetActiveRequests")]
        public IActionResult GetActiveRequests()
        {
            var requests = service.GetDeletionRequests();
            return Ok(requests);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectDeletionRequestModel model)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.ProjectId == 0)
            {
                return BadRequest("Invalid project id provided");
            }

            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 0;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            model.UserId = userId;
            var response = service.AddRequest(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            /*else if (string.IsNullOrEmpty(userTypeStr))
            {
                return BadRequest("Unauthorized user access to api");
            }
            UserTypes userType = (UserTypes) Convert.ToInt32(userTypeStr);
            if ((userType != UserTypes.Manager) || (userType != UserTypes.SuperAdmin))
            {
                return BadRequest("");
            }*/
            return Ok(true);
        }

        [HttpPut("{projectId}")]
        public IActionResult CancelRequest(int projectId)
        {
            if (projectId <= 0)
            {
                return BadRequest("Invalid project id provided");
            }
            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 0;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            var response = service.CancelRequest(projectId, userId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            if (projectId <= 0)
            {
                return BadRequest("Invalid project id provided");
            }

            int userId = 0;
            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userTypeStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            else if (string.IsNullOrEmpty(userTypeStr))
            {
                return BadRequest("Unauthorized user access to api");
            }
            UserTypes userType = (UserTypes) Convert.ToInt32(userTypeStr);
            if ((userType != UserTypes.Manager) || (userType != UserTypes.SuperAdmin))
            {
                return BadRequest("You are not authorized to delete a project");
            }

            var response = service.DeleteProject(projectId, userId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

    }
}