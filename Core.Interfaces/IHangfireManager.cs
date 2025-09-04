using Common.Shared.Dtos;

namespace Core.Interfaces
{
    public interface IHangfireManager
    {
        void Start();
        void EnqueueEmail(string email, string subject, string body);
        void ScheduleRecurringEmailJobs(ReportDto report);
        void RemoveRecurringJob(string jobId);
    }
}
