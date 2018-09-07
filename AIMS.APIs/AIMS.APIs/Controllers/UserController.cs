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
    public class UserController : ControllerBase
    {
        AIMSDbContext context;
        IUserService userService;

        public UserController(AIMSDbContext cntxt, IUserService service)
        {
            this.context = cntxt;
            this.userService = service;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = userService.Add(user);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}