using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
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
        IDropboxService dropboxService;
        IDataBackupService service;
        string connectionString = "";
        string clientUrl = "";

        public DataBackupController(IConfiguration config, IDataBackupService srvc, IDropboxService drpBoxService)
        {
            configuration = config;
            service = srvc;
            dropboxService = drpBoxService;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
            clientUrl = configuration["clientUrl"];
        }

        [HttpGet("PerformBackup")]
        public IActionResult PerformBackup()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string to database is not set in the configuration file.");
            }
            var response = service.BackupData(connectionString);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("PerformRestore")]
        public async Task<IActionResult> PerformRestore([FromBody] RestoreDatabaseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            /*var response = await dropboxService.DownloadFile(model.FileName);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }*/
            var response = await service.RestoreDatabase(model.FileName, connectionString);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpGet("GetBackupFiles")]
        public IActionResult GetBackupFiles()
        {
            return Ok(service.GetBackupFiles());
        }

    }
}