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

        [TestMethod]
        public void TestMethod1()
        {
            // Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
            var definitions = Utils.ReadXmlData<AutoTextDefinition>("TestDefinitions.xml");
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText("Test1_Data.json"), typeof(AutoTextData));
            var autoText = new AutoText(definitions, data);

            var result = autoText.GetSentenceFragment("NL", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("New Listings were up 7.5% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
            var definitions = Utils.ReadXmlData<AutoTextDefinition>("TestDefinitions.xml");
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText("Test1_Data.json"), typeof(AutoTextData));
            var autoText = new AutoText(definitions, data);

            var result = autoText.GetSentenceFragment("CS", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Closed Sales consistent with 0.04% percent to 82.5 for Single Family homes", result);
        }

        [TestMethod]
        public void TestMethod3()
        {
            // Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
            var definitions = Utils.ReadXmlData<AutoTextDefinition>("TestDefinitions.xml");
            var data = (AutoTextData)JsonConvert.DeserializeObject(File.ReadAllText("Test1_Data.json"), typeof(AutoTextData));
            var autoText = new AutoText(definitions, data);

            var result = autoText.GetSentenceFragment("MSP", "SF", "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", _seed);
            Assert.AreEqual("Variable not found SF", result);
        }
    }
}

