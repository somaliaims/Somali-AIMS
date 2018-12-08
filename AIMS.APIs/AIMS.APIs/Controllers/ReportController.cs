using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IViewRenderService _viewRenderService;
        public ReportController(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var viewModel = new ViewModel
            {
               Title = "A good Title"
            };

            //var result = await _viewRenderService.RenderToStringAsync("Email/Invite", viewModel);*/
            var result = await _viewRenderService.RenderToStringAsync("Reports/Index", viewModel);
            return Ok(result);
        }
    }
}