using Serilog;
using Serilog.Events;
using Common.Shared;

namespace Infrastructure.Logic.Logging
{
    public sealed class InfrastructureLoggerConfig
    {
        private static InfrastructureLoggerConfig _instance;
        private static readonly object _lock = new();
        private bool _isInitialized = false;

        public ILogger Logger { get; private set; }

        private InfrastructureLoggerConfig() { }

        public static InfrastructureLoggerConfig Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new InfrastructureLoggerConfig();
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
                Logger.Information("Infrastructure logger initialized at {Path}", settings.FilePath);
            }
        }
    }
}
