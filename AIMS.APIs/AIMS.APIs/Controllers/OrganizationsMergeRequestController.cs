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
    public class OrganizationsMergeRequestController : ControllerBase
    {
        IOrganizationMergeService service;
        public OrganizationsMergeRequestController(IOrganizationMergeService mergeService)
        {
            service = mergeService;
        }

        [HttpGet("GetUserRequests")]
        public IActionResult GetUserRequests()
        {
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
            return Ok(service.GetForUser(userId));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MergeOrganizationModel model)
        {
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await service.AddAsync(model, userId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpGet("ApproveRequest/{id}")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            int organizationId = 0;
            string organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            if (organizationId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            var response = service.ApproveMergeRequest(id, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            response = await service.MergeOrganizations(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpGet("RejectRequest/{id}")]
        public IActionResult RejectRequest(int id)
        {
            int organizationId = 0;
            string organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            if (organizationId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("Unauthorized user access to api");
            }
            var response = service.RejectRequest(id, organizationId, userEmail);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}