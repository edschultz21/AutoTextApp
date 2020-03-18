using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GeneratorTests
    {
        private IFragmentGenerators _generator;
        private IDefinitionProvider _definitionProvider;
        private IDataProvider _dataProvider;
        private IMacroVariables _macroVariables;

        [TestInitialize]
        public void TestInitialize()
        {
            _definitionProvider = new TestDefinitionProvider();
            _dataProvider = new TestDataProvider();
            _generator = new FragmentGenerators(_definitionProvider, _dataProvider);
            _macroVariables = new TestMacroVariables();
        }

        private void ValidateDataFragment(VariableData variable, int id, float current, float previous, float percent, DirectionType direction)
        {
            Assert.AreEqual(id, variable.Id);
            Assert.AreEqual(current, variable.CurrentValue);
            Assert.AreEqual(previous, variable.PreviousValue);
            Assert.AreEqual(percent, variable.PercentChange);
            Assert.AreEqual(direction, variable.Direction);
        }

        [TestMethod]
        public void MetricDefinition()
        {
            var metric = _definitionProvider.GetMetricDefinition("nl");

            Assert.AreEqual("NL", metric.Code);
            Assert.AreEqual("New Listings", metric.ShortName);
        }

        [TestMethod]
        public void VariableDefinition()
        {
            var variable = _definitionProvider.GetVariableDefinition("SF");

            Assert.AreEqual("SF", variable.Code);
            Assert.AreEqual("Single Family", variable.ShortName);
        }

        [TestMethod]
        public void VariableData()
        {
            var variable = _dataProvider.GetVariableData("CS", "SF");

            ValidateDataFragment(variable, 101, 82.5F, 92.5F, 0.04F, DirectionType.FLAT);
        }

        [TestMethod]
        public void MetricFragment()
        {
            var metric = _generator.CreateMetricFragment("NL");

            Assert.AreEqual("NL", metric.MetricCode);
            Assert.AreEqual("New Listings", metric.Metric.ShortName);
        }

        [TestMethod]
        public void VariableFragment()
        {
            var variable = _generator.CreateVariableFragment("SF");

            Assert.AreEqual("SF", variable.VariableCode);
            Assert.AreEqual("Single Family", variable.Variable.ShortName);
        }

        [TestMethod]
        public void DataFragment()
        {
            var variable = _generator.CreateVariableFragment("SF");
            var data = _generator.CreateDataFragment(variable, "CS", "SF");

            Assert.AreEqual("SF", data.VariableCode);
            ValidateDataFragment(data.VariableData, 101, 82.5F, 92.5F, 0.04F, DirectionType.FLAT);
        }

        [TestMethod]
        public void DataFragmentNegativeDir()
        {
            var variable = _generator.CreateVariableFragment("SF");
            var data = _generator.CreateDataFragment(variable, "NL", "SF");

            Assert.AreEqual("SF", data.VariableCode);
            ValidateDataFragment(data.VariableData, 101, 82.5F, 92.5F, 7.5F, DirectionType.NEGATIVE);
        }

        [TestMethod]
        public void DataFragmentFlippedDir()
        {
            var data = _generator.CreateDataFragment(null, "MSI", null);

            Assert.AreEqual(DirectionType.POSITIVE, data.Direction);
            ValidateDataFragment(data.VariableData, -1, 2.4F, 7.2F, 12.3F, DirectionType.NEGATIVE);
        }

        [TestMethod]
        public void SentenceFragment1()
        {
            var sentence = _generator.CreateSentenceFragment("MSP", new string[] { "SF", "TC" });

            Assert.AreEqual("MSP", sentence.MetricFragment.MetricCode);

            var dataFragments = sentence.DataFragments;
            Assert.AreEqual(2, dataFragments.Count);

            var data = sentence.DataFragments[0];
            Assert.AreEqual("Single Family", data.VariableFragment.Variable.ShortName);
            ValidateDataFragment(data.VariableData, 101, 82.5F, 92.5F, 0.04F, DirectionType.FLAT);

            data = sentence.DataFragments[1];
            Assert.AreEqual("Townhouse/Condo homes", data.VariableFragment.Variable.LongName);
            ValidateDataFragment(data.VariableData, 102, 209000F, 200000F, 13.9F, DirectionType.POSITIVE);
        }

        // EZSTODO - find a home for this
        [TestMethod]
        public void MacroVariables1()
        {
            Assert.AreEqual(" homes", _macroVariables.Get("HOMES").Value);
            Assert.AreEqual("PreviousValue", _macroVariables.Get("PREVIOUS VALUE").Value);
            Assert.AreEqual(null, _macroVariables.Get("NOT EXISTING"));
        }
    }
}