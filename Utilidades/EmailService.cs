using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Utilidades
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, List<(Stream FileStream, string FileName, string MimeType)> attachments = null)
        {
            using var client = new SmtpClient(this._config["SmtpSettings:Host"], Convert.ToInt32(this._config["SmtpSettings:Port"]))
            {
                Credentials = new NetworkCredential(this._config["SmtpSettings:UserName"], this._config["SmtpSettings:Password"]),
                EnableSsl = Convert.ToBoolean(this._config["SmtpSettings:EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(this._config["SmtpSettings:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,



            };
            if (attachments != null)
            {
                foreach (var attachmentInfo in attachments)
                {
                    var attachment = new Attachment(attachmentInfo.FileStream, attachmentInfo.FileName, attachmentInfo.MimeType);
                    mailMessage.Attachments.Add(attachment);
                }
            }
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}

