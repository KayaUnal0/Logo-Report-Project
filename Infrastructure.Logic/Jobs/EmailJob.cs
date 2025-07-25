using Core.Interfaces;
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
            Log.Information("Hangfire executing job to send email to {Email}", to);
            _emailSender.Send(to, subject, body);
        }
    }
}
