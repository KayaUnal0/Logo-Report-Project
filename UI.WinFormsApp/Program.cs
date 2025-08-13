using Common.Shared;
using Common.Shared.Dtos;
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
using Microsoft.AspNetCore.Builder;
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

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var dbQuerySettings = config.GetSection("QueryDatabaseSettings").Get<DatabaseSettings>();
            string connectionQueryString = dbQuerySettings.ToConnectionString();

            var dbAppSettings = config.GetSection("AppDatabaseSettings").Get<DatabaseSettings>();
            string connectionAppString = dbAppSettings.ToConnectionString();

            // Setup Hangfire to use memory storage
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionAppString);

            using var server = new BackgroundJobServer();


            // Manually initialize dependencies
            var emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>();
            var emailSender = new EmailSender(emailSettings);
            var emailJob = new EmailJob(emailSender);

            EmailJobWrapper.JobInstance = emailJob;

            var sqlRunner = new SqlQueryRunner(config);
            var hangfireManager = new HangfireServerManager(emailJob, connectionAppString);
            var fileSaver = new FileSaver();
            var templateRenderer = new TemplateRenderer();
            var reportRepository = new ReportRepository(connectionAppString);

            hangfireManager.Start();

            ApplicationConfiguration.Initialize();
            Application.Run(new ReportListUI(emailSender, sqlRunner, hangfireManager, fileSaver, emailJob, templateRenderer, reportRepository));
        }
    }
}
