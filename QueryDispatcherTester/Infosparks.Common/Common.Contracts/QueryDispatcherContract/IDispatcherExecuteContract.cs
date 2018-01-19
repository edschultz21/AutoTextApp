using System.ServiceModel;

namespace TenK.InfoSparks.Common.Contracts.QueryDispatcherContract
{
    [ServiceContract(Namespace = "http://TenK.QueryDispatcherService", CallbackContract = typeof(IDispatcherExecuteCallbackContract))]
    public interface IDispatcherExecuteContract
    {
        [OperationContract (IsOneWay = true)]
        void ExecuteQuery(string serviceKey, QueryRequest request);

        [OperationContract(IsOneWay = true)]
        void ExecuteCommand(string serviceKey, QueryRequest request);

        [OperationContract(IsOneWay = true)]
        void RegisterService(string serviceKey);

        [OperationContract]
        bool Ping(string serviceKey);
    }

    public interface IDispatcherExecuteCallbackContract
    {
        [OperationContract(IsOneWay = true)]
        void ExecuteQueryComplete(string serviceKey, QueryResponse response);
    }
}
