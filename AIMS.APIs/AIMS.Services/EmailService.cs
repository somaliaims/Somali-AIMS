using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends email to provided list of emails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse SendEmailToUsers(EmailModel model);

        /// <summary>
        /// Gets string description for provided suggestion type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetTextForSuggestionType(ProjectSuggestionType type, string projectTitle);

        /// <summary>
        /// Sends contact email to users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse SendContactEmail(EmailModel model, string senderName, string sendEmail, string projectTitle, ContactEmailType emailType);
    }

    public class EmailService : IEmailService
    {
        IEmailHelper emailHelper;
        AIMSDbContext context;
        private readonly string INFORMATION_REQUEST = "SomaliAIMS information request:";
        private readonly string HELP_REQUEST = "SomaliAIMS help request:";

        public EmailService(AIMSDbContext cntxt)
        {
            context = cntxt;
            ISMTPSettingsService smtpService = new SMTPSettingsService(context);
            var smtpSettings = smtpService.GetPrivate();
            SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();

            if (smtpSettings != null)
            {
                smtpSettingsModel.Host = smtpSettings.Host;
                smtpSettingsModel.Port = smtpSettings.Port;
                smtpSettingsModel.Username = smtpSettings.Username;
                smtpSettingsModel.Password = smtpSettings.Password;
                smtpSettings.AdminEmail = smtpSettings.AdminEmail;
                emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings);
            }
            else
            {
                emailHelper = new EmailHelper(null, null);
            }
        }

        public ActionResponse SendEmailToUsers(EmailModel model)
        {
            ActionResponse response = new ActionResponse();
            return emailHelper.SendEmailToUsers(model.EmailsList, model.Subject, model.Title, model.Message);
        }

        public ActionResponse SendContactEmail(EmailModel model, string senderName, string sendEmail, string projectTitle, ContactEmailType emailType)
        {
            ActionResponse response = new ActionResponse();
            if (emailType == ContactEmailType.Help)
            {
                model.Subject = HELP_REQUEST + model.Subject;
            }
            else if (emailType == ContactEmailType.Information)
            {
                model.Subject = INFORMATION_REQUEST + model.Subject;
                model.Message = "<h4>Information request for project (" + projectTitle + ")</h4>" + model.Message;
            }
            return emailHelper.SendEmailToUsers(model.EmailsList, model.Subject, model.Subject, model.Message);
        }

        public string GetTextForSuggestionType(ProjectSuggestionType type, string projectTitle)
        {
            string suggestedText = "";
            switch(type)
            {
                case ProjectSuggestionType.AddData:
                    suggestedText = "Suggestion through AIMS to add some data to the project (<b>" + projectTitle  + "</b>)";
                    break;

                case ProjectSuggestionType.EditData:
                    suggestedText = "Suggestion through AIMS to edit some data for the project (<b>" + projectTitle + "</b>)";
                    break;

                case ProjectSuggestionType.AmendData:
                    suggestedText = "Suggestion through AIMS for correction of some data for the project (<b>" + projectTitle + "</b>)";
                    break;

                default:
                    suggestedText = "Suggestion through AIMS to update data for the project (<b>" + projectTitle + "</b>)";
                    break;
            }
            return suggestedText;
        }

    }
}
