using Core.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Infrastructure.Logic.Filesystem
{
    public class FileSaver : IFileSaver
    {
        public string SaveReportToFile(string directoryPath, string fileName, List<string> lines)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var fullPath = Path.Combine(directoryPath, fileName);

                File.WriteAllLines(fullPath, lines);
                Log.Information("Report saved to file: {Path}", fullPath);

                return fullPath;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving report to file.");
                throw;
            }
        }
    }
}
