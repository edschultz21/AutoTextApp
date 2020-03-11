using System;
using System.IO;
using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class ClauseFragmentTests
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
            var definitions = AutoTextUtils.ReadXmlData<AutoTextDefinitions>(definitionsFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(testDataFilename), typeof(AutoTextData));

            return new AutoTextHandlers(new AutoTextDefinitionHandler(definitions), new AutoTextDataHandler(data));
        }

        [TestMethod]
        public void MetricFragment_Code()
        {
            var fragment = new MetricFragment(_handlers, "MSP", "[METRIC CODE]");
            var result = fragment.GetFragment();
            Assert.AreEqual("MSP", result);
        }

        [TestMethod]
        public void MetricFragment_Name()
        {
            var fragment = new MetricFragment(_handlers, "MSP", "[METRIC NAME]");
            var result = fragment.GetFragment();
            Assert.AreEqual("Median Sales Price", result);
        }

        [TestMethod]
        public void MetricFragment_LongName()
        {
            var fragment = new MetricFragment(_handlers, "MSP", "[METRIC LONGNAME]");
            var result = fragment.GetFragment();
            Assert.AreEqual("MSP LongName", result);
        }

        [TestMethod]
        public void MetricFragment_Mix()
        {
            var fragment = new MetricFragment(_handlers, "MSP", "[METRIC NAME] [ACTUAL VALUE] [METRIC LONGNAME][homes]");
            var result = fragment.GetFragment();
            Assert.AreEqual("Median Sales Price [ACTUAL VALUE] MSP LongName homes", result);
        }

        [TestMethod]
        public void VariableFragment_Current()
        {
            var fragment = new VariableFragment(_handlers, "MSP", "SF", "[ACTUAL VALUE]");
            var result = fragment.GetFragment();
            Assert.AreEqual("82.5", result);
        }

        [TestMethod]
        public void VariableFragment_Previous()
        {
            var fragment = new VariableFragment(_handlers, "MSP", "SF", "[PREVIOUS VALUE]");
            var result = fragment.GetFragment();
            Assert.AreEqual("92.5", result);
        }

        [TestMethod]
        public void VariableFragment_Percent()
        {
            var fragment = new VariableFragment(_handlers, "MSP", "SF", "[PCT]");
            var result = fragment.GetFragment();
            Assert.AreEqual("0.04", result);
        }

        [TestMethod]
        public void VariableFragment_All()
        {
            var fragment = new VariableFragment(_handlers, "CS", "TC", "[ACTUAL VALUE] [PREVIOUS VALUE] [PCT]");
            var result = fragment.GetFragment();
            Assert.AreEqual("92.5 82.5 0.04", result);
        }

        [TestMethod]
        public void VariableFragment_Mix()
        {
            var fragment = new VariableFragment(_handlers, "CS", "TC", "[METRIC NAME] [ACTUAL VALUE] [PREVIOUS VALUE] [PCT][homes]");
            var result = fragment.GetFragment();
            Assert.AreEqual("[METRIC NAME] 92.5 82.5 0.04 homes", result);
        }
    }
}
