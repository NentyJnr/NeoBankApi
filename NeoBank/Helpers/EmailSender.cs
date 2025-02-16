using Microsoft.Extensions.Options;
using MimeKit;
using NeoBank.Contracts;
using NeoBank.Data;
using NeoBank.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;

namespace NeoBank.Helpers
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailsettings { get; }
        private SmtpSettings _smtpSettings { get; }
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(IOptions<EmailSettings> emailSettings, IOptions<SmtpSettings> smtpSettings, ILogger<EmailSender> logger)
        {
            _emailsettings = emailSettings.Value;
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }
        public async Task<bool> SendEmail(Email email)
        {
            SendGridClient client = new(_emailsettings.ApiKey);
            EmailAddress to = new(email.To);
            EmailAddress from = new()
            {
                Email = _emailsettings.FromAddress,
                Name = _emailsettings.FromName
            };

            SendGridMessage message = MailHelper.CreateSingleEmail(from, to, email.Subject,
                                                                   email.Body, "EmailContent");
            SendGrid.Response response = await client.SendEmailAsync(message);

            return response.StatusCode == System.Net.HttpStatusCode.OK ||
                   response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }

        //public async Task<string> SendEmail(EmailBody emailBody)
        //{
        //    MimeMessage email = new MimeMessage();

        //    email.Sender = MailboxAddress.Parse(_smtpSettings.Email);
        //    email.To.Add(MailboxAddress.Parse(emailBody.Receiver));
        //    email.Subject = emailBody.Subject;

        //    BodyBuilder builder = new BodyBuilder
        //    { HtmlBody = emailBody.MessageBody };

        //    if (emailBody.attachmentContent != null)
        //    {
        //        builder.Attachments.Add("Receipt.pdf", emailBody.attachmentContent, ContentType.Parse("application/pdf"));
        //    }

        //    email.Body = builder.ToMessageBody();

        //     SmtpClient smtpClient = new SmtpClient();

        //    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

        //    smtpClient.Connect(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

        //    smtpClient.Authenticate(_smtpSettings.UserName, _smtpSettings.Password);

        //    var result = await smtpClient.SendAsync(email);
        //    smtpClient.Disconnect(true);

        //    return result;
        //}

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}

