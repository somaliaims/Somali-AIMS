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
    public class SectorCategoryController : ControllerBase
    {
        AIMSDbContext context;
        ISectorCategoryService sectorCategoryService;

        public SectorCategoryController(AIMSDbContext cntxt, ISectorCategoryService categoryService)
        {
            context = cntxt;
            sectorCategoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var categories = sectorCategoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var sectorCategory = sectorCategoryService.Get(id);
            return Ok(sectorCategory);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var sectorCategories = sectorCategoryService.GetMatching(criteria);
            return Ok(sectorCategories);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SectorCategoryModel sectorCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorCategoryService.Add(sectorCategory);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SectorCategoryModel sectorCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorCategoryService.Update(id, sectorCategory);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }
    }
}