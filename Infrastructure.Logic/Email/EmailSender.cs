using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Core.Interfaces;
using Serilog;

namespace Infrastructure.Logic.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(EmailSettings settings)
        {
            _settings = settings;
        }

        public bool Send(string toAddress, string subject, string body, string[] attachmentPaths = null)

        {
            try
            {
                var addr = new MailAddress(toAddress);
                if (addr.Address != toAddress)
                    throw new Exception("Geçersiz e-posta adresi");

                if (string.IsNullOrWhiteSpace(subject))
                    subject = "Yeni Form Bildirimi";

                Log.Information("E-posta gönderilmeye hazırlanıyor: Alıcı = {Email}, Konu = {Subject}", toAddress, subject);

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toAddress);

                if (attachmentPaths != null)
                {
                    foreach (var path in attachmentPaths)
                    {
                        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                        {
                            mail.Attachments.Add(new Attachment(path));
                        }
                    }
                }

                using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                Log.Information("E-posta başarıyla gönderildi: {Email}", toAddress);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "E-posta gönderimi sırasında hata oluştu.");
                return false;
            }
        }
    }
}
