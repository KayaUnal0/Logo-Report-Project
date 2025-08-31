using Common.Shared.Dtos;
using System;
using System.Linq.Expressions;
public interface IHangfireManager
{
    void Start();
    void EnqueueEmail(string email, string subject, string body);
    void ScheduleRecurringEmailJobs(ReportDto report);
    void RemoveRecurringJob(string jobId);
}
