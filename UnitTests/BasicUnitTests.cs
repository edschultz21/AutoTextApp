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
        private Random _random;
        private int _seed;

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(381654729);
            _seed = _random.Next(int.MaxValue);
        }

        private AutoText GetAutoText(string definitionsFilename, string testDataFilename)
        {
            var definitions = Utils.ReadXmlData<AutoTextDefinition>(definitionsFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(testDataFilename), typeof(AutoTextData));
            return new AutoText(definitions, data);
        }

        [TestMethod]
        public void SentenceFragment1()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("NL", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("New Listings were up 7.5% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void SentenceFragment2()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("CS", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Closed Sales consistent with 0.04% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void SentenceFragment3()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("MSP", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Variable not found SF", result);
        }

        [TestMethod]
        public void SentenceFragmentCasing1()
        {
            var autoText = GetAutoText("DefinitionsMixedCasing.xml", "DataMixedCasing.json");

            var result = autoText.GetSentenceFragment("nl", "SF", "[METRIC NAME] [dir] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMes]", _seed);
            Assert.AreEqual("New Listings were up 7.5% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void SentenceFragmentCasing2()
        {
            var autoText = GetAutoText("DefinitionsMixedCasing.xml", "DataMixedCasing.json");

            var result = autoText.GetSentenceFragment("CS", "sf", "[METRIC name] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Closed Sales consistent with 0.04% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void SentenceFragmentCasing3()
        {
            var autoText = GetAutoText("DefinitionsMixedCasing.xml", "DataMixedCasing.json");

            var result = autoText.GetSentenceFragment("msp", "sf", "[METRIC NAME] [DIR] [PCT] percent to [Actual VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Variable not found SF", result);
        }

        [TestMethod]
        public void VariableLongName()
        {
            // Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("MSP", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL LONGNAME]", _seed);
            Assert.AreEqual("Variable not found SF", result);
        }
    }
}

