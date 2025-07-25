using Core.Interfaces;
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
                Log.Information("Arkaplanda rapor gönderimi başladı: {Email}", email);
                _emailSender.Send(email, subject, body);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Arkaplanda rapor gönderimi sırasında hata oluştu.");
            }
        }
    }
}
