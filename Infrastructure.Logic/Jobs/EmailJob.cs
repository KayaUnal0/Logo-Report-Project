using Core.Interfaces;
using Infrastructure.Logic.Logging;
using Serilog;

namespace Infrastructure.Logic.Jobs
{
    public class EmailJob
    {
        private readonly IEmailSender _emailSender;

        public EmailJob(IEmailSender emailSender) 
        {
            _emailSender = emailSender;
        }

        public void Send(string to, string subject, string body)
        {
            InfrastructureLoggerConfig.Instance.Logger.Information("Hangfire, {Email} adresine e-posta göndermek için görevi yürütüyor.", to);
            _emailSender.Send(to, subject, body);
        }
    }
}
