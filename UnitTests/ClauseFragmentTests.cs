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
            var definitions = Utils.ReadXmlData<AutoTextDefinitions>(definitionsFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(testDataFilename), typeof(AutoTextData));

            return new AutoTextHandlers(new AutoTextDefinitionsHandler(definitions), new AutoTextDataHandler(data));
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
        public void DataFragment_Current()
        {
            var fragment = new DataFragment(_handlers, "MSP", "SF", "[ACTUAL VALUE]", "[ACTUAL VALUE]");
            var result = fragment.GetFragment();
            Assert.AreEqual("82.5", result);
        }

        [TestMethod]
        public void DataFragment_Previous()
        {
            var fragment = new DataFragment(_handlers, "MSP", "SF", "[PREVIOUS VALUE]", "[PREVIOUS VALUE]");
            var result = fragment.GetFragment();
            Assert.AreEqual("92.5", result);
        }

        [TestMethod]
        public void DataFragment_Percent()
        {
            var fragment = new DataFragment(_handlers, "MSP", "SF", "[PCT]", "[PCT]");
            var result = fragment.GetFragment();
            Assert.AreEqual("0.04", result);
        }

        [TestMethod]
        public void DataFragment_All()
        {
            var template = "[ACTUAL VALUE] [PREVIOUS VALUE] [PCT]";
            var fragment = new DataFragment(_handlers, "CS", "TC", template, template);
            var result = fragment.GetFragment();
            Assert.AreEqual("92.5 82.5 0.04", result);
        }

        [TestMethod]
        public void DataFragment_Mix()
        {
            var template = "[METRIC NAME] [ACTUAL VALUE] [PREVIOUS VALUE] [PCT][homes]";
            var fragment = new DataFragment(_handlers, "CS", "TC", template, template);
            var result = fragment.GetFragment();
            Assert.AreEqual("[METRIC NAME] 92.5 82.5 0.04 homes", result);
        }

        [TestMethod]
        public void DataFragment_FlatDir()
        {
            var fragment = new DataFragment(_handlers, "CS", "TC", " [DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]");
            var result = fragment.GetFragment();
            Assert.AreEqual("were relatively unchanged", result);
        }

        [TestMethod]
        public void DataFragment_IncreasedDir()
        {
            var fragment = new DataFragment(_handlers, "NL", "TC", "[DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]");
            var result = fragment.GetFragment();
            Assert.AreEqual("were down 7.5 percent to 92.5", result);
        }

        [TestMethod]
        public void DataFragment_DecreasedDir()
        {
            var fragment = new DataFragment(_handlers, "NL", "SF", "[DIR] [PCT]% to [Previous VALUE]", "[DIR]");
            var result = fragment.GetFragment();
            Assert.AreEqual("were up 7.5% to 92.5", result);
        }

        [TestMethod]
        public void VariableFragment_Name()
        {
            var fragment = new VariableFragment(_handlers, "SF", "[ACTUAL NAME]");
            var result = fragment.GetFragment();
            Assert.AreEqual("Single Family", result);
        }

        [TestMethod]
        public void VariableFragment_LongName()
        {
            var fragment = new VariableFragment(_handlers, "TC", "[ACTUAL LONGNAME]");
            var result = fragment.GetFragment();
            Assert.AreEqual("Townhouse/Condo homes", result);
        }

        [TestMethod]
        public void VariableFragment_Mix()
        {
            var fragment = new VariableFragment(_handlers, "SF", "[METRIC NAME] [ACTUAL NAME][Homes]");
            var result = fragment.GetFragment();
            Assert.AreEqual("[METRIC NAME] Single Family homes", result);
        }
    }
}
