using Common.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;

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
            return (config, settings);
        }

        public static void SaveQueryDb(DatabaseSettings newSettings)
        {
            // Read existing json as a mutable doc
            var json = File.ReadAllText(ConfigPath);
            using var doc = JsonDocument.Parse(json);
            var root = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                       ?? new Dictionary<string, object>();

            root["QueryDatabaseSettings"] = newSettings; // Scriban/JSON will serialize POCO

            var updated = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, updated);
        }
    }
}
