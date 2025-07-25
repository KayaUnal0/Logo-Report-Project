using Serilog;

namespace Infrastructure.Logic.Logging
{
    public static class LoggerConfig
    {
        private static bool _isInitialized = false;

        public static void Configure()
        {
            if (_isInitialized) return;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _isInitialized = true;
            Log.Information("Logger initialized.");
        }
    }
}
