using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="emailList"></param>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        ActionResponse SendNewRegistrationEmail(List<EmailsModel> emailList, string organizationName);
    }

    public class EmailHelper : IEmailHelper
    {
        SmtpClient client;
        string emailFrom;

        public EmailHelper(SmtpClient sClient, string adminEmail)
        {
            client = sClient;
            emailFrom = adminEmail;
        }

        public ActionResponse SendNewRegistrationEmail(List<EmailsModel> emailList, string organizationName)
        {
            ActionResponse response = new ActionResponse();
            MailMessage mailMessage = new MailMessage();
            var managersEmailList = (from user in emailList
                                     where user.UserType == UserTypes.Manager
                                     || user.UserType == UserTypes.SuperAdmin
                                     select user.Email);
            string managersEmailString = String.Join(',', managersEmailList);

            var usersEmailList = (from user in emailList
                                  where user.UserType == UserTypes.Standard
                                  select user.Email);
            string usersEmailString = String.Join(',', usersEmailList);

            mailMessage.From = new MailAddress(this.emailFrom);

            //Sending bulk email to Managers
            if (managersEmailList.Count() > 0)
            {
                string emailMessage = this.GetUserRegistrationMessageForAdmin(organizationName);
                mailMessage.To.Add(managersEmailString);
                mailMessage.Body = emailMessage;
                mailMessage.Subject = "New User Registration";
                client.Send(mailMessage);
            }

            if (usersEmailList.Count() > 0)
            {
                string emailMessage = this.GetUserRegistrationMessageForUser(organizationName);
                mailMessage.To.Add(usersEmailString);
                mailMessage.Body = emailMessage;
                mailMessage.Subject = "New User Registration for " + organizationName;
                client.Send(mailMessage);
            }


            return response;
        }



        private string GetUserRegistrationMessageForAdmin(string organizationName)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h1>New User Registration for " + organizationName + "</h1>");
            messageList.Add("<p>A new user has just submitted the request for registration.</p>");
            messageList.Add("<p>Please open your notification area using AIMS, and approve/disapprove the request.</p>");
            messageList.Add("<b>AIMS Support Team</b>");
            return (String.Join(string.Empty, messageList));
        }

        private string GetUserRegistrationMessageForUser(string organizationName)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h1>Dear AIMS User</h1>");
            messageList.Add("<p>A new user has just submitted a request for registration into your organization.</p>");
            messageList.Add("<p>Please open your notification area using AIMS, and approve/disapprove the request.</p>");
            messageList.Add("<b>AIMS Support Team</b>");
            return (String.Join(string.Empty, messageList));
        }

    }
}
