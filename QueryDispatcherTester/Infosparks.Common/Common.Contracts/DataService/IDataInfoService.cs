using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;

namespace TenK.InfoSparks.Common.Contracts.DataService
{
    [ServiceContract]
    public interface IDataInfoService
    {
        [OperationContract]
        [FaultContract(typeof(DataInfoServiceFault))]
        string GetVersionInfo();
    }

    [ServiceContract]
    public class DataInfoServiceFault
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
