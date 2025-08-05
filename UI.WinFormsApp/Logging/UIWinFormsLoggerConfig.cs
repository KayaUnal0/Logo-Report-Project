using Serilog;
using Serilog.Events;
using Common.Shared;

namespace Logo_Project.Logging
{
    public sealed class UIWinFormsLoggerConfig
    {
        private static UIWinFormsLoggerConfig _instance;
        private static readonly object _lock = new();
        private bool _isInitialized = false;

        public ILogger Logger { get; private set; } // Project-specific logger

        private UIWinFormsLoggerConfig() { }

        public static UIWinFormsLoggerConfig Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new UIWinFormsLoggerConfig();
                }
            }
        }

        public void Init(LoggerSettings settings)
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(settings.MinimumLevel)
                    .WriteTo.Console()
                    .WriteTo.File(settings.FilePath, rollingInterval: RollingInterval.Day)
                    .Enrich.WithProperty("Project", settings.ProjectName)
                    .CreateLogger();

                _isInitialized = true;
                Logger.Information("UI logger initialized at {Path}", settings.FilePath);
            }
        }
    }
}
