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
        private Random _random;
        private int _seed;
        AutoTextHandlers _handlers;

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(381654729);
            _seed = _random.Next(int.MaxValue);
            _handlers = GetHandlers("Definitions1.xml", "Data1.json");
        }

        private AutoTextHandlers GetHandlers(string definitionsFilename, string testDataFilename)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinitions>(definitionsFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(testDataFilename), typeof(AutoTextData));

            return new AutoTextHandlers(new AutoTextDefinitionsHandler(definitions), new AutoTextDataHandler(data));
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
        public void SentenceFragment1()
        {
            var fragment = new SentenceBuilder(_handlers);
            var result = fragment.GetFragment();
            Assert.AreEqual("New Listings were up 7.5 percent to 82.5 for Single Family and softened 7.5 percent to 92.5 for Townhouse/Condo. New Listings rose 7.5 percent to 82.5 for Single Family and fell 7.5 percent to 92.5 for Townhouse/Condo.", result);
        }
    }
}
