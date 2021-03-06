﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AIMS.APIs.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DataBackupController : ControllerBase
    {
        IConfiguration configuration;
        //IDropboxService dropboxService;
        IWebHostEnvironment hostingEnvironment;
        IDataBackupService service;
        string connectionString = "";
        string clientUrl = "";

        public DataBackupController(IConfiguration config, IDataBackupService srvc, IWebHostEnvironment hostEnvironment)
        {
            configuration = config;
            service = srvc;
            //dropboxService = drpBoxService;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
            clientUrl = configuration["clientUrl"];
            hostingEnvironment = hostEnvironment;
            service.SetDirectoryPath(hostingEnvironment.WebRootPath);
        }

        [HttpGet("PerformBackup")]
        public async Task<IActionResult> PerformBackup()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string to database is not set in the configuration file.");
            }
            var response = await service.BackupData(connectionString);
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

        [HttpPost("DeleteBackup")]
        public IActionResult DeleteBackup([FromBody] DataBackupModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.DeleteBackupFile(model.FileName);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

    }
}