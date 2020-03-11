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
            var definitions = Utils.ReadXmlData<AutoTextDefinitions>(definitionsFilename);
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText(testDataFilename), typeof(AutoTextData));
            return new AutoText(definitions, data);
        }

        [TestMethod]
        public void FragmentOneValue1()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("NL", "SF", "[METRIC NAME] [DIR] [PCT] to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("New Listings were up 7.5% to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void FragmentOneValue2()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("CS", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Closed Sales were about the same for Single Family homes", result);
        }

        [TestMethod]
        public void FragmentOneValue3()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("MSP", "DT", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Variable not found DT", result);
        }

        [TestMethod]
        public void FragmentTwoValues1()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("MSP", "TC", "[METRIC NAME] [DIR] [PCT] to [ACTUAL VALUE] for [ACTUAL LONGNAME]", _seed);
            Assert.AreEqual("Median Sales Price were down 13.9% to $209,000 for Townhouse/Condo homes", result);
        }

        [TestMethod]
        public void FragmentCasing1()
        {
            var autoText = GetAutoText("DefinitionsMixedCasing.xml", "DataMixedCasing.json");

            var result = autoText.GetSentenceFragment("nl", "SF", "[METRIC NAME] [dir] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMes]", _seed);
            Assert.AreEqual("New Listings were up 7.5% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void FragmentCasing2()
        {
            var autoText = GetAutoText("DefinitionsMixedCasing.xml", "DataMixedCasing.json");

            var result = autoText.GetSentenceFragment("CS", "sf", "[METRIC name] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Closed Sales were about the same for Single Family homes", result);
        }

        [TestMethod]
        public void FragmentCasing3()
        {
            var autoText = GetAutoText("DefinitionsMixedCasing.xml", "DataMixedCasing.json");

            var result = autoText.GetSentenceFragment("msp", "sf", "[METRIC NAME] [DIR] [PCT] percent to [Actual VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Variable not found SF", result);
        }

        [TestMethod]
        public void VariableLongName()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("MSP", "SF", "[METRIC NAME] has a long name of '[ACTUAL LONGNAME]'", _seed);
            Assert.AreEqual("Median Sales Price has a long name of 'Single Family homes'", result);
        }

        [TestMethod]
        public void AllVariables()
        {
            var autoText = GetAutoText("Definitions1.xml", "Data1.json");

            var result = autoText.GetSentenceFragment("MSP", "SF", "[METRIC CODE], [METRIC NAME], [METRIC LONGNAME], [ACTUAL VALUE], [PREVIOUS VALUE], [PCT], [DIR], [ACTUAL NAME], [ACTUAL LONGNAME], [HOMES], [MATCHES NONE]", _seed);
            Assert.AreEqual("MSP, Median Sales Price, MSP LongName, 82.5, 92.5, 0.04%, were about the same, Single Family, Single Family homes,  homes, [MATCHES NONE]", result);
        }
    }
}

