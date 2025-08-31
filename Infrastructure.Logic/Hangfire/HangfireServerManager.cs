using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.States;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Logging;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System;
using System.Linq.Expressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace Infrastructure.Logic.Hangfire
{
    public class HangfireServerManager : IHangfireManager
    {
        private BackgroundJobServer? Server;
        private readonly EmailJob EmailJob;
        private readonly string ConnectionString;

        public HangfireServerManager(EmailJob emailJob, string connectionString)
        {
            EmailJob = emailJob;
            ConnectionString = connectionString;
        }

        public void Start()
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConnectionString);

            Task.Run(() => StartWebServer());
            Server = new BackgroundJobServer();

        }

        public void Stop()
        {
            ClearAllHangfireJobs();
            Server?.Dispose();
            InfrastructureLoggerConfig.Instance.Logger.Information("Hangfire sunucusu durduruldu.");
        }

        public void Enqueue(Expression<Action> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
            InfrastructureLoggerConfig.Instance.Logger.Information("Yeni görev Hangfire kuyruğuna eklendi.");
        }

        public void EnqueueEmail(string email, string subject, string body)
        {
            // Call a static method that manually resolves the dependency
            BackgroundJob.Enqueue(() => EmailJobWrapper.SendEmail(email, subject, body));
        }

        private string Slugify(string subject)
        {
            return new string(subject
                .Where(c => char.IsLetterOrDigit(c) || c == '_')
                .ToArray());
        }
        public void ScheduleRecurringEmailJobs(ReportDto report)
        {
            var time = report.Time;
            string cron = "";
            string info = "";

            if (report.Period == ReportPeriod.Günlük)
            {
                cron = CronUtils.DailyCron(time);
                info = report.Period.ToString();
            }
            else if (report.Period == ReportPeriod.Haftalık)
            {
                cron = CronUtils.WeeklyCron(report.SelectedDays, time);
                info = string.Join(",", report.SelectedDays);
            }
            else if (report.Period == ReportPeriod.Aylık)
            {
                int dayOfMonth = report.Date.Day;
                cron = CronUtils.MonthlyCron(time, dayOfMonth);
                info = report.Period.ToString();
            }

            AddOrUpdate(report.Subject, cron, info);
        }

        private void AddOrUpdate(string subject, string cron, string info)
        {
            RecurringJob.AddOrUpdate(
                $"report:{Slugify(subject)}",
                () => EmailReportExecutor.Execute(subject, info),
                cron,
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul")
                }
            );
        }

        public static void ClearAllHangfireJobs()
        {
            var jobStorage = JobStorage.Current;
            var monitoringApi = jobStorage.GetMonitoringApi();


            // Scheduled Jobs
            var scheduledJobs = monitoringApi.ScheduledJobs(0, int.MaxValue);
            foreach (var job in scheduledJobs)
            {
                BackgroundJob.Delete(job.Key);
            }

            // Enqueued Jobs
            var queues = monitoringApi.Queues();
            foreach (var queue in queues)
            {
                var enqueuedJobs = monitoringApi.EnqueuedJobs(queue.Name, 0, int.MaxValue);
                foreach (var job in enqueuedJobs)
                {
                    BackgroundJob.Delete(job.Key);
                }
            }

            // Processing Jobs (isteğe bağlı, genelde bu anlık işlerdir)
            var processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);
            foreach (var job in processingJobs)
            {
                BackgroundJob.Delete(job.Key);
            }

            // Failed Jobs
            var failedJobs = monitoringApi.FailedJobs(0, int.MaxValue);
            foreach (var job in failedJobs)
            {
                BackgroundJob.Delete(job.Key);
            }

            // Succeeded Jobs (İsteğe bağlı: genelde log amaçlı tutulur)
            var succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue);
            foreach (var job in succeededJobs)
            {
                BackgroundJob.Delete(job.Key);
            }

            // Deleted Jobs da varsa temizleyebilirsin
            var deletedJobs = monitoringApi.DeletedJobs(0, int.MaxValue);
            foreach (var job in deletedJobs)
            {
                BackgroundJob.Delete(job.Key);
            }
        }

        public void RemoveRecurringJob(string subject)
        {
            RecurringJob.RemoveIfExists($"report:{Slugify(subject)}");
            InfrastructureLoggerConfig.Instance.Logger.Information("Zamanlanmış görev silindi: {JobId}.", subject);
        }

        private void StartWebServer()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddHangfire(cfg =>
            {
                cfg.UseSqlServerStorage(ConnectionString);
            });

            builder.Services.AddHangfireServer();

            var app = builder.Build();
            app.UseHangfireDashboard("/hangfire");
            app.Run();
        }
    }
}
