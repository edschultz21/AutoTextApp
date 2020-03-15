using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GeneratorTests
    {
        private FragmentGenerators _generator;
        private IDefinitionProvider _definitionProvider;
        private IDataProvider _dataProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _definitionProvider = new TestDefinitionProvider();
            _dataProvider = new TestDataProvider();
            _generator = new FragmentGenerators(_definitionProvider, _dataProvider);
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

            Assert.AreEqual(101, variable.Id);
            Assert.AreEqual(82.5F, variable.CurrentValue);
            Assert.AreEqual(92.5F, variable.PreviousValue);
            Assert.AreEqual(0.04F, variable.PercentChange);
            Assert.AreEqual(DirectionType.FLAT, variable.Direction_Old);
        }

        [TestMethod]
        public void MetricFragment()
        {
            var metric = _generator.CreateMetricFragment("NL", "[METRIC CODE]");

            Assert.AreEqual("NL", metric.MetricCode);
            Assert.AreEqual("[METRIC CODE]", metric.Template);
            Assert.AreEqual("New Listings", metric.Metric.ShortName);
        }

        [TestMethod]
        public void VariableFragment()
        {
            var variable = _generator.CreateVariableFragment("SF", "[ACTUAL NAME]");

            Assert.AreEqual("SF", variable.VariableCode);
            Assert.AreEqual("[ACTUAL NAME]", variable.Template);
            Assert.AreEqual("Single Family", variable.Variable.ShortName);
        }

        [TestMethod]
        public void DataFragment()
        {
            var variable = _generator.CreateVariableFragment("SF", "[ACTUAL NAME]");
            var data = _generator.CreateDataFragment(variable, "CS", "SF", "[DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]");

            Assert.AreEqual("SF", data.VariableCode);
            Assert.AreEqual("[ACTUAL NAME]", data.VariableFragment.Template);
            Assert.AreEqual(101, data.VariableData.Id);
            Assert.AreEqual(82.5F, data.VariableData.CurrentValue);
            Assert.AreEqual(92.5F, data.VariableData.PreviousValue);
            Assert.AreEqual(0.04F, data.VariableData.PercentChange);
            Assert.AreEqual(DirectionType.FLAT, data.Direction);
            Assert.AreEqual("[DIR]", data.Template);
        }

        [TestMethod]
        public void DataFragmentNegativeDir()
        {
            var variable = _generator.CreateVariableFragment("SF", "[ACTUAL LONGNAME]");
            var data = _generator.CreateDataFragment(variable, "NL", "SF", "[DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]");

            Assert.AreEqual("SF", data.VariableCode);
            Assert.AreEqual("[ACTUAL LONGNAME]", data.VariableFragment.Template);
            Assert.AreEqual(101, data.VariableData.Id);
            Assert.AreEqual(82.5F, data.VariableData.CurrentValue);
            Assert.AreEqual(92.5F, data.VariableData.PreviousValue);
            Assert.AreEqual(7.5F, data.VariableData.PercentChange);
            Assert.AreEqual(DirectionType.NEGATIVE, data.Direction);
            Assert.AreEqual("[DIR] [PCT] percent to [ACTUAL VALUE]", data.Template);
        }

        [TestMethod]
        public void DataFragmentFlippedDir()
        {
            var data = _generator.CreateDataFragment(null, "MSI", null, "[DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]");

            Assert.AreEqual(-1, data.VariableData.Id);
            Assert.AreEqual(2.4F, data.VariableData.CurrentValue);
            Assert.AreEqual(7.2F, data.VariableData.PreviousValue);
            Assert.AreEqual(12.3F, data.VariableData.PercentChange);
            Assert.AreEqual(DirectionType.POSITIVE, data.Direction);
            Assert.AreEqual("[DIR] [PCT] percent to [ACTUAL VALUE]", data.Template);
        }
    }
}
