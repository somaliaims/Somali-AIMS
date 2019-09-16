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
    public class EnvelopeTypeController : ControllerBase
    {
        IEnvelopeTypeService service;

        public EnvelopeTypeController(IEnvelopeTypeService srvc)
        {
            service = srvc;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAll());
        }

        [HttpPost]
        public IActionResult Add(EnvelopeTypeModel model)
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
            return Ok(true);
        }

        [HttpPut]
        public IActionResult Put(int id, EnvelopeTypeModel model)
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
            return Ok(true);
        }

        [HttpDelete("{id}/{mappingId}")]
        public async Task<IActionResult> DeleteAsync(int id, int mappingId)
        {
            if (id <= 0 || mappingId <= 0)
            {
                return BadRequest("Invalid id provided for Envelope type");
            }
            var response = await service.DeleteAsync(id, mappingId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}