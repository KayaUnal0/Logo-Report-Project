using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Email;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Logging;
using Infrastructure.Logic.Templates;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    InfrastructureLoggerConfig.Instance.Logger.Warning("'{Subject}' başlıklı rapor bulunamadı.", reportSubject);
                    return;
                }

                var result = sqlRunner.ExecuteQuery(report.Query);

                string htmlBody = null;
                var attachments = new List<string>();
                string safeFileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
                var fileSaver = new FileSaver();

                // render the HTML body for the email
                htmlBody = templateRenderer.RenderTemplateAsync("Templates/EmailTemplate.sbn", new
                {
                    subject = report.Subject,
                    status = result.Status.ToString(),
                    results = string.Join("\n", result.Results),
                    filePath = report.Directory
                }).Result;

                // Save CSV if Excel or Excel+HTML
                if (report.FileType.Contains("Excel"))
                {
                    var csvPath = fileSaver.SaveCsvToFile(report.Directory, safeFileName + ".csv", result.Results);
                    if (!string.IsNullOrEmpty(csvPath))
                        attachments.Add(csvPath);
                }

                // Save HTML file if HTML or Excel+HTML
                if (report.FileType.Contains("HTML"))
                {
                    var htmlPath = Path.Combine(report.Directory, safeFileName + ".html");
                    File.WriteAllText(htmlPath, htmlBody);
                    attachments.Add(htmlPath);
                }

                // Send email with attachments and the rendered template as body
                sender.Send(
                    report.Email,
                    report.Subject,
                    htmlBody,
                    attachments.ToArray()
                );

                InfrastructureLoggerConfig.Instance.Logger.Information("Zamanlanmış rapor gönderildi: {Subject} ({Day})", reportSubject, dayString);
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(ex, "Zamanlanmış rapor yürütülürken hata oluştu: {Subject} ({Day})", reportSubject, dayString);
            }
        }
    }
}
