using Infrastructure.Logic.Email;
using Infrastructure.Logic.Jobs;
using Microsoft.Extensions.Configuration;
using System.Net;

public static class EmailJobWrapper
{
    public static EmailJob JobInstance { get; set; }

    public static void SendEmail(string email, string subject, string body)
    {
        // Read settings again here
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var settings = config.GetSection("EmailSettings").Get<EmailSettings>();

        if (settings == null)
        {
            throw new Exception("EmailSettings could not be loaded.");
        }

        var sender = new EmailSender(settings);

        sender.Send(email, subject, body);
    }
}
