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
    public class MarkerController : ControllerBase
    {
        IMarkersService service;

        public MarkerController(IMarkersService cService)
        {
            service = cService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var fields = service.GetAll();
            return Ok(fields);
        }

        [HttpGet("GetActive")]
        public IActionResult GetActive()
        {
            var fields = service.GetActiveFields();
            return Ok(fields);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var field = service.Get(id);
            return Ok(field);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public IActionResult Post([FromBody] MarkerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = service.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MarkerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = service.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid id provided");
            }
            var response = await service.DeleteAsync(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }
    }
}