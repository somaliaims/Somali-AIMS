using AIMS.APIs.Helpers;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactMessageController : ControllerBase
    {
        IContactMessageService contactService;
        IUserService userService;
        IEmailService emailService;

        public ContactMessageController(IContactMessageService cntctService, IEmailService eService, IUserService uService)
        {
            contactService = cntctService;
            userService = uService;
            emailService = eService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(contactService.GetAll());
        }

        [HttpPost("[action]/{id}")]
        public IActionResult Approve(int id)
        {
            if (id <= 0)
            {
                ICommonMessageHelper messageHelper = new CommonMessageHelper();
                return BadRequest(messageHelper.GetInvalidIdMessage());
            }
            var response = contactService.Approve(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            else if(response.Success)
            {
                ContactEmailRequestModel model = JsonConvert.DeserializeObject<ContactEmailRequestModel>(response.Message);
                List<EmailAddress> usersEmails = null;

                if (model.ProjectId <= 0)
                {
                    return BadRequest("Invalid project id provided");
                }
                int projectId = (int)model.ProjectId;
                usersEmails = contactService.GetProjectUsersEmails(projectId).ToList();
                EmailModel emailModel = new EmailModel()
                {
                    Subject = model.Subject,
                    Message = model.Message,
                    Title = model.ProjectTitle,
                    EmailsList = usersEmails
                };
                response = emailService.SendContactEmail(emailModel, model.SenderName, model.SenderEmail, model.ProjectTitle, model.EmailType);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                response = contactService.Delete(id);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                response.ReturnedId = 1;
            }
            return Ok(true);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ContactEmailRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(model.SenderEmail))
            {
                model.SenderEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(model.SenderEmail))
                {
                    return BadRequest("Unauthorized user access to api");
                }
            }

            ActionResponse response = new ActionResponse();
            var contactToVerify = userService.CheckEmailAvailability(model.SenderEmail);
            if (!contactToVerify.Success)
            {
                List<EmailAddress> usersEmails = null;
                if (model.EmailType == ContactEmailType.Help)
                {
                    usersEmails = contactService.GetManagerUsersEmails().ToList();
                }
                else if (model.EmailType == ContactEmailType.Information)
                {
                    if (model.ProjectId <= 0)
                    {
                        return BadRequest("Invalid project id provided");
                    }
                    int projectId = (int)model.ProjectId;
                    usersEmails = contactService.GetProjectUsersEmails(projectId).ToList();
                }
                EmailModel emailModel = new EmailModel()
                {
                    Subject = model.Subject,
                    Message = model.Message,
                    Title = model.ProjectTitle,
                    EmailsList = usersEmails
                };
                response = emailService.SendContactEmail(emailModel, model.SenderName, model.SenderEmail, model.ProjectTitle, model.EmailType);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                response.ReturnedId = 1;
                response.Message = "Message sent";
            }
            else
            {
                ContactMessageModel contactModel = new ContactMessageModel()
                {
                    SenderEmail = model.SenderEmail,
                    SenderName = model.SenderName,
                    Subject = model.Subject,
                    Message = model.Message,
                    EmailType = model.EmailType,
                    ProjectId = (int)model.ProjectId
                };
                response = contactService.Add(contactModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                response.ReturnedId = 2;
                response.Message = "Message forwarded";
            }
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }
            var response = contactService.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}
