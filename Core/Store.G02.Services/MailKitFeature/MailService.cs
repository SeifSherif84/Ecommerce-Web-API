using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.MailKitFeature
{
    public class MailService : IMailService
    {
        private readonly IOptions<MailKitSetting> _mailSetting;

        public MailService(IOptions<MailKitSetting> mailSetting)
        {
            _mailSetting = mailSetting;
        }
        public bool SendMail(Email email)
        {
            try
            {
                // Build Message Header
                var Mail = new MimeMessage();
                Mail.From.Add(new MailboxAddress(_mailSetting.Value.DisplayName, _mailSetting.Value.Email)); // Sender
                Mail.To.Add(MailboxAddress.Parse(email.To)); // Receiver
                Mail.Subject = email.Subject; // Subject

                // Build Message Body
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = email.Body;
                Mail.Body = bodyBuilder.ToMessageBody(); // Body


                // Open Connection With Mail Server
                using var SmtpServerClient = new MailKit.Net.Smtp.SmtpClient(); // Server Client
                SmtpServerClient.Connect(_mailSetting.Value.Host, _mailSetting.Value.Port);
                SmtpServerClient.Authenticate(_mailSetting.Value.Email, _mailSetting.Value.Password);

                // Send Mail
                SmtpServerClient.Send(Mail);

                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
    }
}
