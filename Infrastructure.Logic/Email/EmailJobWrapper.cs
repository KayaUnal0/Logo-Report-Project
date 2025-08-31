using Infrastructure.Logic.Config;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Jobs;
using Microsoft.Extensions.Configuration;
using System.Net;

public static class EmailJobWrapper
{
    public static EmailJob JobInstance { get; set; }

    public static void SendEmail(string email, string subject, string body)
    {
        var (_, settings) = SettingsManager.LoadEmail();  // ✅ decrypted
        var sender = new Infrastructure.Logic.Email.EmailSender(settings);
        sender.Send(email, subject, body);
    }

    public static void SendEmail(string email, string subject, string body, string[] attachments)
    {
        var (_, settings) = SettingsManager.LoadEmail();  // ✅ decrypted
        var sender = new Infrastructure.Logic.Email.EmailSender(settings);
        sender.Send(email, subject, body, attachments);
    }
}
