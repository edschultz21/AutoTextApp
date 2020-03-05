using System.IO;
using Newtonsoft.Json;

namespace AutoTextApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinition>("Definitions.xml");
            var data = Utils.ReadXmlData<AutoTextData>("CRMLS_Data.xml");
            var newData = (AutoTextData1)JsonConvert.DeserializeObject(File.ReadAllText("CRMLS_Data.json"), typeof(AutoTextData1));
            var autoText = new AutoText(definitions, data, newData);
            
            autoText.Run();
        }
    }
}
