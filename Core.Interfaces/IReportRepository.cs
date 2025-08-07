using Common.Shared.Dtos;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IReportRepository
    {
        void SaveReport(ReportDto report);
        List<ReportDto> GetReports();
        ReportDto? GetReportBySubject(string subject);

        void DeleteReport(string subject);
        void UpdateReport(string originalTitle, ReportDto report);

    }
}
