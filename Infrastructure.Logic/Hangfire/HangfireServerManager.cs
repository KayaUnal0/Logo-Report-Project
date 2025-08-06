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

            Task.Run(() => StartWebServer());

            _server = new BackgroundJobServer();
            InfrastructureLoggerConfig.Instance.Logger.Information("Hangfire sunucusu başlatıldı.");
        }

        public void Stop()
        {
            _server?.Dispose();
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
            // Parse time (fallback to 09:00 if not available)
            var time = report.CreatedAt.TimeOfDay;

            var period = report.Period?.Trim().ToLowerInvariant();

            if (period == "günlük")
            {
                var jobId = $"report:{Slugify(report.Subject)}";
                var cron = CronUtils.DailyCron(time);

                RecurringJob.AddOrUpdate(
                    jobId,
                    () => EmailReportExecutor.Execute(report.Subject, "daily"),
                    cron
                );
            }
            else if (period == "haftalık")
            {
                foreach (var day in report.SelectedDays)
                {
                    var cron = day.WeeklyCron(time);
                    var jobId = $"report:{Slugify(report.Subject)}";

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => EmailReportExecutor.Execute(report.Subject, day.ToString()),
                        cron
                    );
                }
            }
            else if (period == "aylık")
            {
                int dayOfMonth = report.CreatedAt.Day; 
                var cron = CronUtils.MonthlyCron(time, dayOfMonth);
                var jobId = $"report:{Slugify(report.Subject)}";

                RecurringJob.AddOrUpdate(
                    jobId,
                    () => EmailReportExecutor.Execute(report.Subject, "monthly"),
                    cron
                );
            }
        }

        public void RemoveRecurringJob(string subject)
        {
            RecurringJob.RemoveIfExists($"report:{Slugify(subject)}");
            InfrastructureLoggerConfig.Instance.Logger.Information("Zamanlanmış görev silindi: {JobId}.", subject);
        }

        private static void StartWebServer()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            app.UseHangfireDashboard("/hangfire");

            app.Run();
        }
    }
}
