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
            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            else
            {
                return BadRequest("Invalid attempt");
            }
            var requests = service.GetRequestsForFunder(organizationId);
            return Ok(requests);
        }

        [HttpGet]
        [Route("GetUserApprovedRequests")]
        public IActionResult GetUserApprovedRequests()
        {
            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            else
            {
                return BadRequest("Invalid attempt");
            }
            var requests = service.GetRequestsForFunder(organizationId);
            return Ok(requests);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectMembershipRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddMembershipRequest(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("ApproveRequest")]
        public IActionResult ApproveRequest([FromBody] ProjectMembershipRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            var response = service.ApproveMembershipRequest(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("UnApproveRequest")]
        public IActionResult UnApproveRequest([FromBody] ProjectMembershipRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            var response = service.UnApproveMembershipRequest(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}