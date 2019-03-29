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
    public class SectorMappingsController : ControllerBase
    {
        ISectorMappingsService mappingService;

        public SectorMappingsController(ISectorMappingsService mService)
        {
            mappingService = mService;
        }

        public IActionResult GetForSector(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var sectors = mappingService.GetForSector(id);
            return Ok(sectors);
        }
    }
}