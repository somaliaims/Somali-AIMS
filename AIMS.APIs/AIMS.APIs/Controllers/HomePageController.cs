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
    public class HomePageController : ControllerBase
    {
        IHomePageService service;

        public HomePageController(IHomePageService homePageService)
        {
            service = homePageService;
        }

        [HttpGet("GetSettings")]
        public IActionResult Get()
        {
            return Ok(service.GetSettings());
        }

        [HttpPost("SetSettings")]
        public IActionResult SetSettings([FromBody] HomePageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.SetSettings(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}