using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IATIController : ControllerBase
    {
        IIATIService iatiService;
        public IATIController(IIATIService service)
        {
            iatiService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var iatiActivities = iatiService.GetAll();
            return Ok(iatiActivities);
        }
    }
}