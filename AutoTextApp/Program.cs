using System.IO;
using Newtonsoft.Json;

namespace AutoTextApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinitions>(@"..\..\..\UnitTests\Definitions1.xml");
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(@"..\..\..\UnitTests\Data1.json"), typeof(AutoTextData));

            var handlers = new AutoTextHandlers(new AutoTextDefinitionsHandler(definitions), new AutoTextDataHandler(data));

            var fragment = new SentenceBuilder(handlers);
            var result = fragment.GetFragment();
        }
    }
}
