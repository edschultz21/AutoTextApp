using System;
using System.Linq;

// Syntax:
// [MetricFragment] [MetricLocationFragment]? [ChangeFragment] [VariableFragment]?
namespace AutoTextApp
{
    public interface IClauseFragment
    {
        string Template { get; set; }
        string GetFragment(MacroVariableKeyedDictionary macroVariables);
    }

    public interface ISentenceFragment
    {

    }

    public abstract class ClauseFragment : IClauseFragment
    {
        public string Template { get; set; }

        public abstract string GetFragment(MacroVariableKeyedDictionary macroVariables);
    }

    public class MetricFragment : ClauseFragment
    {
        public MetricFragment(string template)
        {
            Template = template;
        }

        // Handle "New Listings", "New Listings and Closed Sales"
        // [METRIC NAME/LONGNAME/CODE]
        public bool ShowShortForm { get; set; } // Useful for direction (set external to this class)

        public override string GetFragment(MacroVariableKeyedDictionary macroVariables)
        {
            throw new NotImplementedException();
        }
    }

    public class MetricLocationFragment : ClauseFragment
    {
        // Handle "", "in Franklin, Hamilton and Saint Lawrence Counties"
        // [METRIC LOCATIONS]

        public override string GetFragment(MacroVariableKeyedDictionary macroVariables)
        {
            throw new NotImplementedException();
        }
    }

    public class DataFragment : ClauseFragment
    {
        // Handle "increased 1.1%", "stayed the same", "decreased 13.9 percent to $209,000"
        // Where  [PERCENT] -("%", " percent")
        // [DIR] [PCT] [ACTUAL/PREVIOUS VALUE]

        public override string GetFragment(MacroVariableKeyedDictionary macroVariables)
        {
            throw new NotImplementedException();
        }
    }

    public class VariableFragment : ClauseFragment
    {
        // Handle "for Single Family", "for Single Family and Townhouse/Condos"
        // [ACTUAL NAME/LONGNAME]

        public override string GetFragment(MacroVariableKeyedDictionary macroVariables)
        {
            throw new NotImplementedException();
        }
    }

    public class TemplateFragment : ISentenceFragment
    {
        // Takes template, fragment objects and creates sentence fragment
        // Create sentence fragments here
    }

    public class StandardFragment : ISentenceFragment
    {
        // Takes template, fragment objects and creates sentence fragment
        // Create sentence fragments here
    }
}