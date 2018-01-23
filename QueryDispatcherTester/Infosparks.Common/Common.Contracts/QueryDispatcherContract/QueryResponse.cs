using TenK.InfoSparks.Common.AnalysisServices;
namespace TenK.InfoSparks.Common.Contracts.QueryDispatcherContract
{
    public class QueryResponse
    {
        public enum ResponseStatusCode
        {
            QUERY_OK, 
            QUERY_ERROR // ALL queries failed
        }

        public string Source { get; set; }
        public string QueryID { get; set; }
        public ResponseStatusCode Status { get; set; }
        public PartitionMDXQueryResult[] Results;
    }

    public class PartitionMDXQueryResult
    {
        public enum PartitionQueryResultStatus
        {
            OK,
            ERROR_DATABASE_UNAVAILABLE,
            ERROR_QUERY_ERROR,
            ERROR_OTHER
        }
        public string PartitionID { get; set; }
        public MDXQueryResult Result { get; set; }
        public PartitionQueryResultStatus Status { get; set; }
        public string Message { get; set; }

    }
}
