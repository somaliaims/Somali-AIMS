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
    public class SectorSubCategoryController : ControllerBase
    {
        /*ISectorSubCategoryService sectorSubCategoryService;

        public SectorSubCategoryController(ISectorSubCategoryService categoryService)
        {
            sectorSubCategoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var categories = sectorSubCategoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var sectorSubCategory = sectorSubCategoryService.Get(id);
            return Ok(sectorSubCategory);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var sectorSubCategorys = sectorSubCategoryService.GetMatching(criteria);
            return Ok(sectorSubCategorys);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SectorSubCategoryModel sectorSubCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorSubCategoryService.Add(sectorSubCategory);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SectorSubCategoryModel sectorSubCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = sectorSubCategoryService.Update(id, sectorSubCategory);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }*/
    }
}