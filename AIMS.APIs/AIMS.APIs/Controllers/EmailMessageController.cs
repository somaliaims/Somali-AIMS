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
    public class EmailMessageController : ControllerBase
    {
        IEmailMessageService emailMessageService;

        public EmailMessageController(IEmailMessageService service)
        {
            this.emailMessageService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var currencies = emailMessageService.GetAll();
            return Ok(currencies);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var emailMessage = emailMessageService.Get(id);
            return Ok(emailMessage);
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public IActionResult Post([FromBody] EmailMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = emailMessageService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] EmailMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = emailMessageService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}