using Common.Shared;                
using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Logic.Config;
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
                var (_, email) = SettingsManager.LoadEmail(); 
                var (_, queryDb) = SettingsManager.LoadQueryDb();
                var (_, appDb) = SettingsManager.LoadAppDb();  
                var connectionString = appDb.ToConnectionString();

                // Services
                var sender = new EmailSender(email);
                var sqlRunner = new SqlQueryRunner(queryDb);
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

                // Build Scriban table model from TSV
                var tableModel = BuildScribanTable(result.Results);

                // Render the whole email with Scriban (template contains table markup)
                var htmlBody = templateRenderer.RenderTemplateAsync("Templates/EmailTemplate.sbn", new
                {
                    subject = report.Subject,
                    status = result.Status.ToString(),
                    table = tableModel,              // <-- pass the model (headers + rows)
                    filePath = report.Directory
                }).Result;


                // Attachments
                var attachments = new List<string>();
                var safeFileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
                var fileSaver = new FileSaver();

                var fileType = (report.FileType ?? "").ToLowerInvariant();
                if (fileType.Contains("excel"))
                {
                    var csvPath = fileSaver.SaveCsvToFile(report.Directory, safeFileName + ".csv", result.Results);
                    if (!string.IsNullOrEmpty(csvPath)) attachments.Add(csvPath);
                }

                if (fileType.Contains("html"))
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

        private static object BuildScribanTable(List<string> tsvLines)
        {
            if (tsvLines == null || tsvLines.Count == 0)
            {
                return new { headers = Array.Empty<string>(), rows = Array.Empty<object[]>() };
            }

            var headers = (tsvLines[0] ?? string.Empty).Split('\t');

            var rows = new List<object[]>();
            for (int i = 1; i < tsvLines.Count; i++)
            {
                var cells = (tsvLines[i] ?? string.Empty).Split('\t');
                var row = new object[cells.Length];

                for (int c = 0; c < cells.Length; c++)
                {
                    var val = cells[c] ?? string.Empty;
                    row[c] = new
                    {
                        value = val,
                        is_numeric = IsNumeric(val)
                    };
                }

                rows.Add(row);
            }

            // object shape that matches your Scriban template
            return new
            {
                headers = headers,
                rows = rows
            };
        }

        private static bool IsNumeric(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return double.TryParse(value, System.Globalization.NumberStyles.Any,
                                   System.Globalization.CultureInfo.InvariantCulture, out _)
                || double.TryParse(value, System.Globalization.NumberStyles.Any,
                                   System.Globalization.CultureInfo.CurrentCulture, out _);
        }

    }
}
