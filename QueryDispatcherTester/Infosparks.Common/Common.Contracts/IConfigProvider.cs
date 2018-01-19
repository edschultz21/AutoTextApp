using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;

namespace TenK.InfoSparks.Common.Contracts
{
    [ServiceContract]
    public interface IConfigProvider
    {
        [OperationContract]
        XElement ListParams();

        [OperationContract]
        string GetParam(string paramName);

        [OperationContract]
        bool SetParam(string paramName, string value);
    }
}
