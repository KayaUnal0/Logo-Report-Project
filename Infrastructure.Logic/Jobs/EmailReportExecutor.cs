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
                var (email, queryDb, appDb) = LoadAllSettings();
                var appConn = appDb.ToConnectionString();

                var (sender, sqlRunner, templateRenderer, reportRepo, fileSaver) =
                    BuildServices(email, queryDb, appConn);

                var report = GetReportOrLog(reportRepo, reportSubject);
                if (report is null) return;

                var result = sqlRunner.ExecuteQuery(report.Query);

                var htmlBody = RenderEmail(templateRenderer, report, result);


                var attachments = BuildAttachments(fileSaver, report, result, htmlBody);

                SendReport(sender, report, htmlBody, attachments);

                InfrastructureLoggerConfig.Instance.Logger.Information(
                    "Zamanlanmış rapor gönderildi: {Subject} ({Day})", reportSubject, dayString);
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(
                    ex, "Zamanlanmış rapor yürütülürken hata oluştu: {Subject} ({Day})", reportSubject, dayString);
            }
        }

        private static (EmailSettings email, DatabaseSettings queryDb, DatabaseSettings appDb) LoadAllSettings()
        {
            // IConfiguration is still built, but we rely on SettingsManager to decrypt sensitive values
            _ = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var (_, email) = SettingsManager.LoadEmail();
            var (_, queryDb) = SettingsManager.LoadQueryDb();
            var (_, appDb) = SettingsManager.LoadAppDb();

            return (email, queryDb, appDb);
        }

        private static (EmailSender sender,
                        SqlQueryRunner sqlRunner,
                        TemplateRenderer templateRenderer,
                        ReportRepository reportRepo,
                        FileSaver fileSaver)
            BuildServices(EmailSettings email, DatabaseSettings queryDb, string appConnectionString)
        {
            var sender = new EmailSender(email);
            var sqlRunner = new SqlQueryRunner(queryDb);
            var templateRenderer = new TemplateRenderer();
            var reportRepo = new ReportRepository(appConnectionString);
            var fileSaver = new FileSaver();

            return (sender, sqlRunner, templateRenderer, reportRepo, fileSaver);
        }

        private static ReportDto? GetReportOrLog(ReportRepository repo, string subject)
        {
            var report = repo.GetReportBySubject(subject);
            if (report == null)
            {
                InfrastructureLoggerConfig.Instance.Logger.Warning(
                    "'{Subject}' başlıklı rapor bulunamadı.", subject);
            }
            return report;
        }

        private static string RenderEmail(TemplateRenderer renderer, ReportDto report, ReportExecutionResult result)
        {
            var tableModel = BuildScribanTable(result.Results);
            // Template contains the table markup
            return renderer.RenderTemplateAsync("Templates/EmailTemplate.sbn", new
            {
                subject = report.Subject,
                status = result.Status.ToString(),
                table = tableModel,
                filePath = report.Directory
            }).Result;
        }

        private static List<string> BuildAttachments(
            FileSaver fileSaver,
            ReportDto report,
            ReportExecutionResult result,
            string htmlBody)
        {
            var attachments = new List<string>();
            var safeFileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
            var fileTypeToken = (report.FileType ?? string.Empty).ToLowerInvariant();

            if (fileTypeToken.Contains("excel"))
            {
                var csvPath = fileSaver.SaveCsvToFile(report.Directory, safeFileName + ".csv", result.Results);
                if (!string.IsNullOrEmpty(csvPath)) attachments.Add(csvPath);
            }

            if (fileTypeToken.Contains("html"))
            {
                var htmlPath = fileSaver.SaveHtmlToFile(report.Directory, safeFileName + ".html", htmlBody);
                attachments.Add(htmlPath);
            }

            return attachments;
        }

        private static void SendReport(EmailSender sender, ReportDto report, string htmlBody, List<string> attachments)
        {
            sender.Send(report.Email, report.Subject, htmlBody, attachments.ToArray());
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
