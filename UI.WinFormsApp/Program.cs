using Core.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Hangfire;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Logging;
using Infrastructure.Logic.Templates;
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
            LoggerConfig.Configure();

            // Setup Hangfire to use memory storage
            GlobalConfiguration.Configuration.UseMemoryStorage();
            using var server = new BackgroundJobServer();

            // Manually initialize dependencies
            var emailSender = new EmailSender();
            var sqlRunner = new SqlQueryRunner();
            var emailJob = new EmailJob(emailSender);
            var hangfireManager = new HangfireServerManager(emailJob);
            var fileSaver = new FileSaver();
            var templateRenderer = new TemplateRenderer();
            string connectionString = "Server=KAYAUNAL;Database=LogoProject;User Id=sa;Password=1;Encrypt=True;TrustServerCertificate=True;";
            var reportRepository = new ReportRepository(connectionString);

            ApplicationConfiguration.Initialize();
            Application.Run(new HomeScreen(emailSender, sqlRunner, hangfireManager, fileSaver, emailJob, templateRenderer, reportRepository));
        }

    }
}
