using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
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
    public class DocumentLinkController : ControllerBase
    {
        IDocumentLinkService service;

        public DocumentLinkController(IDocumentLinkService documentService)
        {
            service = documentService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(service.GetAll());
        }

        [HttpPost]
        public IActionResult Post([FromBody]DocumentLinkModel model)
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
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var response = service.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}
