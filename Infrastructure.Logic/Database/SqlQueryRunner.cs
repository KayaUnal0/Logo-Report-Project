using Common.Shared;
using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Infrastructure.Logic.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;

namespace Infrastructure.Logic.Database
{
    public class SqlQueryRunner : ISqlQueryRunner
    {
        private readonly string _connectionString;
        public SqlQueryRunner(IConfiguration config)
        {
            var dbSettings = config.GetSection("QueryDatabaseSettings").Get<DatabaseSettings>();

            _connectionString = $"Server={dbSettings.Server};" +
                                $"Database={dbSettings.Database};" +
                                $"User Id={dbSettings.UserId};" +
                                $"Password={dbSettings.Password};" +
                                $"Encrypt={dbSettings.Encrypt};" +
                                $"TrustServerCertificate={dbSettings.TrustServerCertificate};";
        }

        public ReportExecutionResult ExecuteQuery(string sql)
        {
            var result = new ReportExecutionResult
            {
                Results = new List<string>()
            };

            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    return new ReportExecutionResult
                    {
                        Results = new List<string> { "No query to execute." }
                    };
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    InfrastructureLoggerConfig.Instance.Logger.Information("Veritabanına bağlantı sağlandı.");

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        // Header row
                        var headers = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            headers.Add(reader.GetName(i));
                        }
                        result.Results.Add(string.Join("\t", headers));

                        // Data rows
                        while (reader.Read())
                        {
                            var row = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader[i]?.ToString() ?? "");
                            }
                            result.Results.Add(string.Join("\t", row));
                        }
                    }
                }

                result.Status = ReportStatus.Sent;
                InfrastructureLoggerConfig.Instance.Logger.Information("SQL sorgusu başarıyla çalıştırıldı.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerConfig.Instance.Logger.Error(ex, "SQL sorgusu çalıştırılırken hata oluştu.");
                result.Status = ReportStatus.Failed;
                result.Results.Add("Hata: " + ex.Message);
            }

            return result;
        }
    }
}
