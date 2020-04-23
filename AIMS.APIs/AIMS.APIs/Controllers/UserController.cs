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
using Microsoft.AspNetCore.Authorization;
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
        IUserService userService;
        IConfiguration configuration;
        string clientUrl = "";
        string defaultUserEmail = "";

        public UserController(IUserService service, IConfiguration config)
        {
            userService = service;
            configuration = config;
            clientUrl = configuration["ClientUrl"];
            defaultUserEmail = configuration["DefaultUserEmail"];
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetStandardUsers")]
        public IActionResult GetStandardUsers()
        {
            var users = userService.GetStandardUsers();
            return Ok(users);
        }

        [HttpGet("GetActiveUsersCount")]
        public IActionResult GetActiveUsersCount()
        {
            return Ok(userService.GetActiveUserCount());
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetManagerUsers")]
        public IActionResult GetManagerUsers()
        {
            var users = userService.GetManagerUsers();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string adminEmail = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                                .GetValue<String>("Email:Smtp:AdminEmail");
            string apiUrl = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                                .GetValue<String>("Email:Smtp:AdminEmail");
            var response = await userService.AddAsync(user, adminEmail);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("ResetPasswordRequest")]
        public IActionResult ResetPasswordRequest([FromBody] PasswordResetRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Valid email address is required");
            }

            ActionResponse response = new ActionResponse();
            var foundUser = userService.GetUserByEmail(model.Email);
            if (foundUser != null)
            {
                string adminEmail = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                .GetValue<String>("Email:Smtp:AdminEmail");
                string resetPasswordUrl = configuration["ResetPasswordUrl"];
                DateTime datedTime = DateTime.Now;
                PasswordTokenModel tModel = new PasswordTokenModel()
                {
                    Email = foundUser.Email,
                    TokenDate = datedTime
                };

                TokenUtility utility = new TokenUtility();
                string token = utility.GeneratePasswordResetToken(tModel);
                PasswordResetEmailModel resetModel = new PasswordResetEmailModel()
                {
                    Email = foundUser.Email,
                    Token = token,
                    Url = resetPasswordUrl
                };

                response = userService.ResetPasswordRequest(resetModel, datedTime, adminEmail);
            }
            return Ok(response);
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

            if (foundUser.OrganizationId != 0 && !foundUser.IsApproved)
            {
                UserReturnView uView = new UserReturnView()
                {
                    Id = foundUser.Id,
                    Email = foundUser.Email,
                    Token = null,
                    UserType = 0,
                    OrganizationId = foundUser.OrganizationId,
                    OrganizationName = foundUser.OrganizationName,
                    IsApproved = false
                };
                return Ok(uView);
            }
            else if (!string.IsNullOrEmpty(foundUser.Email))
            {
                TokenModel tModel = new TokenModel()
                {
                    Id = foundUser.Id.ToString(),
                    JwtKey = configuration["JwtKey"],
                    JwtAudience = configuration["JwtAudience"],
                    JwtIssuer = configuration["JwtIssuer"],
                    TokenExpirationDays = configuration["JwtExpireDays"],
                    OrganizationId = foundUser.OrganizationId.ToString(),
                    OrganizationName = foundUser.OrganizationName,
                    UserType = Convert.ToInt32(foundUser.UserType),
                    Email = foundUser.Email
                };
                TokenUtility tManager = new TokenUtility();
                var jwtToken = tManager.GenerateToken(tModel);
                UserReturnView uView = new UserReturnView()
                {
                    Id = foundUser.Id,
                    Token = jwtToken,
                    Email = foundUser.Email,
                    UserType = foundUser.UserType,
                    OrganizationId = foundUser.OrganizationId,
                    OrganizationName = foundUser.OrganizationName,
                    IsApproved = foundUser.IsApproved
                };
                return Ok(uView);
            }
            
            ErrorModel errModel = new ErrorModel() { Error = "Username/Password provided is invalid" };
            return Ok(errModel);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("Activate")]
        public async Task<IActionResult> Activate([FromBody] UserApprovalModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string loginUrl = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                                .GetValue<String>("LoginUrl");
            var response = await userService.ActivateUserAccountAsync(model, loginUrl, defaultUserEmail);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            SuccessModel successModel = new SuccessModel() { Message = response.ReturnedId.ToString() };
            return Ok(successModel);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("PromoteUser/{id}")]
        public IActionResult PromoteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user id provided");
            }
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdVal))
            {
                return BadRequest("Invalid attempt");
            }
            int loggedInUserId = Convert.ToInt32(userIdVal);
            if (id == loggedInUserId)
            {
                return BadRequest("You cannot promote your own account");
            }
            var response = userService.PromoteUser(id, loggedInUserId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("DemoteUser/{id}")]
        public IActionResult DemoteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user id provided");
            }
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdVal))
            {
                return BadRequest("Invalid attempt");
            }
            int loggedInUserId = Convert.ToInt32(userIdVal);
            if (id == loggedInUserId)
            {
                return BadRequest("You cannot demote your own account");
            }
            var response = userService.DemoteUser(id, loggedInUserId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
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
            return Ok(response);
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
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TokenUtility utility = new TokenUtility();
            var tokenTime = utility.GetDecodedResetToken(model.Token);
            var response = await userService.ResetPasswordAsync(model, tokenTime);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("EditPassword")]
        public IActionResult EditPassword([FromBody] EditUserPassword model)
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

            int userId = Convert.ToInt32(userIdVal);
            var response = userService.UpdatePassword(userId, model.Password);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount([FromBody] UserApprovalModel model)
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
            string loginUrl = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                                .GetValue<String>("LoginUrl");
            var response = await userService.ActivateUserAccountAsync(model, loginUrl, defaultUserEmail);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }

        [HttpPost]
        [Route("DeleteAccount")]
        public IActionResult Delete(DeleteAccountModel model)
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
            int userId = Convert.ToInt32(userIdVal);
            var response = userService.Delete(userId, model.Password);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}