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
        public bool Send(string toAddress, string subject, string body, string attachmentPath = null)
        {
            try
            {
                var addr = new MailAddress(toAddress);
                if (addr.Address != toAddress)
                    throw new Exception("Geçersiz e-posta adresi");

                if (string.IsNullOrWhiteSpace(subject))
                    subject = "Yeni Form Bildirimi";

                Log.Information("E-posta gönderilmeye hazırlanıyor: Alıcı = {Email}, Konu = {Subject}", toAddress, subject);

                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string senderEmail = "kayaunal010@gmail.com";
                string senderPassword = "urcs rafh iage wckf"; // move to config!

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toAddress);

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    mail.Attachments.Add(new Attachment(attachmentPath));
                }

                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
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
