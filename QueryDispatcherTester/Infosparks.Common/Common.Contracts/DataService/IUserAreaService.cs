using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;
using Microsoft.SqlServer.Types;

namespace TenK.InfoSparks.Common.Contracts.DataService
{
    [ServiceContract]    
    public interface IUserAreaService
    {
        [OperationContract]
        [FaultContract(typeof(UserAreaFault))]
        AreaStats Add(string resourceID, int userID, string areaName, AreaDefinition areaDefinition);

        [OperationContract]
        [FaultContract(typeof(UserAreaFault))]
        AreaStats Update(string resourceID, int userID, string areaName, AreaDefinition areaDefinition, int userAreaID);

        [OperationContract]
        [FaultContract(typeof(UserAreaFault))]
        AreaStats Delete(string resourceID, int userID, int userAreaID);

        [OperationContract]
        [FaultContract(typeof (UserAreaFault))]
        Area[] List(string resourceID, int userID);

        [OperationContract]
        [FaultContract(typeof(UserAreaFault))]
        Area[] Details(string resourceID, int userID, int[] areaIds );
    }

    [ServiceContract]
    public class UserAreaFault
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
    }


    public class Area
    {
        public int UserAreaID;
        public int UserAreaKey;
        public string AreaName;
        public AreaDefinition AreaDefinition;
        public bool IsArchived;
    }


    public class AreaDefinition
    {
        public MapShape[] Shapes;

    }


    public class AreaStats
    {
        public int UserAreaID;
        public int NumListings;
        public double TotalArea;
    }

    [XmlInclude(typeof(MapPolygon))]
    [XmlInclude(typeof(MapCircle))]
    [KnownType(typeof(MapPolygon))]
    [KnownType(typeof(MapCircle))]
    public abstract class MapShape
    {
        public bool IsExcluded { get; set; }
    }

    public class MapPolygon : MapShape
    {
        public Point[] Points;
    }

    public class MapCircle : MapShape
    {
        // The first point is always the center of the circle.
        // If a second point is speficied, then it is any point on the perimeter
        // of the circle. Otherwise we use the radius (in meters).
        public Point[] Points;

        public double Radius;
    }

    public class Point
    {
        public double Latitude;
        public double Longitude;
    }

}
