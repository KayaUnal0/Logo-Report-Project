using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Jobs;
using Serilog;
using System;
using System.Linq.Expressions;
using Hangfire.Common;
using Hangfire.States;


namespace Infrastructure.Logic.Hangfire
{
    public class HangfireServerManager : IHangfireManager
    {
        private BackgroundJobServer? _server;
        private readonly EmailJob _emailJob; 

        public HangfireServerManager(EmailJob emailJob)
        {
            _emailJob = emailJob;
        }

        public void Start()
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage();

            _server = new BackgroundJobServer();
            Log.Information("Hangfire server started with in-memory storage.");
        }

        public void Stop()
        {
            _server?.Dispose();
            Log.Information("Hangfire server stopped.");
        }

        public void Enqueue(Expression<Action> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
            Log.Information("Job enqueued to Hangfire.");
        }

        public void EnqueueEmail(string email, string subject, string body)
        {
            // Call a static method that manually resolves the dependency
            BackgroundJob.Enqueue(() => EmailJobWrapper.SendEmail(email, subject, body));
        }

        private string GenerateSafeJobId(string subject, WeekDay day)
        {
            var clean = new string(subject
                .Where(c => char.IsLetterOrDigit(c) || c == '_')
                .ToArray());

            return $"report:{clean}:{day}";
        }

        public void ScheduleRecurringEmailJobs(ReportDto report)
        {
            // Remove all potential previous jobs
            foreach (var day in Enum.GetValues(typeof(WeekDay)))
            {
                var oldJobId = GenerateSafeJobId(report.Subject, (WeekDay)day);
                RecurringJob.RemoveIfExists(oldJobId);
            }

            // Add selected days
            foreach (var day in report.SelectedDays)
            {
                var cron = CronUtils.ToCron(day);
                var jobId = GenerateSafeJobId(report.Subject, day);

                var job = Job.FromExpression(() =>
                    EmailReportExecutor.Execute(report.Subject, day.ToString())
                );

                RecurringJob.AddOrUpdate(
                    jobId,
                    () => EmailReportExecutor.Execute(report.Subject, day.ToString()),
                    Cron.Minutely // TEST PURPOSES
                );
            }
        }


        public void RemoveRecurringJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
            Log.Information("Recurring job {JobId} removed.", jobId);
        }


    }
}
