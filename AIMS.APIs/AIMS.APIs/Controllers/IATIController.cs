using System;
using System.Collections.Generic;
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
        IHostingEnvironment hostingEnvironment;

        public IATIController(IIATIService service, IConfiguration config, IHostingEnvironment _hostingEnvironment)
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
            string iatiUrl = configuration.GetValue<string>("IATI:Url");
            var response = await iatiService.DownloadIATIFromUrl(iatiUrl, iatiFilePath);
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


        [HttpGet]
        [Route("GetProjects")]
        public IActionResult Projects()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            var projects = iatiService.GetProjects(iatiFilePath);
            return Ok(projects);
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

        [HttpPost]
        [Route("SetIATISettings")]
        public IActionResult SetIATISettings([FromBody] IATISettings model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = iatiService.SaveIATISettings(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
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
            var activities = await iatiService.GetActivitiesByIds(iatiFilePath, Ids, tTypeFilePath);
            return Ok(activities);
        }
    }
}