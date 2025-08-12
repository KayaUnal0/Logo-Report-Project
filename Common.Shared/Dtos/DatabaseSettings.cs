namespace Common.Shared.Dtos
{
    public class DatabaseSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool Encrypt { get; set; }
        public bool TrustServerCertificate { get; set; }

        public string ToConnectionString()
        {
            return $"Server={Server};Database={Database};User Id={UserId};Password={Password};" +
                   $"Encrypt={Encrypt};TrustServerCertificate={TrustServerCertificate};";
        }
    }
}
