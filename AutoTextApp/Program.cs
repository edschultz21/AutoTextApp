using System.IO;
using Newtonsoft.Json;

namespace AutoTextApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinition>("Definitions.xml");
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText("CRMLS_Data.json"), typeof(AutoTextData));
            var autoText = new AutoText(definitions, data);
            
            autoText.Run();
        }
    }
}
