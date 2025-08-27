using Serilog;
using Serilog.Events;
using Common.Shared;

namespace Infrastructure.Logic.Logging
{
    public sealed class InfrastructureLoggerConfig
    {
        private static InfrastructureLoggerConfig _instance;
        private static readonly object Lock = new();
        private bool IsInitialized = false;

        public ILogger Logger { get; private set; }

        private InfrastructureLoggerConfig() { }

        public static InfrastructureLoggerConfig Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance ??= new InfrastructureLoggerConfig();
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
                Logger.Information("Infrastructure logger initialized at {Path}", settings.FilePath);
            }
        }
    }
}
