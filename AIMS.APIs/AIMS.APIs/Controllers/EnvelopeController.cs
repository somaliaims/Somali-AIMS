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
    public class EnvelopeController : ControllerBase
    {
        IEnvelopeService envelopeService;

        public EnvelopeController(IEnvelopeService service)
        {
            this.envelopeService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            bool isValid = int.TryParse(organizationIdVal, out organizationId);
            if (organizationId == 0)
            {
                return BadRequest("A bad attempt to access the envelope");
            }
            var envelope = envelopeService.GetFunderEnvelope(organizationId);
            return Ok(envelope);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EnvelopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            bool isValid = int.TryParse(organizationIdVal, out organizationId);
            if (organizationId == 0)
            {
                return BadRequest("A bad attempt to access the envelope");
            }

            var response = await envelopeService.AddAsync(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EnvelopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            bool isValid = int.TryParse(organizationIdVal, out organizationId);
            if (organizationId == 0)
            {
                return BadRequest("A bad attempt to access the envelope");
            }
            var response = await envelopeService.AddAsync(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{funderId}")]
        public IActionResult Delete(int funderId)
        {
            if (funderId <= 0)
            {
                return BadRequest("Invalid parameters provided");
            }
            var response = envelopeService.Delete(funderId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}