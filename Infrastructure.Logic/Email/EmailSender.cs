using Core.Interfaces;
using Infrastructure.Logic.Logging;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Logic.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings Settings;

        public EmailSender(EmailSettings settings)
        {
            Settings = settings;
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

                InfrastructureLoggerConfig.Instance.Logger.Information("E-posta gönderilmeye hazırlanıyor: Alıcı = {Email}, Konu = {Subject}", toAddress, subject);

                var mail = new MailMessage
                {
                    From = new MailAddress(Settings.SenderEmail),
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

                using var smtpClient = new SmtpClient(Settings.SmtpServer, Settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(Settings.SenderEmail, Settings.SenderPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                InfrastructureLoggerConfig.Instance.Logger.Information("E-posta başarıyla gönderildi: {Email}", toAddress);
                return true;
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(ex, "E-posta gönderimi sırasında hata oluştu.");
                return false;
            }
        }
    }
}
