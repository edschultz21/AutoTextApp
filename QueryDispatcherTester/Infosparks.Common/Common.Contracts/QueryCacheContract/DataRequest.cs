using System.Runtime.Serialization;

namespace TenK.InfoSparks.Common.Contracts.QueryCacheContract
{
    [DataContract]
    public class DataRequest
    {
        /// <summary>
        /// Data source for the query
        /// </summary>
        [DataMember]
        public string DataSource;

        /// <summary>
        /// Query's unique identifier
        /// </summary>
        [DataMember]
        public string QueryID;

        /// <summary>
        /// Query string
        /// </summary>
        [DataMember]
        public string QueryString;

        /// <summary>
        /// Query options
        /// </summary>
        [DataMember]
        public DataRequestOptions Options;
    }

    [DataContract]
    public class DataRequestOptions
    {
        /// <summary>
        /// Each request will wait this long in second before returnning with timeout
        /// </summary>
        [DataMember]
        public int? TimeoutInSeconds = null;

        /// <summary>
        /// This priority will be passed to dispatcher. Smaller values are handled prior to larger ones
        /// </summary>
        [DataMember]
        public int? Priority = null;

        /// <summary>
        /// Determines caching mode used by the QueryCache
        /// </summary>
        [DataMember]
        public int? CacheFlag = null;

        /// <summary>
        ///  If there is a timeout, get any (possibly outdated) results if possible
        /// </summary>
        [DataMember]
        public bool? ForceResultOnTimeout = null;

        /// <summary>
        /// Determines routing mode used by the QueryCache
        /// </summary>
        [DataMember]
        public int? RoutingFlag = null;

    }
}
