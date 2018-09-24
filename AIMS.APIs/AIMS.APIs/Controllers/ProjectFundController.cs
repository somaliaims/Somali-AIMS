using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectFundController : ControllerBase
    {
        AIMSDbContext context;
        IProjectFundsService projectFundService;

        public ProjectFundController(AIMSDbContext cntxt, IProjectFundsService service)
        {
            this.context = cntxt;
            this.projectFundService = service;
        }


        /// <summary>
        /// Gets fundings for the provided project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var projectFunds = projectFundService.GetAll(id);
            return Ok(projectFunds);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectFundsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectFundService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("UpdateProjectFunds")]
        public IActionResult RemoveFunder([FromBody] ProjectFundsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectFundService.Update(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok("1");
        }
    }
}