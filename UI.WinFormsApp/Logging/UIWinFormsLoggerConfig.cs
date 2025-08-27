using Serilog;
using Serilog.Events;
using Common.Shared;

namespace Logo_Project.Logging
{
    public sealed class UIWinFormsLoggerConfig
    {
        private static UIWinFormsLoggerConfig _instance;
        private static readonly object Lock = new();
        private bool IsInitialized = false;

        public ILogger Logger { get; private set; } // Project-specific logger

        private UIWinFormsLoggerConfig() { }

        public static UIWinFormsLoggerConfig Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance ??= new UIWinFormsLoggerConfig();
                }
            }
        }

        public void Init(LoggerSettings settings)
        {
            if (IsInitialized) return;

            lock (Lock)
            {
                if (IsInitialized) return;

                Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(settings.MinimumLevel)
                    .WriteTo.Console()
                    .WriteTo.File(settings.FilePath, rollingInterval: RollingInterval.Day)
                    .Enrich.WithProperty("Project", settings.ProjectName)
                    .CreateLogger();

                IsInitialized = true;
                Logger.Information("UI logger initialized at {Path}", settings.FilePath);
            }
        }
    }
}
