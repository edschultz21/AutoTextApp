using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;

namespace TenK.InfoSparks.Common.Contracts.DataService
{
    [ServiceContract]
    public interface IListingInfoService
    {
        [OperationContract]
        [FaultContract(typeof(ListingInfoFault))]
        XElement GetListingInfo(string resourceID, string mlsFeedId, string listingID);
    }

    [ServiceContract]
    public class ListingInfoFault
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
