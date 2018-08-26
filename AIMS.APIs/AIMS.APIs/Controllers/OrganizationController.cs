using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        public OrganizationController()
        {

        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] string test)
        {
            return Ok();
        }
    }
}