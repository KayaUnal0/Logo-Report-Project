using Common.Shared.Enums;
using System.Collections.Generic;

namespace Common.Shared.Dtos
{
    public class ReportExecutionResult
    {
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public List<string> Results { get; set; } = new();
    }
}
