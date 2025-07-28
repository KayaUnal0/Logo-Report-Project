using Common.Shared.Dtos;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IReportRepository
    {
        void SaveReport(ReportDto report);
        List<ReportDto> GetReports();
        void DeleteReport(string subject);
        void UpdateReport(string originalTitle, ReportDto report);
    }
}
