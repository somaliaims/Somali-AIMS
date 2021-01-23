using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS.IATILib.Parsers;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IATIController : ControllerBase
    {
        IIATIService iatiService;
        IConfiguration configuration;
        IWebHostEnvironment hostingEnvironment;

        public IATIController(IIATIService service, IConfiguration config, IWebHostEnvironment _hostingEnvironment)
        {
            iatiService = service;
            hostingEnvironment = _hostingEnvironment;
            configuration = config;
        }

        [HttpGet]
        [Route("GetActivities")]
        public IActionResult GetActivities()
        {
            IEnumerable<IATIActivity> iatiActivities = new List<IATIActivity>();
            iatiActivities = iatiService.GetAll();
            int counter = 1;
            foreach(var activity in iatiActivities)
            {
                activity.Id = counter;
                ++counter;
            }
            return Ok(iatiActivities);
        }

        [HttpGet]
        [Route("LoadLatestIATI")]
        public async Task<IActionResult> LoadLatestIATI()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            string sectorVocabPath = hostingEnvironment.WebRootPath + "/IATISectorVocabulary.json";
            var response = await iatiService.DownloadLatestIATIAsync(iatiFilePath);
            //response = iatiService.ExtractAndSaveIATISectors(iatiFilePath, sectorVocabPath);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("LoadIATITransactionTypes")]
        public async Task<IActionResult> LoadIATITransactionTypes()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATITransactionTypes.json";
            string iatiUrl = configuration.GetValue<string>("IATI:TransactionTypesUrl");
            var response = await iatiService.DownloadTransactionTypesFromUrl(iatiUrl, iatiFilePath);
            return Ok(response);
        }

        /*[HttpGet]
        [Route("AmendSectorLabels")]
        public IActionResult AmendSectorLabels()
        {
            string sectorsFilePath = hostingEnvironment.WebRootPath + "/Sectors.xml";
            var response = iatiService.NameSectorsCorrectly(sectorsFilePath);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }*/

        [HttpGet]
        [Route("GetProjects")]
        public IActionResult Projects()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            FileInfo fileInfo = new FileInfo(iatiFilePath);
            if (!fileInfo.Exists)
            {
                return BadRequest("IATI source file not found. Load latest IATI before making this request.");
            }
            var projects = iatiService.GetProjects(iatiFilePath);
            return Ok(projects);
        }

        [HttpGet]
        [Route("SaveSectors")]
        public IActionResult SaveSectors()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            string sectorVocabPath = hostingEnvironment.WebRootPath + "/IATISectorVocabulary.json";
            FileInfo fileInfo = new FileInfo(iatiFilePath);
            if (!fileInfo.Exists)
            {
                return BadRequest("IATI source file not found. Load latest IATI before making this request.");
            }
            var response = iatiService.ExtractAndSaveIATISectors(iatiFilePath, sectorVocabPath);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetOrganizations")]
        public IActionResult GetOrganizations()
        {
            var organizations = iatiService.GetOrganizations();
            return Ok(organizations);
        }

        [HttpGet]
        [Route("GetMatchingActivities/{keywords}")]
        public IActionResult GetMatchingActivities(string keywords)
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            var activities = iatiService.GetMatchingIATIActivities(iatiFilePath, keywords);
            return Ok(activities);
        }

        [HttpGet]
        [Route("GetIATISettings")]
        public IActionResult GetIATISettings()
        {
            var settings = iatiService.GetIATISettings();
            return Ok(settings);
        }

        [HttpGet]
        [Route("GetIATISettingsList")]
        public IActionResult GetIATISettingsList()
        {
            return Ok(iatiService.GetIATISettingsList());
        }

        [HttpPost("FixInactiveOrganizations")]  
        public IActionResult FixInactiveOrganizations()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            var response = iatiService.DeleteInActiveOrganizations(iatiFilePath);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("SetIATISettings")]
        public async Task<IActionResult> SetIATISettings([FromBody] IATISettings model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.IATIFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            var response = await iatiService.SaveIATISettingsAsync(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("ExtractProjectsByIds")]
        public async Task<IActionResult> ExtractProjectsByIds([FromBody] List<IATIByIdModel> Ids)
        {
            if (Ids.Count() == 0)
            {
                return Ok("[]");
            }

            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            string tTypeFilePath = hostingEnvironment.WebRootPath + "/IATITransactionTypes.json";
            string fTypeFilePath = hostingEnvironment.WebRootPath + "/IATIFinanceTypes.json";
            var activities = await iatiService.GetActivitiesByIds(iatiFilePath, Ids, tTypeFilePath, fTypeFilePath);
            return Ok(activities);
        }
    }
}