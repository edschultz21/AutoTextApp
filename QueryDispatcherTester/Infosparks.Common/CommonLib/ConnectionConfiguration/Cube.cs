namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    public class Cube
    {
        public string DataSourceKey { get; set; }
        public string ServerKey { get; set; }
        public string CubeDatabase { get; set; }

        public const string SELECT_QUERY = "SELECT * FROM dbo.Cube";

    }
}
