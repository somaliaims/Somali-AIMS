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
        public IActionResult Post([FromBody]SponsorLogoModel model)
        {
            try
            {
                string webRoot =  hostEnvironment.WebRootPath;
                service.SetLogoDirectoryPath(webRoot);
                string logoDirectory = service.GetLogoDirectoryPath();
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Logo.FileName;
                var file = Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(logoDirectory, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
