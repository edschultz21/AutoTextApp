using System;
using System.IO;
using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class SentenceFragmentTests
    {
        AutoTextHandlers _handlers;

        [TestInitialize]
        public void TestInitialize()
        {
            _handlers = GetHandlers("Definitions1.xml", "Data1.json");
        }

        private AutoTextHandlers GetHandlers(string definitionFilename, string dataFilename)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinitions>(definitionFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(dataFilename), typeof(AutoTextData));

            return new AutoTextHandlers(new AutoTextDefinitionsProvider(definitions), new AutoTextDataProvider(data));
        }

        [TestMethod]
        public void StandardFragment1()
        {
            var fragment = new StandardFragment(_handlers, _handlers.DataHandler.Blocks[1].BlockItems[0]);
            var result = fragment.GetFragment();
            Assert.AreEqual("New Listings were up 7.5 percent to 82.5 for Single Family and softened 7.5 percent to 92.5 for Townhouse/Condo", result);
        }

        [TestMethod]
        public void StandardFragment2()
        {
            var fragment = new StandardFragment(_handlers, _handlers.DataHandler.Blocks[1].BlockItems[1]);
            var result = fragment.GetFragment();
            Assert.AreEqual("Closed Sales were relatively unchanged for Single Family and softened 27.5 percent to 264.7 for Mobile", result);
        }

        [TestMethod]
        public void TemplateFragment1()
        {
            var fragment = new TemplateFragment(_handlers, _handlers.DataHandler.Blocks[0].BlockItems[0], 
                new AutoTextTemplate
                {
                    Metric = "[METRIC CODE]",
                    Data = " with value of [ACTUAL VALUE] [DIR] by [PCT] percent"
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("MSI with value of 2.4 were up by 12.3 percent", result);
        }

        [TestMethod]
        public void SentenceFragment1()
        {
            var handlers = GetHandlers("Definitions1.xml", "Data2.json");

            var fragment = new SentenceBuilder(handlers);
            var result = fragment.GetFragment();
            Assert.AreEqual("Median Sales Price were relatively unchanged for Single Family and softened 13.9 percent to 209000 for Townhouse/Condo. New Listings rose 7.5 percent to 82.5 for Single Family and fell 7.5 percent to 92.5 for Townhouse/Condo. Closed Sales were fairly even for Single Family and remained flat for Townhouse/Condo.", result);
        }
    }
}
