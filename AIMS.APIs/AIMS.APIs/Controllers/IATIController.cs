using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
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
        IDistributedCache cache;
        IConfiguration configuration;
        double cacheIntervalInSeconds = 300;

        public IATIController(IIATIService service, IDistributedCache distributedCache, IConfiguration config)
        {
            iatiService = service;
            cache = distributedCache;
            configuration = config;
            string expirationTimeInSecondsStr = configuration.GetSection("Caching:ExpirationTimeInSeconds").Value;
            if (!string.IsNullOrEmpty(expirationTimeInSecondsStr))
            {
                cacheIntervalInSeconds = Convert.ToDouble(expirationTimeInSecondsStr);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cachedActivities = await cache.GetAsync("iati-activities");
            ICollection<IATIActivity> iatiActivities = new List<IATIActivity>();
            if (cachedActivities == null)
            {
                iatiActivities = iatiService.GetAll();
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(cacheIntervalInSeconds));
                string activitiesStr = JsonConvert.SerializeObject(iatiActivities);
                byte[] encodedActivities = Encoding.UTF8.GetBytes(activitiesStr);
                await cache.SetAsync("iati-activities", encodedActivities, options);
            }
            else
            {
                byte[] encodedActivities = await cache.GetAsync("iati-activities");
                string activitiesStr = Encoding.UTF8.GetString(encodedActivities);
                iatiActivities = (List<IATIActivity>)JsonConvert.DeserializeObject(activitiesStr);
            }
            return Ok(iatiActivities);
        }
    }
}