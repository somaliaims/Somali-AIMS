using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using AIMS.APIs.Utilities;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using AIMS.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        AIMSDbContext context;
        IUserService userService;
        IConfiguration configuration;

        public UserController(AIMSDbContext cntxt, IUserService service, IConfiguration config)
        {
            this.context = cntxt;
            this.userService = service;
            this.configuration = config;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string adminEmail = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                .GetValue<String>("Email:Smtp:AdminEmail");

            var configuredSmtpClient = HttpContext.RequestServices.GetRequiredService<SmtpClient>();
            var response = userService.Add(user, configuredSmtpClient, adminEmail);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("ResetPasswordRequest")]
        public IActionResult ResetPasswordRequest([FromBody] PasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Valid email address is required");
            }

            var foundUser = userService.GetUserByEmail(model.Email);
            if (foundUser != null)
            {
                string adminEmail = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                .GetValue<String>("Email:Smtp:AdminEmail");
                string resetPasswordUrl = configuration["ResetPasswordUrl"];
                var configuredSmtpClient = HttpContext.RequestServices.GetRequiredService<SmtpClient>();

                TokenModel tModel = new TokenModel()
                {
                    Id = foundUser.Id.ToString(),
                    JwtKey = configuration["JwtKey"],
                    JwtAudience = configuration["JwtAudience"],
                    JwtIssuer = configuration["JwtIssuer"],
                    TokenExpirationDays = configuration["JwtExpireDays"],
                    OrganizationId = foundUser.OrganizationId.ToString(),
                    UserType = Convert.ToInt32(foundUser.UserType),
                    Email = foundUser.Email
                };

                TokenUtility utility = new TokenUtility();
                string token = utility.GeneratePasswordResetToken(tModel);
                PasswordResetEmailModel resetModel = new PasswordResetEmailModel()
                {
                    FullName = foundUser.Name,
                    Email = foundUser.Email,
                    Token = token,
                    Url = resetPasswordUrl
                };

                var response = userService.ResetPasswordRequest(resetModel, configuredSmtpClient, adminEmail);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
            }
            return Ok(true);
        }

        [HttpPost]
        [Route("Token")]
        public IActionResult Token([FromBody] AuthenticateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SecurityHelper sHelper = new SecurityHelper();
            model.Password = sHelper.GetPasswordHash(model.Password);
            var foundUser = userService.AuthenticateUser(model.Email, model.Password);

            if (!string.IsNullOrEmpty(foundUser.Name))
            {
                TokenModel tModel = new TokenModel()
                {
                    Id = foundUser.Id.ToString(),
                    JwtKey = configuration["JwtKey"],
                    JwtAudience = configuration["JwtAudience"],
                    JwtIssuer = configuration["JwtIssuer"],
                    TokenExpirationDays = configuration["JwtExpireDays"],
                    OrganizationId = foundUser.OrganizationId.ToString(),
                    UserType = Convert.ToInt32(foundUser.UserType),
                    Email = foundUser.Email
                };
                TokenUtility tManager = new TokenUtility();
                var jwtToken = tManager.GenerateToken(tModel);
                UserReturnView uView = new UserReturnView()
                {
                    Token = jwtToken,
                    Name = foundUser.Name,
                    UserType = foundUser.UserType
                };
                return Ok(uView);
            }
            ErrorModel errModel = new ErrorModel() { Error = "Username/Password provided is invalid" };
            return Ok(errModel);
        }

        [HttpPost]
        [Route("Activate")]
        public IActionResult Activate([FromBody] UserApprovalModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = userService.ActivateUserAccount(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            SuccessModel successModel = new SuccessModel() { Message = response.ReturnedId.ToString() };
            return Ok(successModel);
        }
        
        [HttpGet]
        [Route("[action]/{email}")]
        public IActionResult CheckEmailAvailability(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email value cannot be null");
            }
            var response = userService.CheckEmailAvailability(email);
            return Ok(response.Success);
        }

        [HttpPost]
        [Route("EditUserOrganization")]
        public IActionResult EditUserOrganization([FromBody] EditUserOrganization model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = userService.UpdateOrganization(model.UserId, model.OrganizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }

        [HttpPost]
        [Route("EditPassword")]
        public IActionResult EditPassword([FromBody] EditUserPassword model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = userService.UpdatePassword(model.UserId, model.Password);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }

        [HttpPost]
        [Route("ActivateAccount")]
        public IActionResult ActivateAccount([FromBody] UserApprovalModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdVal))
            {
                return BadRequest("Invalid attempt");
            }

            int approvedById = Convert.ToInt32(userIdVal);
            model.ApprovedById = approvedById;
            var response = userService.ActivateUserAccount(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }
    }
}