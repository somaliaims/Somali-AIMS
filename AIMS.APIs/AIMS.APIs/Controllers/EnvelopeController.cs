using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvelopeController : ControllerBase
    {
        IEnvelopeService envelopeService;

        public EnvelopeController(IEnvelopeService service)
        {
            this.envelopeService = service;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var envelope = envelopeService.GetFunderEnvelope(id);
            return Ok(envelope);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EnvelopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await envelopeService.AddAsync(model);
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

            var response = await envelopeService.AddAsync(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{funderId}/{year}")]
        public IActionResult Delete(int funderId, int year)
        {
            if (funderId <= 0 || year < 1900)
            {
                return BadRequest("Invalid parameters provided");
            }
            var response = envelopeService.Delete(funderId, year);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}