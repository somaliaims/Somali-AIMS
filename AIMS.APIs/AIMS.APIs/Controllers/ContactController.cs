using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        IContactService service;
        IEmailService emailService;

        public ContactController(IContactService contactService, IEmailService eService)
        {
            service = contactService;
            emailService = eService;
        }

        [HttpPost("SendSuggestionEmailForProject")]
        public IActionResult SendSuggestionEmailForProject([FromBody] ProjectHelpEmail model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data is not in valid format");
            }
            if (string.IsNullOrEmpty(model.SenderEmail))
            {
                model.SenderEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(model.SenderEmail))
                {
                    return BadRequest("Unauthorized user access to api");
                }
            }
            List<EmailAddress> emailsList = new List<EmailAddress>();
            emailsList = service.GetProjectUsersEmails(model.ProjectId).ToList();
            string messageTail = "<p><b>Sender name: " + model.SenderName + "</b><br/><b>Sender email: " + model.SenderEmail + "</b></p>";
            model.Message = "<p>" + model.Message + "</p>" + messageTail;
            EmailModel emailModel = new EmailModel()
            {
                EmailsList = emailsList,
                Title = emailService.GetTextForSuggestionType(model.suggesstionType, model.ProjectTitle),
                Subject = "Suggestion to improve AIMS project data",
                Message = model.Message,
            };
            ActionResponse response = emailService.SendEmailToUsers(emailModel);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ContactEmailRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data is not in valid format");
            }

            if (string.IsNullOrEmpty(model.SenderEmail))
            {
                model.SenderEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(model.SenderEmail))
                {
                    return BadRequest("Unauthorized user access to api");
                }
            }
            List<EmailAddress> usersEmails = null;
            if (model.EmailType == ContactEmailType.Help)
            {
                usersEmails = service.GetManagerUsersEmails().ToList();
            }
            else if(model.EmailType == ContactEmailType.Information)
            {
                if (model.ProjectId <= 0)
                {
                    return BadRequest("Invalid project id provided");
                }
                int projectId = (int)model.ProjectId;
                usersEmails = service.GetProjectUsersEmails(projectId).ToList();
            }

            ActionResponse response = null;
            if (usersEmails.Count > 0)
            {
                EmailModel emailModel = new EmailModel()
                {
                    Title = "",
                    Subject = model.Subject,
                    Message = model.Message,
                    EmailsList = usersEmails
                };
                response = emailService.SendContactEmail(emailModel, model.SenderName, model.SenderEmail, model.ProjectTitle, model.EmailType);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
            }
            return Ok(true);
        }
    }
}