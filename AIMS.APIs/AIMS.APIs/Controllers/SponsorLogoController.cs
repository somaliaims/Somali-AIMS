using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorLogoController : ControllerBase
    {
        IWebHostEnvironment hostEnvironment;
        ISponsorLogoService service;

        public SponsorLogoController(IWebHostEnvironment hostingEnvironment, ISponsorLogoService logoService)
        {
            hostEnvironment = hostingEnvironment;
            service = logoService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(service.GetAll());
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Post([FromForm]SponsorLogoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string webRoot =  hostEnvironment.WebRootPath;
            service.SetLogoDirectoryPath(webRoot);
            string logoDirectory = service.GetLogoDirectoryPath();
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Logo.FileName;
            var file = Request.Form.Files[0];
            var response = service.SaveLogo(file, model.Title, uniqueFileName);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}
