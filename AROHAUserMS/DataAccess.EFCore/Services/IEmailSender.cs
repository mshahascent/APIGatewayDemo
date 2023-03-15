using DataAccess.EFCore.Models;
using MimeKit;
using System.Threading.Tasks;

namespace AROHAUserMS.DataAccess.EFCore.Services
{
    public interface IEmailSender
    {
        //AuthMessageSenderOptions Options { get; }
        Task<string> SendEmailAsync(EmailMessage emailMessage);
        //Task<HttpResponse> Execute(string apiKey, string subject, string message, string email);

    }
}
