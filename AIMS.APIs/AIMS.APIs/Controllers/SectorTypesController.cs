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
        ISectorTypesService sectorTypeService;

        public SectorTypesController(ISectorTypesService service)
        {
            this.sectorTypeService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var sectorTypes = sectorTypeService.GetAll();
            return Ok(sectorTypes);
        }

        [HttpGet]
        [Route("GetDefault")]
        public IActionResult GetDefault()
        {
            var sectorType = sectorTypeService.GetDefault();
            return Ok(sectorType);
        }

        [HttpGet]
        [Route("GetOtherSectorTypes")]
        public IActionResult GetOtherSectorTypes()
        {
            var sectorTypes = sectorTypeService.GetOtherSectors();
            return Ok(sectorTypes);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var sectorType = sectorTypeService.Get(id);
            return Ok(sectorType);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var sectorTypes = sectorTypeService.GetMatching(criteria);
            return Ok(sectorTypes);
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