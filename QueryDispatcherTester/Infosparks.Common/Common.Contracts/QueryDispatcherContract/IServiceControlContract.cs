using System.ServiceModel;

namespace TenK.InfoSparks.Common.Contracts.QueryDispatcherContract
{
    [ServiceContract(Namespace = "http://TenK.QueryDispatcherService")]//;
    public interface IServiceControlContract
    {
        [OperationContract]
        bool ClearCubeCache(string targetCubeCode,  int scope, string measureGroup, string dimension);
    }
}
