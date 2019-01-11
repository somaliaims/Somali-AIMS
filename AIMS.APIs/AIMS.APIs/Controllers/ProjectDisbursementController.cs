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
    public class ProjectDisbursementController : ControllerBase
    {
        IProjectDisbursementService projectDisbursementService;

        public ProjectDisbursementController(IProjectDisbursementService service)
        {
            this.projectDisbursementService = service;
        }


        /// <summary>
        /// Gets disbursements for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var projectDisbursements = projectDisbursementService.GetAll(id);
            return Ok(projectDisbursements);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectDisbursementModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectDisbursementService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

    }
}