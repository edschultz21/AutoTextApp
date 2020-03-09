using System.IO;
using System.Xml.Serialization;

namespace AutoTextApp
{
    public class Utils
    {
        public static T ReadXmlData<T>(string filename)
        {
            StreamReader streamReader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(streamReader);
        }
    }
}
