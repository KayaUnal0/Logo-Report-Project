using Common.Shared;
using Common.Shared.Dtos;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Logic.Config;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Hangfire;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Logging;
using Infrastructure.Logic.Templates;
using Logo_Project.Logging;
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
            // 1) Logging
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
            InfrastructureLoggerConfig.Instance.Logger?.Information("Bootstrapping...");
            InfrastructureLoggerConfig.Instance.Init(infraLoggerSettings);

            // Deccrypt Secrets
            var (_, emailSettings) = SettingsManager.LoadEmail();   
            var (_, queryDb) = SettingsManager.LoadQueryDb();    
            var (_, appDb) = SettingsManager.LoadAppDb();     

            // Guard (helps diagnose master key issues)
            if (string.IsNullOrWhiteSpace(appDb?.Password))
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(
                    "App DB password missing after LoadAppDb(). Check LOGO_APP_MASTER_KEY and AppDatabaseSettings.PasswordEnc.");
                MessageBox.Show("App DB password is missing (decryption failed). See logs.", "Startup Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Build connection strings
            var connectionAppString = appDb.ToConnectionString();
            var connectionQueryString = queryDb.ToConnectionString();

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionAppString);

            //Start Hangfire server
            using var server = new BackgroundJobServer();


            var emailSender = new EmailSender(emailSettings);
            var emailJob = new EmailJob(emailSender);
            EmailJobWrapper.JobInstance = emailJob;

            var sqlRunner = new SqlQueryRunner(queryDb);
            var fileSaver = new FileSaver();
            var templateRenderer = new TemplateRenderer();
            var reportRepository = new ReportRepository(connectionAppString);

            var hangfireManager = new HangfireServerManager(emailJob, connectionAppString);
            hangfireManager.Start();

            //Run UI
            ApplicationConfiguration.Initialize();
            Application.Run(new ReportListUI(
                emailSender, sqlRunner, hangfireManager, fileSaver, emailJob, templateRenderer, reportRepository));
        }
    }
}
