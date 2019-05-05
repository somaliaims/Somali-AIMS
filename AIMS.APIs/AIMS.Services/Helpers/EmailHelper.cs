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

        ActionResponse SendEmailToUsers(List<EmailAddress> emailsList, string subject, string emailTitle, string emailMessage);

        /// <summary>
        /// Sends email to recover password for the provided email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ActionResponse SendPasswordRecoveryEmail(PasswordResetEmailModel model);
    }

    public class EmailHelper : IEmailHelper
    {
        SmtpClient client;
        string emailFrom;

        public EmailHelper(string adminEmail, SMTPSettingsModel smtpSettings)
        {
            client = this.GetSMTPClient(smtpSettings);
            emailFrom = adminEmail;
        }

        public ActionResponse SendNewRegistrationEmail(List<EmailsModel> emailList, string organizationName)
        {
            ActionResponse response = new ActionResponse();
            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress(this.emailFrom);

            var managersEmailList = (from user in emailList
                                     where user.UserType == UserTypes.Manager
                                     || user.UserType == UserTypes.SuperAdmin
                                     select user.Email);
            string managersEmailString = String.Join(',', managersEmailList);

            var usersEmailList = (from user in emailList
                                  where user.UserType == UserTypes.Standard
                                  select user.Email);
            string usersEmailString = String.Join(',', usersEmailList);

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

        public ActionResponse SendPasswordRecoveryEmail(PasswordResetEmailModel model)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(this.emailFrom);
                string emailMessage = this.GetPasswordResetMessage(model.Email, model.FullName, model.Token, model.Url);
                mailMessage.To.Add(model.Email);
                mailMessage.Body = emailMessage;
                mailMessage.Subject = "Password Reset Request AIMS Somalia";
                client.Send(mailMessage);
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        public ActionResponse SendEmailToUsers(List<EmailAddress> emailsList, string subject, string emailTitle, string emailMessage)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                MailMessage mailMessage = new MailMessage();
                var emailAddresses = (from e in emailsList
                                      select e.Email);

                foreach (var emailAddress in emailAddresses)
                {
                    mailMessage.To.Add(emailAddress);
                }
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(this.emailFrom);
                mailMessage.Body = this.FormatMessage(emailTitle, emailMessage);
                mailMessage.Subject = subject;
                client.Send(mailMessage);
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        private string FormatMessage(string title, string message)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h2>" + title + "</h2>");
            messageList.Add("<p>" + message + "</p>");
            return (String.Join(string.Empty, messageList));
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

        private string GetPasswordResetMessage(string email, string fullName, string token, string url)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h1>Dear AIMS User (" + fullName + ")</h1>");
            messageList.Add("<p>We have received a password reset request for your email. If it was not you, please ignore this email.</p>");
            messageList.Add("<p>Click on the link below and follow the instructions to reset password. This link will expire in two hours</p>");
            messageList.Add("<p><a target='_blank' href='" + url + token + "'>Password Reset Link</a></p>");
            messageList.Add("<b>AIMS Support Team</b>");
            return (String.Join(string.Empty, messageList));
        }

        private SmtpClient GetSMTPClient(SMTPSettingsModel settings)
        {
            return new SmtpClient()
            {
                Host = settings.Host,
                Port = settings.Port,
                Credentials = new NetworkCredential(
                            settings.Username,
                            settings.Password
                        ),
                EnableSsl = true
            };
        }

    }
}
