using Core.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure.Logic.Filesystem
{
    public class FileSaver : IFileSaver
    {
        public string SaveCsvToFile(string directoryPath, string fileName, List<string> lines)
        {
            var csvLines = lines.Select(line =>
                string.Join(",", line.Split('\t').Select(field =>
                    field.Contains(",") || field.Contains("\"")
                        ? $"\"{field.Replace("\"", "\"\"")}\""
                        : field
                ))
            );

            var fullPath = Path.Combine(directoryPath, fileName);
            File.WriteAllLines(fullPath, csvLines);
            Log.Information("CSV report saved to: {Path}", fullPath);
            return fullPath;
        }

        public string SaveHtmlToFile(string directoryPath, string fileName, string html)
        {
            var fullPath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(fullPath, html);
            Log.Information("HTML report saved to: {Path}", fullPath);
            return fullPath;
        }
    }

}
