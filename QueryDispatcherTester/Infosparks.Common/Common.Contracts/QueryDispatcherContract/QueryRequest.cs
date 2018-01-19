namespace TenK.InfoSparks.Common.Contracts.QueryDispatcherContract
{
    public class QueryRequest
    {
        public string Database;
        public string Source;
        public string ID;
        public PartitionQuery QueryData;
        public QueryRequestOptions Options;
    }

    public class PartitionQuery
    {
        public string Query { get; set; }
        public PartitionQueryPart[] Partitions { get; set; }
    }

    public struct PartitionQueryPart
    {
        public string PartitionID { get; set; }
        public string PartitionExpression { get; set; }
    }

    public struct QueryRequestOptions
    {
        public int Priority { get; set; }

        public override string ToString()
        {
            return string.Format("Priority={0}", Priority);
        }
    }
}
