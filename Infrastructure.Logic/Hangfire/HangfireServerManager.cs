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
            var time = report.Time;

            //GÜNLÜK
            if (report.Period == ReportPeriod.Günlük)
            {
                var jobId = $"report:{Slugify(report.Subject)}";
                var cron = CronUtils.DailyCron(time);

                RecurringJob.AddOrUpdate(
                    jobId,
                    () => EmailReportExecutor.Execute(report.Subject, report.Period.ToString()),  
                    Cron.Minutely()
                );
            }
            //HAFTALIK
            else if (report.Period == ReportPeriod.Haftalık)
            {
                var cron = CronUtils.WeeklyCron(report.SelectedDays, time);
                var jobId = $"report:{Slugify(report.Subject)}";

                RecurringJob.AddOrUpdate(
                    jobId,
                    () => EmailReportExecutor.Execute(report.Subject, string.Join(",", report.SelectedDays)),
                    cron
                );
            }
            //AYLIK
            else if (report.Period == ReportPeriod.Aylık)
            {
                int dayOfMonth = report.CreatedAt.Day;
                var cron = CronUtils.MonthlyCron(time, dayOfMonth);
                var jobId = $"report:{Slugify(report.Subject)}";

                RecurringJob.AddOrUpdate(
                    jobId,
                    () => EmailReportExecutor.Execute(report.Subject, report.Period.ToString()),
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
