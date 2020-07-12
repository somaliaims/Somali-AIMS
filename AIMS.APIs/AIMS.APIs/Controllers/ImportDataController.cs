using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AIMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportDataController : ControllerBase
    {
        IDataImportService service;
        IProjectService projectService;
        IEnvelopeService envelopeService;
        IOrganizationTypeService organizationTypeService;
        IExchangeRateService ratesService;
        IExchangeRateHttpService ratesHttpService;
        IWebHostEnvironment hostingEnvironment;

        public ImportDataController(IDataImportService dataImportService, IProjectService projService, 
            IEnvelopeService envpService, IOrganizationTypeService orgTypeService, IExchangeRateService exRateService,
            IExchangeRateHttpService exRateHttpService, IWebHostEnvironment hostEnvironment)
        {
            service = dataImportService;
            projectService = projService;
            envelopeService = envpService;
            organizationTypeService = orgTypeService;
            ratesService = exRateService;
            ratesHttpService = exRateHttpService;
            hostingEnvironment = hostEnvironment;
            service.SetDirectoryPath(hostingEnvironment.WebRootPath);
        }

        /*[HttpPost("UploadDataImportFileEighteen"), DisableRequestSizeLimit]
        public IActionResult UploadDataImportFileEighteen()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "DataImportFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Directory.CreateDirectory(pathToSave);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var extractedProjects = service.ImportAidDataEighteen(filePath, file); 
                    return Ok(extractedProjects);
                }
                else
                {
                    return BadRequest("Invalid data file provided");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UploadDataImportFileSeventeen"), DisableRequestSizeLimit]
        public IActionResult UploadDataImportFileSeventeen()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "DataImportFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Directory.CreateDirectory(pathToSave);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var extractedProjects = service.ImportAidDataSeventeen(filePath);
                    return Ok(extractedProjects);
                }
                else
                {
                    return BadRequest("Invalid data file provided");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ImportLatestData"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportLatestData()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "DataImportFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Directory.CreateDirectory(pathToSave);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    var ratesView = await ratesHttpService.GetRatesAsync(apiKey);
                    var extractedProjects = service.ImportLatestAidData(filePath, file, ratesView.Rates);
                    if (extractedProjects.Count > 0)
                    {
                        var response = await projectService.ImportProjects(extractedProjects);
                        if (!response.Success)
                        {
                            return BadRequest(response.Message);
                        }
                    }
                    return Ok(true);
                }
                else
                {
                    return BadRequest("Invalid data file provided");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ImportEnvelopeData"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportEnvelopeData()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "DataImportFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Directory.CreateDirectory(pathToSave);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    var ratesView = await ratesHttpService.GetRatesAsync(apiKey);
                    var envelopeList = service.ImportEnvelopeData(filePath, file, ratesView.Rates);
                    if (envelopeList.Count > 0)
                    {
                        var response = await envelopeService.ImportEnvelopeData(envelopeList);
                        if (!response.Success)
                        {
                            return BadRequest(response.Message);
                        }
                    }
                    return Ok(envelopeList);
                }
                else
                {
                    return BadRequest("Invalid data file provided");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ImportOrganizationTypes"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportOrganizationTypes()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "DataImportFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Directory.CreateDirectory(pathToSave);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var organizationsList = service.ImportOrganizations(filePath, file);
                    if (organizationsList.Count > 0)
                    {
                        var response = await organizationTypeService.ImportOrganizationAndTypes(organizationsList);
                        if (!response.Success)
                        {
                            return BadRequest(response.Message);
                        }
                    }
                    return Ok(organizationsList);
                }
                else
                {
                    return BadRequest("Invalid data file provided");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetMatchesForImportedData")]
        public IActionResult GetMatchesForImportedData()
        {
            var folderName = Path.Combine("wwwroot", "DataImportFiles");
            var pathToFiles = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var dataResult = service.GetMatchForOldNewData(pathToFiles);
            return Ok(dataResult);
        }

        [HttpGet("GenerateExcelForActiveProjects")]
        public IActionResult GenerateExcelForActiveProjects()
        {
            var folderName = Path.Combine("wwwroot", "DataImportFiles");
            var pathToFiles = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            string fileName = service.GenerateExcelFileForActiveProjects(pathToFiles);
            return Ok(fileName);
        }*/


    }
}