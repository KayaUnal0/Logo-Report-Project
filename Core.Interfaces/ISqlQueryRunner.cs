using System.Collections.Generic;
using Common.Shared.Dtos; 

namespace Core.Interfaces
{
    public interface ISqlQueryRunner
    {
        ReportExecutionResult ExecuteQuery(string query);
    }
}
