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
    public class ProjectMembershipController : ControllerBase
    {
        IProjectMembershipService service;

        public ProjectMembershipController(IProjectMembershipService membershipService)
        {
            this.service = membershipService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            int organizationId = 0, userId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            if(!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }

            if (organizationId < 1 || userId < 1)
            {
                return BadRequest("Invalid request");
            }
            var requests = service.GetRequestsForFunder(organizationId, userId);
            return Ok(requests);
        }

        [HttpGet]
        [Route("GetUserApprovedRequests")]
        public IActionResult GetUserApprovedRequests()
        {
            int organizationId = 0, userId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }

            if (organizationId < 1 || userId < 1)
            {
                return BadRequest("Invalid request");
            }
            var requests = service.GetRequestsForFunder(organizationId, userId);
            return Ok(requests);
        }

        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid id provided");
            }

            string email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid attempt");
            }

            ProjectMembershipRequestModel model = new ProjectMembershipRequestModel()
            {
                ProjectId = id,
                UserEmail = email
            };
            var response = service.AddMembershipRequest(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("ApproveRequest")]
        public async Task<IActionResult> ApproveRequest([FromBody] ProjectRequestStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0, ownerId = 0;
            string organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                ownerId = Convert.ToInt32(userIdVal);
            }
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail) || ownerId == 0)
            {
                return BadRequest("Unauthorized request");
            }
            var response = await service.ApproveMembershipRequestAsync(model.UserId, model.ProjectId, organizationId, ownerId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("UnApproveRequest")]
        public IActionResult UnApproveRequest([FromBody] ProjectRequestStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0, ownerId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                ownerId = Convert.ToInt32(userIdVal);
            }
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("Unauthorized request");
            }
            var response = service.UnApproveMembershipRequest(model.UserId, model.ProjectId, organizationId, ownerId, model.MembershipType);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}