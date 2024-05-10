using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ArmsFW.Services.Email
{
    public interface IEmailSender
    {
        string Destionation { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        SendCompletedEventHandler OnCompleted { get; set; }
        Action<EmailResponse> ColetarResponse { get; set; }
        string LogFile { get; set; }

        Task<EmailResponse> SendEmailAsync(string email, string subject, string message);
        Task<EmailResponse> SendEmailAsync(string email);
        Task<EmailResponse> SendEmailAsync(IEmailMessage message);
        Task<EmailResponse> SendEmailAsync();
        Task<EmailResponse> SendEmailAsync(EmailRequest mailRequest);
        Task LimparLog();
    }
}
