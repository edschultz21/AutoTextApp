namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    public class Server
    {
        public string ServerKey { get; set; }
        public string SSASConnectionString { get; set; }
        public string QueryDisparcherUri { get; set; }

        public const string SELECT_QUERY = "SELECT * FROM dbo.Server";
    }
}
