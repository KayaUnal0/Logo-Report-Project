using Common.Shared.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace Common.Shared
{
    public static class ReportStorage
    {
        private static readonly List<ReportDto> _reports = new();
        private static readonly string SavePath = "SavedReports.json";

        public static void SaveToDisk()
        {
            var json = JsonSerializer.Serialize(_reports);
            File.WriteAllText(SavePath, json);
        }

        public static void LoadFromDisk()
        {
            if (File.Exists(SavePath))
            {
                var json = File.ReadAllText(SavePath);
                _reports.Clear();
                var loaded = JsonSerializer.Deserialize<List<ReportDto>>(json);
                if (loaded != null)
                    _reports.AddRange(loaded);
            }
        }

        public static void AddReport(ReportDto report)
        {
            _reports.Add(report);
            SaveToDisk();
        }

        public static void RemoveReport(ReportDto report)
        {
            _reports.Remove(report);
        }

        public static void DeleteReport(int index)
        {
            if (index >= 0 && index < _reports.Count)
            {
                _reports.RemoveAt(index);
                SaveToDisk();
            }
        }

        public static List<ReportDto> GetAllReports()
        {
            return _reports.ToList(); // copy for safety
        }

        public static List<ReportDto> LoadReports()
        {
            return GetAllReports();
        }

        public static void ClearReports()
        {
            _reports.Clear();
        }

        public static void UpdateReport(ReportDto updated)
        {
            var index = _reports.FindIndex(r =>
                r.Subject == updated.Subject && r.Directory == updated.Directory);
            if (index >= 0)
            {
                _reports[index] = updated;
                SaveToDisk();
            }
        }
    }
}
