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
    public class SectorTypesController : ControllerBase
    {
        AIMSDbContext context;
        ISectorTypesService sectorTypeService;

        public SectorTypesController(AIMSDbContext cntxt, ISectorTypesService service)
        {
            this.context = cntxt;
            this.sectorTypeService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] SectorTypesModel sectorType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorTypeService.Add(sectorType);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SectorTypesModel sectorType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorTypeService.Update(id, sectorType);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }
    }
}