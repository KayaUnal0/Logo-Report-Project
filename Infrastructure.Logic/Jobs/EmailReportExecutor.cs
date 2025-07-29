using System;
using System.Linq;
using Core.Interfaces;
using Common.Shared.Enums;
using Common.Shared.Dtos;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Templates;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Infrastructure.Logic.Jobs
{
    public static class EmailReportExecutor
    {
        public static void Execute(string reportSubject, string dayString)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                var emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>();
                var sender = new EmailSender(emailSettings);
                var sqlRunner = new SqlQueryRunner();
                var templateRenderer = new TemplateRenderer();
                var reportRepo = new ReportRepository("Server=KAYAUNAL;Database=LogoProject;User Id=sa;Password=1;Encrypt=True;TrustServerCertificate=True;");

                var reports = reportRepo.GetReports();
                var report = reports.FirstOrDefault(r => r.Subject == reportSubject);
                if (report == null)
                {
                    Log.Warning("No report found with subject {Subject}", reportSubject);
                    return;
                }

                var result = sqlRunner.ExecuteQuery(report.Query);

                var templatePath = "Templates/EmailTemplate.sbn";
                var body = templateRenderer.RenderTemplateAsync(templatePath, new
                {
                    subject = report.Subject,
                    status = result.Status.ToString(),
                    results = string.Join("\n", result.Results),
                    filePath = report.Directory
                }).Result;

                sender.Send(report.Email, report.Subject, body);
                Log.Information("Recurring report sent for {Subject} on {Day}", reportSubject, dayString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing recurring report for {Subject} on {Day}", reportSubject, dayString);
            }
        }
    }
}
