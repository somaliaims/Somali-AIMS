using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EmailController : ControllerBase
    {
        IEmailService emailService;

        public EmailController(IEmailService eService)
        {
            emailService = eService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("SendEmailToUsers")]
        public IActionResult SendEmailToUsers([FromBody] EmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = emailService.SendEmailToUsers(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("ContactRequest")]
        public IActionResult SendContactEmailRequest([FromBody] ContactEmailRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}