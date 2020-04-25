using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.APIs.Helpers;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        IHomePageService service;

        public HomePageController(IHomePageService homePageService)
        {
            service = homePageService;
        }

        [HttpGet("GetSettings")]
        public IActionResult Get()
        {
            return Ok(service.GetSettings());
        }

        [HttpPost("SetSettings")]
        public IActionResult SetSettings([FromBody] HomePageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.SetSettings(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("SetFavicon"), DisableRequestSizeLimit]
        public async Task<IActionResult> SetFavicon()
        {
            ActionResponse response = new ActionResponse();
            try
            {
                IImageHelper imageHelper = new ImageHelper();
                var file = Request.Form.Files[0];
                var fileValidity = imageHelper.IsImageFormatValid(file);
                if (!fileValidity.IsImageValid)
                {
                    return BadRequest("Image file provided with invalid format. Valid image formats are (bmp,jpeg,gif,tiff,png,)");
                }
                if (!fileValidity.IsImageSizeValid)
                {
                    return BadRequest("Image file cannot be greater than 2 MB in size.");
                }
                response = await service.SetFaviconAsync(file);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
    }
}