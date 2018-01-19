using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace TenK.InfoSparks.Common.Contracts.DataService
{

    [ServiceContract]
    public interface IDocumentService
    {
        [OperationContract]
        [FaultContract(typeof(DocumentServiceFault))]
        DocumentData GetDocument(string resourceID, int userID, string reportType, string locationType, string locationName, DateTime reportDate, int fileID, string source);

        [OperationContract]
        [FaultContract(typeof (DocumentServiceFault))]
        DocumentInfo[] GetDocumentList(string resourceID, int userID, string reportType, DateTime reportDate, int numDatesLimit = 1);

        [OperationContract]
        [FaultContract(typeof(DocumentServiceFault))]
        DateTime[] GetAvailableDateList(string resourceID, int userID, string reportType);

        [OperationContract]
        [FaultContract(typeof (DocumentServiceFault))]
        void ForceDocumentRefresh();
    }

    [ServiceContract]
    public class DocumentInfo
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

    }

    [ServiceContract]
    public class DocumentData
    {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public byte[] FileData { get; set; }
    }

    [ServiceContract]
    public class DocumentServiceFault
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
