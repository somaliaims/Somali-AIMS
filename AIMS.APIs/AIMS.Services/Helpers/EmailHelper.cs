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
        ActionResponse SendNewRegistrationEmail(List<EmailsModel> emailList, string organizationName, string subject, string message, string footerMessage = null);

        /// <summary>
        /// Send emails to users
        /// </summary>
        /// <param name="emailsList"></param>
        /// <param name="subject"></param>
        /// <param name="emailTitle"></param>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        ActionResponse SendEmailToUsers(List<EmailAddress> emailsList, string subject, string emailTitle, string emailMessage, string footerMessage = null);

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
        string emailFrom, senderName;
        const string EMAIL_SIGNATURE = "AIMS Support Team";
        const string FOOTER_LINE = "If you need further assistance, please submit a help request via the contact form in the AIMS";

        public EmailHelper(string adminEmail, string adminName, SMTPSettingsModel smtpSettings)
        {
            client = this.GetSMTPClient(smtpSettings);
            senderName = adminName;
            emailFrom = adminEmail;
        }

        public ActionResponse SendNewRegistrationEmail(List<EmailsModel> emailList, string organizationName, string subject, string message, string footerMessage = null)
        {
            ActionResponse response = new ActionResponse();
            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress(this.emailFrom, senderName);

            var managersEmailList = (from user in emailList
                                     where user.UserType == UserTypes.Manager
                                     || user.UserType == UserTypes.SuperAdmin
                                     select user.Email);
            string managersEmailString = String.Join(',', managersEmailList);

            var usersEmailList = (from user in emailList
                                  where user.UserType == UserTypes.Standard
                                  select user.Email);
            usersEmailList = (from email in usersEmailList
                              where !managersEmailList.Contains(email)
                              select email);

            string usersEmailString = String.Join(',', usersEmailList);
            //Sending bulk email to Managers
            if (managersEmailList.Count() > 0)
            {
                string emailMessage = this.GetUserRegistrationMessageForAdmin(organizationName, message, footerMessage);
                mailMessage.To.Add(managersEmailString);
                mailMessage.Body = emailMessage;
                mailMessage.Subject = subject;
                client.Send(mailMessage);
            }

            if (usersEmailList.Count() > 0)
            {
                string emailMessage = this.GetUserRegistrationMessageForUser(organizationName, message, footerMessage);
                mailMessage.To.Add(usersEmailString);
                mailMessage.Body = emailMessage;
                mailMessage.Subject = subject;
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
                mailMessage.From = new MailAddress(this.emailFrom, senderName);
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

        public ActionResponse SendEmailToUsers(List<EmailAddress> emailsList, string subject, string emailTitle, string emailMessage, string footerMessage = null)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                var emailAddresses = (from e in emailsList
                                      select e.Email);
                var emailAddressString = string.Join(',', emailAddresses);
                MailMessage mailMessage = new MailMessage();
                foreach (var emailAddress in emailAddresses)
                {
                    mailMessage.To.Add(new MailAddress(emailAddress));
                }
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(this.emailFrom, senderName);
                mailMessage.Body = this.FormatMessage(emailTitle, emailMessage, footerMessage);
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

        private string FormatMessage(string title, string message, string footerMessage = null)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h3>" + title + "</h3>");
            messageList.Add("<h3>Dear user,</h3>");
            footerMessage = (footerMessage != null) ? footerMessage : FOOTER_LINE; 
            messageList.Add(message);
            messageList.Add("<h5>" + footerMessage + "</h5>");
            messageList.Add("<h5><b>" + EMAIL_SIGNATURE + "</b></h5>");
            return (String.Join(string.Empty, messageList));
        }

        private string GetUserRegistrationMessageForAdmin(string organizationName, string message = null, string footerMessage = null)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h3>Dear user,</h3>");
            if (message != null)
            {
                messageList.Add("<p>" + message + "</p>");
            }
            else
            {
                messageList.Add("<p>A new user has just submitted the request for registration.</p>");
                messageList.Add("<p>Please open your notification area using AIMS, and approve/disapprove the request.</p>");
            }
            footerMessage = (footerMessage != null) ? footerMessage : FOOTER_LINE;
            messageList.Add("<h5>" + footerMessage + "</h5>");
            messageList.Add("<h5><b>" + EMAIL_SIGNATURE + "</b></h5>");
            return (String.Join(string.Empty, messageList));
        }

        private string GetUserRegistrationMessageForUser(string organizationName, string message = null, string footerMessage = null)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h3>Dear user</h3>");

            if (message != null)
            {
                messageList.Add("<p>" + message + "</p>");
            }
            else
            {
                messageList.Add("<p>A new user has just submitted a request for registration into your organization.</p>");
                messageList.Add("<p>Please open your notification area using AIMS, and approve/disapprove the request.</p>");
            }
            footerMessage = (footerMessage != null) ? footerMessage : FOOTER_LINE;
            messageList.Add("<h5>" + footerMessage + "</h5>");
            messageList.Add("<h5><b>" + EMAIL_SIGNATURE + "</b></h5>");
            return (String.Join(string.Empty, messageList));
        }

        private string GetPasswordResetMessage(string email, string fullName, string token, string url)
        {
            List<string> messageList = new List<string>();
            messageList.Add("<h3>Dear user</h3>");
            messageList.Add("<p>We have received a password reset request for your email. If it was not you, please ignore this email.</p>");
            messageList.Add("<p>Click on the link below and follow the instructions to reset password. This link will expire in two hours</p>");
            messageList.Add("<p><a target='_blank' href='" + url + token + "'>Password Reset Link</a></p>");
            messageList.Add("<h5>" + FOOTER_LINE + "</h5>");
            messageList.Add("<h5><b>" + EMAIL_SIGNATURE + "</b></h5>");
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
