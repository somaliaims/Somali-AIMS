using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationTypeController : ControllerBase
    {
        AIMSDbContext context;
        IOrganizationTypeService organizationTypeService;

        public OrganizationTypeController(AIMSDbContext cntxt, IOrganizationTypeService service)
        {
            this.context = cntxt;
            this.organizationTypeService = service;
        }
    }
}