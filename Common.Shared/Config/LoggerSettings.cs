using Serilog.Events;

namespace Common.Shared
{
    public class LoggerSettings
    {
        public string ProjectName { get; set; } = "Default";
        public string FilePath { get; set; } = "Logs/default-log-.txt";
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Debug;
    }
}
