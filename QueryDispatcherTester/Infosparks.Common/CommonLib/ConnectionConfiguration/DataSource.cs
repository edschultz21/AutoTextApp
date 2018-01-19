namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    public class DataSource
    {
        public string DataSourceKey { get; set; }
        public string CubeName { get; set; }
        public string SQLConnectionString { get; set; }

        public const string SELECT_QUERY = "SELECT * FROM dbo.DataSource";

    }
}
