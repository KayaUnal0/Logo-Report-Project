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
        private readonly string _connectionString;

        public ReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveReport(ReportDto report)
        {
            var json = JsonSerializer.Serialize(report);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
            INSERT INTO Reports (Title, Subject, Query, Email, Period, Directory, IsActive, JsonContent)
            VALUES (@Title, @Subject, @Query, @Email, @Period, @Directory, @IsActive, @Json)", conn);

            cmd.Parameters.AddWithValue("@Title", report.Subject ?? "");
            cmd.Parameters.AddWithValue("@Subject", report.Subject ?? "");
            cmd.Parameters.AddWithValue("@Query", report.Query ?? "");
            cmd.Parameters.AddWithValue("@Email", report.Email);
            cmd.Parameters.AddWithValue("@Period", report.Period ?? "");
            cmd.Parameters.AddWithValue("@Directory", string.IsNullOrWhiteSpace(report.Directory) ? DBNull.Value : report.Directory);
            cmd.Parameters.AddWithValue("@IsActive", true);
            cmd.Parameters.AddWithValue("@Json", json);

            cmd.ExecuteNonQuery();
        }

        public List<ReportDto> GetReports()
        {
            var list = new List<ReportDto>();

            using var conn = new SqlConnection(_connectionString);
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

        public void DeleteReport(string subject)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
            DELETE FROM Reports
            WHERE Title = @Title", conn);

            cmd.Parameters.AddWithValue("@Title", subject);
            cmd.ExecuteNonQuery();
        }


        public void UpdateReport(string originalTitle, ReportDto report)
        {
            var json = JsonSerializer.Serialize(report);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
            UPDATE Reports
            SET Title = @Title,
                Subject = @Subject,
                Query = @Query,
                Email = @Email,
                Period = @Period,
                Directory = @Directory,
                IsActive = @IsActive,
                JsonContent = @Json
            WHERE Title = @OriginalTitle", conn);

            cmd.Parameters.AddWithValue("@Title", report.Subject ?? "");
            cmd.Parameters.AddWithValue("@Subject", report.Subject ?? "");
            cmd.Parameters.AddWithValue("@Query", report.Query ?? "");
            cmd.Parameters.AddWithValue("@Email", report.Email);
            cmd.Parameters.AddWithValue("@Period", report.Period ?? "");
            cmd.Parameters.AddWithValue("@Directory", string.IsNullOrWhiteSpace(report.Directory) ? DBNull.Value : report.Directory);
            cmd.Parameters.AddWithValue("@IsActive", true);
            cmd.Parameters.AddWithValue("@Json", json);
            cmd.Parameters.AddWithValue("@OriginalTitle", originalTitle);

            cmd.ExecuteNonQuery();
        }

    }
}
