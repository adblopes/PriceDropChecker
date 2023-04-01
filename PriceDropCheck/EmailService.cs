using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace PriceDropCheck
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:From").Value));
            email.To.Add(MailboxAddress.Parse(_config.GetSection("Email:To").Value));
            email.Subject = "Test Subject";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("Email:Host").Value, 465, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_config.GetSection("Email:From").Value, _config.GetSection("Email:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
