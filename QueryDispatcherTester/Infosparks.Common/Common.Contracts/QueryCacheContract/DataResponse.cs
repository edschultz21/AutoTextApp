using System.Runtime.Serialization;
using TenK.InfoSparks.Common.AnalysisServices;

namespace TenK.InfoSparks.Common.Contracts.QueryCacheContract
{

    [DataContract]
    public class DataResponse
    {
        
        public enum ResponseStatus
        {
            TIMEOUT,
            OK,
            OK_CACHED,
            ERROR
        }

        [DataMember]
        public ResponseStatus Status;

        [DataMember]
        public MDXQueryResult MDXQueryResult;

        [DataMember]
        public string Message;
    }

}
