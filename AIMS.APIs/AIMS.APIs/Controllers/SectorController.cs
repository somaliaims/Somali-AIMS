using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectorController : ControllerBase
    {
        ISectorService sectorService;

        public SectorController(ISectorService service)
        {
            this.sectorService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var sectors = sectorService.GetAll();
            return Ok(sectors);
        }

        [HttpGet]
        [Route("GetDefaultSectors")]
        public IActionResult GetDefaultSectors()
        {
            var sectors = sectorService.GetDefaultSectors();
            return Ok(sectors);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var sector = sectorService.Get(id);
            return Ok(sector);
        }

        [HttpGet]
        [Route("GetSectorsForType/{id}")]
        public IActionResult GetSectorsForType(int id)
        {
            var sector = sectorService.GetSectorsForType(id);
            return Ok(sector);
        }

        [HttpGet]
        [Route("GetChildren/{id}")]
        public IActionResult GetChildren(int id)
        {
            var sectors = sectorService.GetChildren(id);
            return Ok(sectors);
        }

        [HttpGet]
        [Route("SetChild/{sectorId}/{childId}")]
        public IActionResult SetChild(int sectorId, int childId)
        {
            var response = sectorService.SetChildSector(sectorId, childId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpGet]
        [Route("RemoveChild/{sectorId}/{childId}")]
        public IActionResult RemoveChild(int sectorId, int childId)
        {
            var response = sectorService.RemoveChildSector(sectorId, childId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var sectors = sectorService.GetMatching(criteria);
            return Ok(sectors);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SectorModel sector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorService.Add(sector);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("AddIATISector")]
        public IActionResult AddIATISector([FromBody] IATINewSectorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorService.AddIATISector(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SectorModel sector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorService.Update(id, sector);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpDelete]
        [Route("Delete/{id}/{newId}")]
        public async Task<IActionResult> Delete(int id, int newId)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = await sectorService.DeleteAsync(id, newId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}