using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IFileSaver
    {
        string SaveReportToFile(string directoryPath, string fileName, List<string> lines);
    }
}
