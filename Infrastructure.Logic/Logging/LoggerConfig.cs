using Serilog;
using System;

namespace Infrastructure.Logic.Logging
{
    public sealed class LoggerConfig
    {
        private static LoggerConfig _instance;
        private static readonly object _lock = new();

        private bool _isInitialized = false;

        private LoggerConfig() { }

        public static LoggerConfig Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new LoggerConfig();
                }
            }
        }

        public void Init(Common.Shared.LoggerSettings settings)
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(settings.MinimumLevel)
                    .WriteTo.Console()
                    .WriteTo.File(settings.FilePath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                _isInitialized = true;
                Log.Information("Logger initialized for project: {Project}", settings.ProjectName);
            }
        }
    }
}
