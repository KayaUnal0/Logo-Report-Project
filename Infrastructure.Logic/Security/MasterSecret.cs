using System;

namespace Infrastructure.Logic.Security
{
    public static class MasterSecret
    {
        private const string EnvVar = "LOGO_APP_MASTER_KEY";
        public static string Get()
        {
            var v = Environment.GetEnvironmentVariable(EnvVar);
            if (string.IsNullOrWhiteSpace(v))
                throw new InvalidOperationException($"Set environment variable {EnvVar} for the user/service running the app.");
            return v;
        }
    }
}
