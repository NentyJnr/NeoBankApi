using NeoBank.Models;

namespace NeoBank.Contracts
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(Email email);
        //Task<string> SendEmail(EmailBody emailBody);
    }
}
