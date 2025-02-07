using System;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Threading.Tasks;

namespace AIMS.Services
{
    internal class emailCode // Or EmailService if you prefer
    {
        private string _smtpServer;
        private int _smtpPort;
        private string _smtpUser;
        private string _smtpPassword;

        public emailCode() // Constructor to read config
        {
            try
            {
                _smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
                _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                _smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
                _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading email configuration: " + ex.Message);
                throw; // Re-throw to stop execution if config is invalid
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer))
                {
                    smtpClient.Port = _smtpPort;
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_smtpUser);
                        mailMessage.To.Add(to);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = isHtml;

                        await smtpClient.SendMailAsync(mailMessage);

                        Console.WriteLine("Email sent successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                throw; // Re-throw for handling by the calling method
            }
        }
    }
}