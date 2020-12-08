using AIMS.APIs.Helpers;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubLocationController : ControllerBase
    {
        ISubLocationService service;

        public SubLocationController(ISubLocationService srvc)
        {
            service = srvc;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(service.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(service.Get(id));
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetForLocation(int id)
        {
            if (id <= 0)
            {
                CommonMessageHelper helper = new CommonMessageHelper();
                return BadRequest(helper.GetInvalidIdMessage());
            }
            return Ok(service.GetForLocation(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] SubLocationModel model)
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

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SubLocationModel model)
        {
            if (id <= 0)
            {
                CommonMessageHelper helper = new CommonMessageHelper();
                return BadRequest(helper.GetInvalidIdMessage());
            }

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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                CommonMessageHelper helper = new CommonMessageHelper();
                return BadRequest(helper.GetInvalidIdMessage());
            }
            var response = service.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

    }
}
