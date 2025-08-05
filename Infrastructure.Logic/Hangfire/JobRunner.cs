using Core.Interfaces;
using Infrastructure.Logic.Logging;
using Serilog;

namespace Infrastructure.Logic.Hangfire
{
    public class JobRunner : IJobRunner
    {
        private readonly IEmailSender _emailSender;

        public JobRunner(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void SubmitReportJob(string email, string subject, string body)
        {
            try
            {
                InfrastructureLoggerConfig.Instance.Logger.Information("Arkaplanda rapor gönderimi başladı: {Email}", email);
                _emailSender.Send(email, subject, body);
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(ex, "Arkaplanda rapor gönderimi sırasında hata oluştu.");
            }
        }
    }
}
