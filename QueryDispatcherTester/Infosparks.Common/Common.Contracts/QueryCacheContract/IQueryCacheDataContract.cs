using System.ServiceModel;

namespace TenK.InfoSparks.Common.Contracts.QueryCacheContract
{
    [ServiceContract]
    [ServiceKnownType(typeof(DataResponse))]
    [ServiceKnownType(typeof(DataRequest))]
    public interface IQueryCacheDataContract
    {
        /// <summary>
        /// Method for running queries through a dispatcher
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        DataResponse GetQueryResult(DataRequest request);

        /// <summary>
        /// Methor for running commands
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        DataResponse RunCommand(DataRequest request);
    }
}
