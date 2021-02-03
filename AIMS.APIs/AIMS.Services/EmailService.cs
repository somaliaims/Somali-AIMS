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
        string GetTextForSuggestionType(string projectTitle);

        /// <summary>
        /// Sends contact email to users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse SendContactEmail(EmailModel model, string senderName, string sendEmail, string projectTitle, ContactEmailType emailType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="projectTitle"></param>
        /// <returns></returns>
        ActionResponse SendEmailForPendingMessages(EmailModel model, string senderName, string senderEmail, string projectTitle);
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
                smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                smtpSettings.SenderName = smtpSettings.SenderName;
                emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettings);
            }
            else
            {
                emailHelper = new EmailHelper(null, null, null);
            }
        }

        public ActionResponse SendEmailToUsers(EmailModel model)
        {
            ActionResponse response = new ActionResponse();
            return emailHelper.SendEmailToUsers(model.EmailsList, model.Subject, model.Title, model.Message, model.FooterMessage);
        }

        public ActionResponse SendContactEmail(EmailModel model, string senderName, string senderEmail, string projectTitle, ContactEmailType emailType)
        {
            ActionResponse response = new ActionResponse();
            if (emailType == ContactEmailType.Help)
            {
                model.Subject = HELP_REQUEST + model.Subject;
            }
            else if (emailType == ContactEmailType.Information)
            {
                model.Subject = INFORMATION_REQUEST + model.Subject;
            }
            StringBuilder strBuilder = new StringBuilder("<h4>Information request for project (");
            strBuilder.Append(projectTitle);
            strBuilder.Append(")</h4>");
            strBuilder.Append("<br><b>Sender name:</b>");
            strBuilder.Append(senderName);
            strBuilder.Append("<br><b>Sender email:</b>");
            strBuilder.Append(senderEmail);
            strBuilder.Append("<br><br><b>Message</b><br>");
            strBuilder.Append(model.Message);
            model.Message = strBuilder.ToString();
            return emailHelper.SendEmailToUsers(model.EmailsList, model.Subject, model.Subject, model.Message, model.FooterMessage);
        }

        public ActionResponse SendEmailForPendingMessages(EmailModel model, string senderName, string senderEmail, string projectTitle)
        {
            ActionResponse response = new ActionResponse();
            model.Subject = INFORMATION_REQUEST + model.Subject;
            StringBuilder strBuilder = new StringBuilder("<h4>Message not seen by Management User</h4>");
            strBuilder.Append("<p><i>It is to inform you that your message has not been reviewed by any management user and has not been sent to the project owner.</i></p>");
            strBuilder.Append("Project: Information request for project (");
            strBuilder.Append(projectTitle);
            strBuilder.Append(")");
            strBuilder.Append("<br>Sender name:");
            strBuilder.Append(senderName);
            strBuilder.Append("<br>Sender email:");
            strBuilder.Append(senderEmail);
            strBuilder.Append("<br><br>Your message: ");
            strBuilder.Append(model.Message);
            model.Message = strBuilder.ToString();
            return emailHelper.SendEmailToUsers(model.EmailsList, model.Subject, model.Subject, model.Message, model.FooterMessage);
        }

        public string GetTextForSuggestionType(string projectTitle)
        {
            string suggestedText = "Suggestion through AIMS to update data for the project (<b>" + projectTitle + "</b>)";
            return suggestedText;
        }

    }
}
