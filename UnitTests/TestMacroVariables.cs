using AutoTextApp;
using System.Collections.Generic;

namespace UnitTests
{
    public class TestMacroVariables : IMacroVariables
    {
        public IMacroVariables MacroVariables { get; set; }

        // EZSTOOD - write unit tests for this
        public TestMacroVariables()
        {
            MacroVariables = new MacroVariables();

            var macroList = new List<MacroVariable>()
            {
                new MacroVariable { Name = "HOMES", Value = " homes" },
                new MacroVariable { Name = "METRIC CODE", Value ="Code", Type = "MetricFragment"},
                new MacroVariable { Name = "METRIC NAME", Value ="ShortName", Type = "MetricFragment"},
                new MacroVariable { Name = "METRIC LONGNAME", Value ="LongName", Type = "MetricFragment"},
                new MacroVariable { Name = "ACTUAL VALUE", Value ="CurrentValue", Type = "DataFragment"},
                new MacroVariable { Name = "PREVIOUS VALUE", Value ="PreviousValue", Type = "DataFragment"},
                new MacroVariable { Name = "DIR", Value ="DirectionText", Type = "DataFragment"},
                new MacroVariable { Name = "PCT", Value ="PercentChange", Type = "DataFragment"},
                new MacroVariable { Name = "ACTUAL NAME", Value ="ShortName", Type = "VariableFragment"},
                new MacroVariable { Name = "ACTUAL LONGNAME", Value ="LongName", Type = "VariableFragment"}
            };
            MacroVariables.Add(macroList);
        }

        public void Add(IEnumerable<MacroVariable> macros)
        {
            MacroVariables.Add(macros);
        }

        public void AddOrUpdate(MacroVariable macro)
        {
            MacroVariables.AddOrUpdate(macro);
        }

        public MacroVariable Get(string name)
        {
            return MacroVariables.Get(name);
        }
    }
}
