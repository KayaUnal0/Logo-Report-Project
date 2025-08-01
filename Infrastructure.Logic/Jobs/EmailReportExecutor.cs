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

                string htmlBody = null;
                var attachments = new List<string>();
                string safeFileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
                var fileSaver = new FileSaver();

                // Save CSV if Excel or Excel+HTML
                if (report.FileType.Contains("Excel"))
                {
                    var csvPath = fileSaver.SaveCsvToFile(report.Directory, safeFileName + ".csv", result.Results);
                    if (!string.IsNullOrEmpty(csvPath))
                        attachments.Add(csvPath);
                }

                // Save HTML if HTML or Excel+HTML
                if (report.FileType.Contains("HTML"))
                {
                    htmlBody = templateRenderer.RenderTemplateAsync("Templates/EmailTemplate.sbn", new
                    {
                        subject = report.Subject,
                        status = result.Status.ToString(),
                        results = string.Join("\n", result.Results),
                        filePath = report.Directory
                    }).Result;

                    // Save HTML version to file too
                    var htmlPath = Path.Combine(report.Directory, safeFileName + ".html");
                    File.WriteAllText(htmlPath, htmlBody);
                    attachments.Add(htmlPath);
                }

                // Send email with all attachments
                sender.Send(
                    report.Email,
                    report.Subject,
                    htmlBody ?? "Rapor ektedir.",
                    attachments.ToArray()
                );

                Log.Information("Recurring report sent for {Subject} on {Day}", reportSubject, dayString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing recurring report for {Subject} on {Day}", reportSubject, dayString);
            }
        }


    }
}
