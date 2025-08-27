using Common.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using Infrastructure.Logic.Security;


namespace Infrastructure.Logic.Config
{
    public static class SettingsManager
    {
        private static string ConfigPath =>
            Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        public static (IConfiguration config, DatabaseSettings queryDb) LoadQueryDb()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var settings = config.GetSection("QueryDatabaseSettings").Get<DatabaseSettings>() ?? new DatabaseSettings();

            // decrypt password if PasswordEnc exists
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("QueryDatabaseSettings", out var dbSec))
                {
                    if (dbSec.TryGetProperty("PasswordEnc", out var encEl) && encEl.ValueKind == JsonValueKind.String)
                    {
                        settings.Password = SecretProtector.Unprotect(encEl.GetString());
                    }
                }
            }
            return (config, settings);
        }

        public static void SaveQueryDb(DatabaseSettings newSettings)
        {
            // Read existing json as a mutable doc
            var json = File.ReadAllText(ConfigPath);
            using var doc = JsonDocument.Parse(json);
            var root = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                       ?? new Dictionary<string, object>();

            root["QueryDatabaseSettings"] = new Dictionary<string, object?>
            {
                ["Server"] = newSettings.Server,
                ["Database"] = newSettings.Database,
                ["UserId"] = newSettings.UserId,
                ["Encrypt"] = newSettings.Encrypt,
                ["TrustServerCertificate"] = newSettings.TrustServerCertificate,
                ["PasswordEnc"] = SecretProtector.Protect(newSettings.Password)
            };


            var updated = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, updated);
        }

        public static (IConfiguration config, EmailSettings email) LoadEmail()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var settings = config.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();

            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("EmailSettings", out var sec))
                {
                    if (sec.TryGetProperty("SenderPasswordEnc", out var encEl) && encEl.ValueKind == JsonValueKind.String)
                    {
                        settings.SenderPassword = SecretProtector.Unprotect(encEl.GetString());
                    }
                }
            }

            return (config, settings);
        }


        public static void SaveEmail(EmailSettings newSettings)
        {
            var json = File.ReadAllText(ConfigPath);
            var root = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                       ?? new Dictionary<string, object>();

            root["EmailSettings"] = new Dictionary<string, object?>
            {
                ["SenderEmail"] = newSettings.SenderEmail,
                ["SmtpServer"] = newSettings.SmtpServer,
                ["SmtpPort"] = newSettings.SmtpPort,
                ["SenderPasswordEnc"] = SecretProtector.Protect(newSettings.SenderPassword)
            };


            var updated = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, updated);
        }


    }
}
