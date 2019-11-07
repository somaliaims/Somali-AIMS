using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBackupController : ControllerBase
    {
        IConfiguration configuration;
        IDataBackupService service;

        public DataBackupController(IConfiguration config, IDataBackupService srvc)
        {
            configuration = config;
            service = srvc;
        }

        [HttpGet]
        public IActionResult DoBackup()
        {
            string connString = configuration["DefaultConnection"];
            if (string.IsNullOrEmpty(connString))
            {
                return BadRequest("Connection string to database is not set in the configuration file.");
            }
            var response = service.BackupData(connString);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

    }
}