using System;
using System.Linq;
using System.Text.RegularExpressions;

// Syntax:
// [MetricFragment] [MetricLocationFragment]? [ChangeFragment] [VariableFragment]?
namespace AutoTextApp
{
    public interface IClauseFragment
    {
        string GetFragment();
    }

    public interface ISentenceFragment
    {

    }

    public abstract class ClauseFragment : IClauseFragment
    {
        protected string _template { get; set; }
        protected AutoTextHandlers _handlers { get; set; }

        protected ClauseFragment(AutoTextHandlers handlers, string template)
        {
            _handlers = handlers;
            _template = template;
        }

        public abstract string GetFragment();
    }

    // Handle "New Listings", "New Listings and Closed Sales"
    // [METRIC NAME/LONGNAME/CODE]
    public class MetricFragment : ClauseFragment
    {
        private readonly MetricDefinition _metric;

        public MetricFragment(AutoTextHandlers handlers, string metricCode, string template) : base(handlers, template)
        {
            _metric = handlers.DefinitionHandler.GetMetricDefinition(metricCode);
            if (_metric == null)
            {
                throw new Exception($"Metric not found {metricCode}");
            }

            _template = template;
        }

        public override string GetFragment()
        {
            var result = _template;
            var items = Regex.Matches(_template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                var itemValue = item.Value.ToUpper();
                var macroVariable = _handlers.DefinitionHandler.GetMacroVariable(itemValue);
                if (macroVariable != null)
                {
                    string macroValue = macroVariable.Value;

                    if (!string.IsNullOrEmpty(macroVariable.Type))
                    {
                        var reflectedType = Type.GetType($"{GetType().Namespace}.{macroVariable.Type}");
                        if (_metric.GetType().Name == macroVariable.Type)
                        {
                            macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(_metric).ToString();
                        }
                    }

                    if (macroValue != null)
                    {
                        if (!string.IsNullOrEmpty(macroVariable.Format))
                        {
                            macroValue = string.Format(macroVariable.Format, macroValue);
                        }
                        result = result.Replace(item.Value, macroValue);
                    }
                }
            }

            return result;
        }
    }

#if NOTYET
    // Handle "", "in Franklin, Hamilton and Saint Lawrence Counties"
    // [METRIC LOCATIONS]
    public class MetricLocationFragment : ClauseFragment
    {
        public override string GetFragment()
        {
            throw new NotImplementedException();
        }
    }

    // Handle "increased 1.1%", "stayed the same", "decreased 13.9 percent to $209,000"
    // Where  [PERCENT] -("%", " percent")
    // [DIR] [PCT] [ACTUAL/PREVIOUS VALUE]
    public class DataFragment : ClauseFragment
    {
        public bool ShowShortForm { get; set; } // Useful for direction (set external to this class)

        public override string GetFragment()
        {
            throw new NotImplementedException();
        }
    }

    // Handle "for Single Family", "for Single Family and Townhouse/Condos"
    // [ACTUAL NAME/LONGNAME]
    public class VariableFragment : ClauseFragment
    {
        public override string GetFragment()
        {
            throw new NotImplementedException();
        }
    }
#endif

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