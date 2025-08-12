using Common.Shared;                
using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
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
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                // Settings
                var emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>();
                var dbSettings = config.GetSection("AppDatabaseSettings").Get<DatabaseSettings>();
                var connectionString = dbSettings.ToConnectionString();

                // Services
                var sender = new EmailSender(emailSettings);
                var sqlRunner = new SqlQueryRunner(config);
                var templateRenderer = new TemplateRenderer();
                var reportRepo = new ReportRepository(connectionString); 

                // Get report
                var report = reportRepo.GetReportBySubject(reportSubject);
                if (report == null)
                {
                    InfrastructureLoggerConfig.Instance.Logger.Warning("'{Subject}' başlıklı rapor bulunamadı.", reportSubject);
                    return;
                }

                // Run SQL
                var result = sqlRunner.ExecuteQuery(report.Query);

                // Build HTML body (table)
                var tableHtml = HtmlTableBuilder.FromTsvLines(result.Results);
                var htmlBody = templateRenderer.RenderTemplateAsync("Templates/EmailTemplate.sbn", new
                {
                    subject = report.Subject,
                    status = result.Status.ToString(),
                    results_table = tableHtml,
                    filePath = report.Directory
                }).Result;

                // Attachments
                var attachments = new List<string>();
                var safeFileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
                var fileSaver = new FileSaver();

                if (report.FileType.Contains("Excel"))
                {
                    var csvPath = fileSaver.SaveCsvToFile(report.Directory, safeFileName + ".csv", result.Results);
                    if (!string.IsNullOrEmpty(csvPath)) attachments.Add(csvPath);
                }

                if (report.FileType.Contains("HTML"))
                {
                    // Save as full HTML document
                    var htmlPath = fileSaver.SaveHtmlToFile(report.Directory, safeFileName + ".html", htmlBody);
                    attachments.Add(htmlPath);
                }

                // Send
                sender.Send(report.Email, report.Subject, htmlBody, attachments.ToArray());

                InfrastructureLoggerConfig.Instance.Logger.Information("Zamanlanmış rapor gönderildi: {Subject} ({Day})", reportSubject, dayString);
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(ex, "Zamanlanmış rapor yürütülürken hata oluştu: {Subject} ({Day})", reportSubject, dayString);
            }
        }
    }
}
