using Core.Interfaces;
using Infrastructure.Logic.Logging;
using Serilog;

namespace Infrastructure.Logic.Hangfire
{
    public class JobRunner : IJobRunner
    {
        private readonly IEmailSender EmailSender;

        public JobRunner(IEmailSender emailSender)
        {
            EmailSender = emailSender;
        }

        public void SubmitReportJob(string email, string subject, string body)
        {
            try
            {
                InfrastructureLoggerConfig.Instance.Logger.Information("Arkaplanda rapor gönderimi başladı: {Email}", email);
                EmailSender.Send(email, subject, body);
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(ex, "Arkaplanda rapor gönderimi sırasında hata oluştu.");
            }
        }
    }
}
