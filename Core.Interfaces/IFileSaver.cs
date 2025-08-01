using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IFileSaver
    {
        string SaveCsvToFile(string directoryPath, string fileName, List<string> lines);
        string SaveHtmlToFile(string directoryPath, string fileName, string htmlContent);

    }
}
