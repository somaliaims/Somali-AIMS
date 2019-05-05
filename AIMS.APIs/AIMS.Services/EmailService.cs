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
    }

    public class EmailService : IEmailService
    {
        IEmailHelper emailHelper;
        AIMSDbContext context;

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
            response = emailHelper.SendEmailToUsers(model.EmailsList, model.Subject, model.Title, model.Message);
            return response;
        }
    }
}
