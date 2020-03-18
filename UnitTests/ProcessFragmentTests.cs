using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ProcessFragmentTests
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

        //[TestMethod]
        //public void ProcessTemplate_Metric()
        //{
        //    var metric = _generator.CreateMetricFragment("NL", "[METRIC NAME]");

        //    var result = MacroVariableProcessor.ProcessMacros(_macroVariables, metric);
        //    Assert.AreEqual("New Listings", result);
        //}

        //[TestMethod]
        //public void ProcessTemplate_MetricDir1()
        //{
        //    var metric = _generator.CreateMetricFragment("NL", "[METRIC NAME][DIR]");

        //    var result = MacroVariableProcessor.ProcessMacros(_macroVariables, metric);
        //    Assert.AreEqual("New Listings[DIR]", result);
        //}

        //[TestMethod]
        //public void ProcessTemplate_MetricDir2()
        //{
        //    var metric = _generator.CreateMetricFragment("NL", "[METRIC NAME][DIR]");

        //    _macroVariables.AddOrUpdate(new MacroVariable { Name = "DIR", Value = " went up" });
        //    var result = MacroVariableProcessor.ProcessMacros(_macroVariables, metric);
        //    Assert.AreEqual("New Listings went up", result);
        //}

        //[TestMethod]
        //public void ProcessTemplate_MetricFull()
        //{
        //    var template = "[METRIC NAME] [DIR][PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME]";
        //    var metric = _generator.CreateMetricFragment("NL", "[METRIC NAME]");

        //    var result = MacroVariableProcessor.ProcessMacros(_macroVariables, metric, template);
        //    Assert.AreEqual("New Listings [DIR][PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME]", result);
        //}

        //[TestMethod]
        //public void ProcessTemplate_MetricAndVariable()
        //{
        //    var template = "[METRIC NAME] [DIR][PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME]";
        //    var variable = _generator.CreateVariableFragment("SF", "[ACTUAL NAME]");
        //    var metric = _generator.CreateMetricFragment("NL", "[METRIC NAME]");

        //    var result1 = MacroVariableProcessor.ProcessMacros(_macroVariables, metric, template);
        //    var result2 = MacroVariableProcessor.ProcessMacros(_macroVariables, variable, result1);

        //    Assert.AreEqual("New Listings [DIR][PCT] percent to [ACTUAL VALUE] for Single Family", result2);
        //}

        //[TestMethod]
        //public void ProcessTemplate_All()
        //{
        //    var template = "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME]";
        //    var variable = _generator.CreateVariableFragment("SF", "[ACTUAL LONGNAME]");
        //    var data = _generator.CreateDataFragment(variable, "NL", "SF", "[DIR] [PCT] percent to [ACTUAL VALUE]", "[DIR]");
        //    var metric = _generator.CreateMetricFragment("NL", "[METRIC NAME]");

        //    var result1 = MacroVariableProcessor.ProcessMacros(_macroVariables, metric, template);
        //    var result2 = MacroVariableProcessor.ProcessMacros(_macroVariables, variable, result1);
        //    var result3 = MacroVariableProcessor.ProcessMacros(_macroVariables, data, result2);

        //    Assert.AreEqual("New Listings were down 7.5 percent to 82.5 for Single Family", result3);
        //}
    }
}
