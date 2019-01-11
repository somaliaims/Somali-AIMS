using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMTPSettingsController : ControllerBase
    {
        ISMTPSettingsService smtpSettingsService;

        public SMTPSettingsController(ISMTPSettingsService service)
        {
            this.smtpSettingsService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var smtpSettings = smtpSettingsService.Get();
            return Ok(smtpSettings);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SMTPSettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = smtpSettingsService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SMTPSettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = smtpSettingsService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}