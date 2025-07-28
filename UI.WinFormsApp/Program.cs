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
using Microsoft.Extensions.Configuration;
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
            string connectionString = "Server=KAYAUNAL;Database=LogoProject;User Id=sa;Password=1;Encrypt=True;TrustServerCertificate=True;";
            var reportRepository = new ReportRepository(connectionString);

            ApplicationConfiguration.Initialize();
            Application.Run(new HomeScreen(emailSender, sqlRunner, hangfireManager, fileSaver, emailJob, templateRenderer, reportRepository));
        }

    }
}
