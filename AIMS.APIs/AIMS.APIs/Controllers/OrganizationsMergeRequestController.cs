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
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsMergeRequestController : ControllerBase
    {
        IOrganizationMergeService service;
        public OrganizationsMergeRequestController(IOrganizationMergeService mergeService)
        {
            service = mergeService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MergeOrganizationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await service.AddAsync(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}