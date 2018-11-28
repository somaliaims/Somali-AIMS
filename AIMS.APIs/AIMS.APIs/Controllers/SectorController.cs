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
        AIMSDbContext context;
        ISectorService sectorService;

        public SectorController(AIMSDbContext cntxt, ISectorService service)
        {
            this.context = cntxt;
            this.sectorService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var sectors = sectorService.GetAll();
            return Ok();
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var sector = sectorService.Get(id);
            return Ok();
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
    }
}