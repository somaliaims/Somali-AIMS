using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AIMS.Services.Helpers
{
    public interface IEmailHelper
    {
        /// <summary>
        /// Sends email to the other users of the organization, plus manager group
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        ActionResponse SendNewRegistrationEmail(List<string> emails);
    }

    public class EmailHelper
    {
        SmtpClient client;
        public EmailHelper(SmtpClient sClient)
        {
            client = sClient;
        }

        public void SendNewRegistrationEmail()
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("whoever@me.com");
            mailMessage.To.Add("receiver@me.com");
            mailMessage.Body = "body";
            mailMessage.Subject = "subject";
            client.Send(mailMessage);
        }

    }
}
