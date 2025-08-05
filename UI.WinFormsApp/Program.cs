using Common.Shared;
using Core.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Hangfire;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Logging;
using Infrastructure.Logic.Templates;
using Logo_Project.Logging;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using System;
using System.Windows.Forms;
using UI.WinFormsApp;

namespace Logo_Project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Logging configuration
            // Setup logger settings
            var uiLoggerSettings = new LoggerSettings
            {
                ProjectName = "UI",
                FilePath = "Logs/ui-winforms-log-.txt",
                MinimumLevel = LogEventLevel.Information
            };

            var infraLoggerSettings = new LoggerSettings
            {
                ProjectName = "Infrastructure",
                FilePath = "Logs/infrastructure-log-.txt",
                MinimumLevel = LogEventLevel.Debug
            };

            UIWinFormsLoggerConfig.Instance.Init(uiLoggerSettings);
            InfrastructureLoggerConfig.Instance.Init(infraLoggerSettings);

            string connectionString = "Server=KAYAUNAL;Database=LogoProject;User Id=sa;Password=1;Encrypt=True;TrustServerCertificate=True;";

            // Setup Hangfire to use memory storage
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionString);

            using var server = new BackgroundJobServer();

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Manually initialize dependencies
            var emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>();
            var emailSender = new EmailSender(emailSettings);
            var emailJob = new EmailJob(emailSender);

            EmailJobWrapper.JobInstance = emailJob;

            var sqlRunner = new SqlQueryRunner();
            var hangfireManager = new HangfireServerManager(emailJob);
            var fileSaver = new FileSaver();
            var templateRenderer = new TemplateRenderer();
            var reportRepository = new ReportRepository(connectionString);

            var allReports = reportRepository.GetReports();
            foreach (var report in allReports.Where(r => r.Aktif)) 
            {
                hangfireManager.ScheduleRecurringEmailJobs(report);
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new HomeScreen(emailSender, sqlRunner, hangfireManager, fileSaver, emailJob, templateRenderer, reportRepository));
        }

    }
}
