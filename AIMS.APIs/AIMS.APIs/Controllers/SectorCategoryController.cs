using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
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
    }
}