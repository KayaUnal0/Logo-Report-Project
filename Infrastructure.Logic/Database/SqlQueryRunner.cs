using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Serilog;
using Core.Interfaces;
using Common.Shared.Dtos;
using Common.Shared.Enums;

namespace Infrastructure.Logic.Database
{
    public class SqlQueryRunner : ISqlQueryRunner
    {
        private readonly string _connectionString;

        public SqlQueryRunner()
        {
            _connectionString = "Server=KAYAUNAL;Database=LogoProject;User Id=sa;Password=1;Encrypt=True;TrustServerCertificate=True;";
        }

        public ReportExecutionResult ExecuteQuery(string sql)
        {
            var result = new ReportExecutionResult
            {
                Results = new List<string>()
            };

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Log.Information("Veritabanına bağlantı sağlandı.");

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
                Log.Information("SQL sorgusu başarıyla çalıştırıldı.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SQL sorgusu çalıştırılırken hata oluştu.");
                result.Status = ReportStatus.Failed;
                result.Results.Add("Hata: " + ex.Message);
            }

            return result;
        }
    }
}
