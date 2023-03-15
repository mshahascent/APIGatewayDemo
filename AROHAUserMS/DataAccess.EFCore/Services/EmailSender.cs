
using DataAccess.EFCore.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace AROHAUserMS.DataAccess.EFCore.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

      
        public async Task<string> SendEmailAsync(EmailMessage message)
        {
            string response = "";

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress(_emailConfig.From, _emailConfig.From));
                    message.To = new List<MailboxAddress> {new MailboxAddress("default", "miki.shah@ascentinfo.solutions" )};
                    emailMessage.To.AddRange(message.To);
                   
                    emailMessage.Subject = message.Subject;
                    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

                    response = await client.SendAsync(emailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
            return response;
        }

        //public async Task<HttpResponse> Execute(string apiKey, string subject, string message, string email)
        //{

          
        //    var msg = new SendGridMessage()
        //    {
        //        From = new EmailAddress("abc@gmail.com", Options.SendGridUser),
        //        Subject = subject,
        //        PlainTextContent = message,
        //        HtmlContent = message
        //    };
        //    msg.AddTo(new EmailAddress(email));

        //    // Disable click tracking.
        //    // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        //    msg.SetClickTracking(false, false);

        //    //return client.SendEmailAsync(msg);
        //    var response = await client.SendEmailAsync(msg);

        //    return new HttpResponse();
        //}
    }
}
