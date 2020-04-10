using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(AuthenticationSchemes = "Bearer")]
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut]
        [Route("SetAsDefault/{id}")]
        public async Task<IActionResult> SetAsDefault(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid id provided");
            }
            var response = await sectorTypeService.SetAsDefaultAsync(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut]
        [Route("SetAsIATIType/{id}")]
        public async Task<IActionResult> SetAsIATIType(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid id provided");
            }
            var response = await sectorTypeService.SetAsIATIAsync(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
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
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var response = sectorTypeService.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}