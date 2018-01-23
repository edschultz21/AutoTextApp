using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;

namespace TenK.InfoSparks.Common.Contracts
{
    [ServiceContract]
    public interface IStatsProvider
    {
        [OperationContract]
        [WebGet]
        XmlElement GetStats();
    }
}
