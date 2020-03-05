using System;
using System.IO;
using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class BasicUnitTests
    {
        [TestMethod]
        public void All()
        {
            //Random random = new Random(381654729);
            //var autoText = new AutoText();

            //var definitions = Utils.ReadXmlData<AutoTextDefinition>("Definitions.xml");
            //var data = Utils.ReadXmlData<AutoTextData>("CRMLS_Data.xml");
            //var newData = (AutoTextData1)JsonConvert.DeserializeObject(File.ReadAllText("CRMLS_Data.json"), typeof(AutoTextData1));
            //autoText.SetDirection(definitions, data); // EZSTODO - move into AutoText
            //var variables = definitions.MacroVariables?.ToDictionary(x => $"[{x.Name}]", y => string.IsNullOrEmpty(y?.Value) ? "" : y.Value) ?? new Dictionary<string, string>();

            //var results = new Dictionary<MetricData, string>();
            //foreach (var block in newData.Blocks)
            //{
            //    foreach (var blockItem in block.BlockItems)
            //    {
            //        var seed = random.Next(int.MaxValue);

            //        // EZSTODO - sort property values by percent change (largest to smallest)
            //        // - need to handle conjunction correctly. Above needs to be sorted by pos/neg change. All positive/negatives are
            //        // "and"ed while the two are "or"ed.
            //        foreach (var propertyValue in sentence.PropertyValues)
            //        {
            //            var result = GetSentenceFragment(definitions, variables, sentence.Code, propertyValue, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
            //            results.Add(propertyValue, result);
            //        }
            //    }
            //}

            //foreach (var paragraph in data.Paragraphs)
            //{
            //    // EZSTODO - sort sentences by percent change (largest to smallest)
            //    foreach (var sentence in paragraph.Sentences)
            //    {
            //        var seed = random.Next(int.MaxValue);

            //        // EZSTODO - sort property values by percent change (largest to smallest)
            //        // - need to handle conjunction correctly. Above needs to be sorted by pos/neg change. All positive/negatives are
            //        // "and"ed while the two are "or"ed.
            //        foreach (var propertyValue in sentence.PropertyValues)
            //        {
            //            var result = GetSentenceFragment(definitions, variables, sentence.Code, propertyValue, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
            //            results.Add(propertyValue, result);
            //        }
            //    }
            //}
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    // Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
        //    var definitions = AutoText.ReadXmlData<AutoTextDefinition>("Definitions.xml");
        //    var data = JsonConvert.DeserializeObject(File.ReadAllText("Test1_Data.json"), typeof(AutoTextData1));

        //}
    }
}
