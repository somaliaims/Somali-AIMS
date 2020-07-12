using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AIMS.APIs.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        IProjectService projectService;
        IDataBackupService backupService;
        IWebHostEnvironment hostingEnvironment;
        IConfiguration config;
        IExchangeRateService ratesService;
        IExchangeRateHttpService ratesHttpService;
        ICurrencyService currencyService;
        string projectUrl = "";
        string connectionString = "";

        public ProjectController(IProjectService service, IWebHostEnvironment _hostingEnvironment, IConfiguration conf,
            IExchangeRateHttpService exRatesHttpService, IExchangeRateService exRatesService, ICurrencyService curService,
            IDataBackupService dataBackupService)
        {
            hostingEnvironment = _hostingEnvironment;
            projectService = service;
            backupService = dataBackupService;
            config = conf;
            ratesService = exRatesService;
            ratesHttpService = exRatesHttpService;
            currencyService = curService;
            projectUrl = config.GetValue<string>("ProjectUrl"); ;
            connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection"); ;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var projects = projectService.GetAll(exchangeRate);
            return Ok(projects);
        }

        [HttpGet("GetAllWithDetail")]
        public async Task<IActionResult> GetAllWithDetail()
        {
            return Ok(await projectService.GetAllDetailAsync());
        }

        //We will need to remove this, it is temporarily kept if a use case discovered
        /*[HttpGet("FixProjectCurrencyInDisbursements")]
        public async Task<IActionResult> FixProjectCurrencyInDisbursements()
        {
            return Ok(await projectService.FixProjectCurrencyInDisbursements());
        }*/

        [HttpGet("GetProjectTitles")]
        public IActionResult GetProjectTitles()
        {
            return Ok(projectService.GetProjectTitles());
        }

        [HttpGet("GetLatest")]
        public async Task<IActionResult> GetLatest()
        {
            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            return Ok(projectService.GetLatest(exchangeRate, defaultCurrency));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("UpdateFinancialYearsForProjects")]
        public IActionResult UpdateFinancialYearsForProjects()
        {
            /*var response = projectService.UpdateFinancialYearsForProjects();
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }*/
            return Ok(true);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var project = projectService.Get(id);
            return Ok(project);
        }

        [HttpGet]
        [Route("GetProjectProfileReport/{id}")]
        public async Task<IActionResult> GetProjectProfileReport(int id)
        {
            var projects = await projectService.GetProjectProfileReportAsync(id);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetProjectProfileReportBySector/{id}")]
        public async Task<IActionResult> GetProjectProfileReportBySector(int id)
        {
            var projects = await projectService.GetProjectsReportForSectorAsync(id);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetLocations/{id}")]
        public IActionResult GetLocations(int id)
        {
            var locations = projectService.GetProjectLocations(id);
            return Ok(locations);
        }

        [HttpGet]
        [Route("GetSectors/{id}")]
        public IActionResult GetSectors(int id)
        {
            var sectors = projectService.GetProjectSectors(id);
            return Ok(sectors);
        }

        [HttpGet]
        [Route("GetFunders/{id}")]
        public IActionResult GetFunders(int id)
        {
            var funders = projectService.GetProjectFunders(id);
            return Ok(funders);
        }

        [HttpGet]
        [Route("GetImplementers/{id}")]
        public IActionResult GetImplementers(int id)
        {
            var funders = projectService.GetProjectImplementers(id);
            return Ok(funders);
        }

        [HttpGet]
        [Route("GetDisbursements/{id}")]
        public async Task<IActionResult> GetDisbursements(int id)
        {
            return Ok(await projectService.GetProjectDisbursementsAsync(id));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("CreateDisbursements/{id}")]
        public async Task<IActionResult> CreateDisbursements(int id)
        {
            return Ok(await projectService.CreateProjectDisbursementsAsync(id));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("AdjustDisbursements/{id}")]
        public async Task<IActionResult> AdjustDisbursements(int id)
        {
            return Ok(await projectService.AdjustProjectDisbursementsAsync(id));
        }

        [HttpGet]
        [Route("GetDocuments/{id}")]
        public IActionResult GetDocuments(int id)
        {
            var documents = projectService.GetProjectDocuments(id);
            return Ok(documents);
        }

        [HttpGet("{criteria}")]
        public IActionResult Get(string criteria)
        {
            var projects = projectService.GetMatching(criteria);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetTitle/{id}")]
        public IActionResult GetTitle(int id)
        {
            var projectInfo = projectService.GetTitle(id);
            return Ok(projectInfo);
        }

        [HttpGet]
        [Route("GetOrganizationProjects/{id}")]
        public IActionResult GetOrganizationProjects(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var projects = projectService.GetOrganizationProjects(id);
            return Ok(projects);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetUserProjects")]
        public IActionResult GetUserProjects()
        {
            int organizationId = 0;
            string organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            if (organizationId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 0;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            var userProjects = projectService.GetUserProjects(userId, organizationId);
            return Ok(userProjects);
        }

        [HttpGet]
        [Route("GetLocationProjects/{id}")]
        public IActionResult GetLocationProjects(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var projects = projectService.GetLocationProjects(id);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetSectorProjects/{id}")]
        public IActionResult GetSectorProjects(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var projects = projectService.GetSectorProjects(id);
            return Ok(projects);
        }

        [HttpGet]
        [Route("GetMarkerProjects/{id}")]
        public IActionResult GetMarkerProjects(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var projects = projectService.GetMarkerProjects(id);
            return Ok(projects);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 0;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            project.ProjectUrl = projectUrl;
            decimal exchangeRate = 1;
            var dated = DateTime.Now;
            var rates = await ratesService.GetCurrencyRatesForDate(dated);
            if (rates.Rates == null)
            {
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                rates = await ratesHttpService.GetRatesAsync(apiKey);
                if (rates.Rates != null)
                {
                    ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                    exchangeRate = projectService.GetExchangeRateForCurrency(project.ProjectCurrency, rates.Rates);
                }
            }
            else
            {
                exchangeRate = projectService.GetExchangeRateForCurrency(project.ProjectCurrency, rates.Rates);
            }
            project.ExchangeRate = exchangeRate;
            var response = await projectService.AddAsync(project, userId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("MergeProjects")]
        public async Task<IActionResult> MergeProjects([FromBody] MergeProjectsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 0;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            decimal exchangeRate = 1;
            var dated = DateTime.Now;
            var rates = await ratesService.GetCurrencyRatesForDate(dated);
            if (rates.Rates == null)
            {
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                rates = await ratesHttpService.GetRatesAsync(apiKey);
                if (rates.Rates != null)
                {
                    ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                    exchangeRate = projectService.GetExchangeRateForCurrency(model.ProjectCurrency, rates.Rates);
                }
            }
            else
            {
                exchangeRate = projectService.GetExchangeRateForCurrency(model.ProjectCurrency, rates.Rates);
            }
            model.ExchangeRate = exchangeRate;
            var response = await projectService.MergeProjectsAsync(model, userId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ExtractProjectsByIds")]
        public async Task<IActionResult> ExtractProjectsByIds([FromBody] int[] ids)
        {
            if (ids.Count() == 0)
            {
                return Ok("[]");
            }
            var idsList = ids.ToList<int>();
            var projects = await projectService.GetProjectsByIdsAsync(idsList);
            return Ok(projects);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectLocation")]
        public async Task<IActionResult> AddProjectLocation([FromBody] ProjectLocationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await projectService.AddProjectLocation(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectSector")]
        public async Task<IActionResult> AddProjectSector([FromBody] ProjectSectorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await projectService.AddProjectSector(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectFunder")]
        public IActionResult AddProjectFunder([FromBody] ProjectFunderModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            model.ProjectUrl = projectUrl;
            var response = projectService.AddProjectFunder(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectFunderFromSource")]
        public IActionResult AddProjectFunderFromSource([FromBody] ProjectFunderSourceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            var response = projectService.AddProjectFunderFromSource(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectImplementerFromSource")]
        public IActionResult AddProjectImplementerFromSource([FromBody] ProjectImplementerSourceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int organizationId = 0;
            var organizationIdVal = User.FindFirst(ClaimTypes.Country)?.Value;
            if (!string.IsNullOrEmpty(organizationIdVal))
            {
                organizationId = Convert.ToInt32(organizationIdVal);
            }
            var response = projectService.AddProjectImplementerFromSource(model, organizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectImplementer")]
        public IActionResult AddProjectImplementer([FromBody] ProjectImplementerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.ProjectUrl = projectUrl;
            var response = projectService.AddProjectImplementer(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("AdjustDisbursementsForProjects")]
        public async Task<IActionResult> AdjustDisbursementsForProjects()
        {
            string userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 0;
            if (!string.IsNullOrEmpty(userIdVal))
            {
                userId = Convert.ToInt32(userIdVal);
            }
            if (userId == 0)
            {
                return BadRequest("Unauthorized user access to api");
            }
            var result = await backupService.BackupData(connectionString);
            var response = await projectService.AdjustDisbursementsForProjectsAsync(userId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpGet("GetActiveProjectsCount")]
        public IActionResult GetActiveProjectsCount()
        {
            return Ok(projectService.GetActiveProjectsCount());
        }

        [HttpGet("GetCurrentYearDisbursements")]
        public IActionResult GetCurrentYearDisbursements()
        {
            return Ok(projectService.GetCurrentYearDisbursements());
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectDisbursement")]
        public async Task<IActionResult> AddProjectDisbursement([FromBody] ProjectDisbursementModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await projectService.AddProjectDisbursement(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectDocument")]
        public IActionResult AddProjectDocument([FromBody] ProjectDocumentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddProjectDocument(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProjectMarker")]
        public IActionResult AddProjectMarker([FromBody] ProjectMarkerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddUpdateProjectMarker(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut]
        [Route("EditProjectMarker")]
        public IActionResult EditProjectMarker([FromBody] ProjectMarkerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectService.AddUpdateProjectMarker(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("SearchProjectsByCriteria")]
        public async Task<IActionResult> SearchProjectsByCriteria([FromBody] SearchProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var projects = await projectService.SearchProjectsViewByCriteria(model, exchangeRate);
            return Ok(projects);
        }

        [HttpPost]
        [Route("SearchProjectsViewByCriteria")]
        public async Task<IActionResult> SearchProjectsViewByCriteria([FromBody] SearchProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var defaultCurrencyObj = currencyService.GetDefaultCurrency();
            if (defaultCurrencyObj == null)
            {
                return BadRequest("Default currency is not set. Please contact administrator");
            }

            string defaultCurrency = defaultCurrencyObj.Currency;
            decimal exchangeRate = 1;
            if (!string.IsNullOrEmpty(defaultCurrency))
            {
                var dated = DateTime.Now;
                var rates = await ratesService.GetCurrencyRatesForDate(dated);
                if (rates.Rates == null)
                {
                    string apiKey = ratesService.GetAPIKeyForOpenExchange();
                    rates = await ratesHttpService.GetRatesAsync(apiKey);
                    if (rates.Rates != null)
                    {
                        ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                        exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                    }
                }
                else
                {
                    exchangeRate = projectService.GetExchangeRateForCurrency(defaultCurrency, rates.Rates);
                }
            }
            var projects = await projectService.SearchProjectsViewByCriteria(model, exchangeRate);
            return Ok(projects);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            decimal exchangeRate = 1;
            var dated = DateTime.Now;
            var rates = await ratesService.GetCurrencyRatesForDate(dated);
            if (rates.Rates == null)
            {
                string apiKey = ratesService.GetAPIKeyForOpenExchange();
                rates = await ratesHttpService.GetRatesAsync(apiKey);
                if (rates.Rates != null)
                {
                    ratesService.SaveCurrencyRates(rates.Rates, DateTime.Now);
                    exchangeRate = projectService.GetExchangeRateForCurrency(project.ProjectCurrency, rates.Rates);
                }
            }
            else
            {
                exchangeRate = projectService.GetExchangeRateForCurrency(project.ProjectCurrency, rates.Rates);
            }
            project.ExchangeRate = exchangeRate;
            var response = await projectService.UpdateAsync(id, project);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectLocation/{projectId}/{locationId}")]
        public async Task<IActionResult> DeleteProjectLocation(int projectId, int locationId)
        {
            if (projectId <= 0 || locationId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            var response = await projectService.DeleteProjectLocationAsync(projectId, locationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectSector/{projectId}/{sectorId}")]
        public async Task<IActionResult> DeleteProjectSector(int projectId, int sectorId)
        {
            if (projectId <= 0 || sectorId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            var response = await projectService.DeleteProjectSectorAsync(projectId, sectorId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectFunder/{projectId}/{funderId}")]
        public IActionResult DeleteProjectFunder(int projectId, int funderId)
        {
            if (projectId <= 0 || funderId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            var response  = projectService.DeleteProjectFunder(projectId, funderId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectImplementer/{projectId}/{implementerId}")]
        public IActionResult DeleteProjectImplementer(int projectId, int implementerId)
        {
            if (projectId <= 0 || implementerId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }
            var response = projectService.DeleteProjectImplementer(projectId, implementerId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectMarker/{projectId}/{customFieldId}")]
        public IActionResult DeleteProjectMarker(int projectId, int customFieldId)
        {
            if (projectId <= 0 || customFieldId <= 0)
            {
                return BadRequest("Invalid Ids provided");
            }

            var response = projectService.DeleteProjectMarker(projectId, customFieldId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectDisbursement/{id}")]
        public IActionResult DeleteProjectDisbursement(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id provided");
            }
            var response = projectService.DeleteProjectDisbursement(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete]
        [Route("DeleteProjectDocument/{id}")]
        public IActionResult DeleteProjectDocument(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id provided");
            }
            var response = projectService.DeleteProjectDocument(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}