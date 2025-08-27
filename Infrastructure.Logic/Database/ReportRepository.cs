using Common.Shared.Dtos;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;

namespace Infrastructure.Logic.Database
{
    public class ReportRepository : IReportRepository
    {
        private readonly string ConnectionString;

        public ReportRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void SaveReport(ReportDto report)
        {
            var json = JsonSerializer.Serialize(report);
            report.JsonContent = json;

            using var conn = new SqlConnection(ConnectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
            INSERT INTO Reports (Subject, Query, Email, Period, Directory, IsActive, JsonContent)
            VALUES (@Subject, @Query, @Email, @Period, @Directory, @IsActive, @Json)", conn);

            cmd.Parameters.AddWithValue("@Subject", report.Subject ?? "");
            cmd.Parameters.AddWithValue("@Query", report.Query ?? "");
            cmd.Parameters.AddWithValue("@Email", report.Email);
            cmd.Parameters.AddWithValue("@Period", report.Period.ToString());
            cmd.Parameters.AddWithValue("@Directory", string.IsNullOrWhiteSpace(report.Directory) ? DBNull.Value : report.Directory);
            cmd.Parameters.AddWithValue("@IsActive", true);
            cmd.Parameters.AddWithValue("@Json", json);

            cmd.ExecuteNonQuery();
        }

        public List<ReportDto> GetReports()
        {
            var list = new List<ReportDto>();

            using var conn = new SqlConnection(ConnectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT JsonContent FROM Reports", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var json = reader.GetString(0);
                var report = JsonSerializer.Deserialize<ReportDto>(json);
                if (report != null)
                    list.Add(report);
            }

            return list;
        }

        public ReportDto? GetReportBySubject(string subject)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT JsonContent FROM Reports WHERE Subject = @subject", conn);
            cmd.Parameters.AddWithValue("@subject", subject);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var json = reader.GetString(0);
                return JsonSerializer.Deserialize<ReportDto>(json);
            }

            return null;
        }

        public void DeleteReport(string subject)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
            DELETE FROM Reports
            WHERE Subject = @Subject", conn);

            cmd.Parameters.AddWithValue("@Subject", subject);
            cmd.ExecuteNonQuery();
        }


        public void UpdateReport(string originalTitle, ReportDto report)
        {
            var json = JsonSerializer.Serialize(report);
            report.JsonContent = json;

            using var conn = new SqlConnection(ConnectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
            UPDATE Reports
            SET
                Subject = @Subject,
                Query = @Query,
                Email = @Email,
                Period = @Period,
                Directory = @Directory,
                IsActive = @IsActive,
                JsonContent = @Json
            WHERE Subject = @originalSubject", conn);

            cmd.Parameters.AddWithValue("@Subject", report.Subject ?? "");
            cmd.Parameters.AddWithValue("@Query", report.Query ?? "");
            cmd.Parameters.AddWithValue("@Email", report.Email);
            cmd.Parameters.AddWithValue("@Period", report.Period.ToString());
            cmd.Parameters.AddWithValue("@Directory", string.IsNullOrWhiteSpace(report.Directory) ? DBNull.Value : report.Directory);
            cmd.Parameters.AddWithValue("@IsActive", report.Active ? 1 : 0);
            cmd.Parameters.AddWithValue("@Json", json);
            cmd.Parameters.AddWithValue("@originalSubject", originalTitle);

            cmd.ExecuteNonQuery();
        }

    }
}
