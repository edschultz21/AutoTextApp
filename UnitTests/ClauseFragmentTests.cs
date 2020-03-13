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
        AutoTextHandlers _handlers;

        [TestInitialize]
        public void TestInitialize()
        {
            _handlers = GetHandlers("Definitions1.xml", "Data1.json");
        }

        private AutoTextHandlers GetHandlers(string definitionsFilename, string testDataFilename)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinitions>(definitionsFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(testDataFilename), typeof(AutoTextData));

            return new AutoTextHandlers(new AutoTextDefinitionsHandler(definitions), new AutoTextDataHandler(data));
        }

        private AutoTextTemplate GetTemplate(string data, string flatData)
        {
            return new AutoTextTemplate { Data = data, FlatData = flatData };
        }

        [TestMethod]
        public void MetricFragment_Code()
        {
            var fragment = new MetricFragment(_handlers, new MetricFragment.Parameters { MetricCode = "MSP", Template = "[METRIC CODE]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("MSP", result);
        }

        [TestMethod]
        public void MetricFragment_Name()
        {
            var fragment = new MetricFragment(_handlers, new MetricFragment.Parameters { MetricCode = "MSP", Template = "[METRIC NAME]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("Median Sales Price", result);
        }

        [TestMethod]
        public void MetricFragment_LongName()
        {
            var fragment = new MetricFragment(_handlers, new MetricFragment.Parameters { MetricCode = "MSP", Template = "[METRIC LONGNAME]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("MSP LongName", result);
        }

        [TestMethod]
        public void MetricFragment_Mix()
        {
            var fragment = new MetricFragment(_handlers, new MetricFragment.Parameters { MetricCode = "MSP", Template = "[METRIC NAME] [ACTUAL VALUE] [METRIC LONGNAME][homes]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("Median Sales Price [ACTUAL VALUE] MSP LongName homes", result);
        }

        [TestMethod]
        public void DataFragment_Current()
        {
            var fragment = new DataFragment(_handlers, 
                new DataFragment.Parameters { 
                    MetricCode = "MSP",
                    VariableCode = "SF",
                    Templates = GetTemplate("[ACTUAL VALUE]", "[ACTUAL VALUE]")
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("82.5", result);
        }

        [TestMethod]
        public void DataFragment_Previous()
        {
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "MSP",
                    VariableCode = "SF",
                    Templates = GetTemplate("[PREVIOUS VALUE]", "[PREVIOUS VALUE]")
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("92.5", result);
        }

        [TestMethod]
        public void DataFragment_Percent()
        {
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "MSP",
                    VariableCode = "SF",
                    Templates = GetTemplate("[PCT]", "[PCT]")
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("0.04", result);
        }

        [TestMethod]
        public void DataFragment_All()
        {
            var template = "[ACTUAL VALUE] [PREVIOUS VALUE] [PCT]";
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "CS",
                    VariableCode = "TC",
                    Templates = GetTemplate(template, template)
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("92.5 82.5 0.04", result);
        }

        [TestMethod]
        public void DataFragment_Mix()
        {
            var template = "[METRIC NAME] [ACTUAL VALUE] [PREVIOUS VALUE] [PCT][homes]";
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "CS",
                    VariableCode = "TC",
                    Templates = GetTemplate(template, template)
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("[METRIC NAME] 92.5 82.5 0.04 homes", result);
        }

        [TestMethod]
        public void DataFragment_FlatDir()
        {
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "CS",
                    VariableCode = "TC",
                    Templates = GetTemplate(" [DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]")
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("were relatively unchanged", result);
        }

        [TestMethod]
        public void DataFragment_IncreasedDir()
        {
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "NL",
                    VariableCode = "TC",
                    Templates = GetTemplate("[DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]")
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("were down 7.5 percent to 92.5", result);
        }

        [TestMethod]
        public void DataFragment_DecreasedDir()
        {
            var fragment = new DataFragment(_handlers,
                new DataFragment.Parameters
                {
                    MetricCode = "NL",
                    VariableCode = "SF",
                    Templates = GetTemplate("[DIR] [PCT]% to [Previous VALUE]", "[DIR]")
                });
            var result = fragment.GetFragment();
            Assert.AreEqual("were up 7.5% to 92.5", result);
        }

        [TestMethod]
        public void VariableFragment_Name()
        {
            var fragment = new VariableFragment(_handlers, new VariableFragment.Parameters { VariableCode = "SF", Template = "[ACTUAL NAME]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("Single Family", result);
        }

        [TestMethod]
        public void VariableFragment_LongName()
        {
            var fragment = new VariableFragment(_handlers, new VariableFragment.Parameters { VariableCode = "TC", Template = "[ACTUAL LONGNAME]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("Townhouse/Condo homes", result);
        }

        [TestMethod]
        public void VariableFragment_Mix()
        {
            var fragment = new VariableFragment(_handlers, new VariableFragment.Parameters { VariableCode = "SF", Template = "[METRIC NAME] [ACTUAL NAME][Homes]" });
            var result = fragment.GetFragment();
            Assert.AreEqual("[METRIC NAME] Single Family homes", result);
        }
    }
}
