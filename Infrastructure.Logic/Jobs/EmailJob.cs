using Core.Interfaces;
using Infrastructure.Logic.Logging;
using Serilog;

namespace Infrastructure.Logic.Jobs
{
    public class EmailJob
    {
        private readonly IEmailSender EmailSender;

        public EmailJob(IEmailSender emailSender) 
        {
            EmailSender = emailSender;
        }

        public void Send(string to, string subject, string body)
        {
            InfrastructureLoggerConfig.Instance.Logger.Information("Hangfire, {Email} adresine e-posta göndermek için görevi yürütüyor.", to);
            EmailSender.Send(to, subject, body);
        }
    }
}
