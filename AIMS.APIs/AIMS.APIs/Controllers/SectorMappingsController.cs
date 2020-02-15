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
    public class SectorMappingsController : ControllerBase
    {
        ISectorMappingsService mappingService;

        public SectorMappingsController(ISectorMappingsService mService)
        {
            mappingService = mService;
        }

        [HttpGet("GetAllMappings")]
        public IActionResult GetAllMappings()
        {
            return Ok(mappingService.GetAllMappings());
        }

        [HttpGet]
        [Route("GetForSector/{id}")]
        public IActionResult GetForSector(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var sectors = mappingService.GetForSector(id);
            return Ok(sectors);
        }

        [HttpGet]
        [Route("GetSectorMappings/{sectorId}")]
        public IActionResult GetSectorMappings(int sectorId)
        {
            if (sectorId <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var sectors = mappingService.GetSectorMappings(sectorId);
            return Ok(sectors);
        }

        [HttpPost]
        [Route("GetSectorMappingsByName")]
        public IActionResult GetSectorMappingsByName([FromBody] SearchSectorMappingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var sectors = mappingService.GetSectorMappings(model.Sector);
            return Ok(sectors);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SectorMappingsModel model)
        {
            var response = await mappingService.AddAsync(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(SectorMappingModel model)
        {
            var response = await mappingService.AddOrUpdateAsync(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpDelete("{sectorId}/{mappingId}")]
        public IActionResult Delete(int sectorId, int mappingId)
        {
            if (sectorId <= 0 || mappingId <= 0)
            {
                return BadRequest("Invalid Id/s provided");
            }
            var response = mappingService.Delete(sectorId, mappingId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}