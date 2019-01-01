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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IATIController : ControllerBase
    {
        IIATIService iatiService;
        //IDistributedCache cache;
        IConfiguration configuration;
        //double cacheIntervalInSeconds = 300;
        IHostingEnvironment hostingEnvironment;

        public IATIController(IIATIService service, IConfiguration config, IHostingEnvironment _hostingEnvironment)
        {
            iatiService = service;
            hostingEnvironment = _hostingEnvironment;
            //cache = distributedCache;
            configuration = config;
            /*string expirationTimeInSecondsStr = configuration.GetSection("Caching:ExpirationTimeInSeconds").Value;
            if (!string.IsNullOrEmpty(expirationTimeInSecondsStr))
            {
                cacheIntervalInSeconds = Convert.ToDouble(expirationTimeInSecondsStr);
            }*/
        }

        [HttpGet]
        [Route("GetActivities")]
        public IActionResult GetActivities()
        {
            //var cachedActivities = await cache.GetAsync("iati-activities");
            IEnumerable<IATIActivity> iatiActivities = new List<IATIActivity>();
            //if (cachedActivities == null)
            //{
            iatiActivities = iatiService.GetAll();
            int counter = 1;
            foreach(var activity in iatiActivities)
            {
                activity.Id = counter;
                ++counter;
            }
            //var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(cacheIntervalInSeconds));
            //string activitiesStr = JsonConvert.SerializeObject(iatiActivities);
            //byte[] encodedActivities = Encoding.UTF8.GetBytes(activitiesStr);
            //await cache.SetAsync("iati-activities", encodedActivities, options);
            /*}
            else
            {
                byte[] encodedActivities = await cache.GetAsync("iati-activities");
                string activitiesStr = Encoding.UTF8.GetString(encodedActivities);
                iatiActivities = (List<IATIActivity>)JsonConvert.DeserializeObject(activitiesStr);
            }*/
            return Ok(iatiActivities);
        }

        [HttpGet]
        [Route("LoadLatestIATI")]
        public IActionResult LoadLatestIATI()
        {
            string iatiFilePath = hostingEnvironment.WebRootPath + "/IATISomali.xml";
            string country = configuration.GetValue<string>("IATI:Country");
            var response = iatiService.LoadLatestIATI(country, iatiFilePath);
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
            var activities = iatiService.GetMatchingTitleDescriptions(keywords);
            return Ok(activities);
        }
    }
}