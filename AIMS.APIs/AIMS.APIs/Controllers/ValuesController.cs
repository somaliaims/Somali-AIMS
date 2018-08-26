using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// Retrieves the values
        /// </summary>
        /// <returns>Returns a string array</returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Get the string
        /// </summary>
        /// <param name="id">Id to be passed as integer</param>
        /// <returns>Returns a string</returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="value">Input string</param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id">input id</param>
        /// <param name="value">Object</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">Pass id to delete</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
