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

        protected string ProcessMacros(object data)
        {
            return ProcessMacros(data, _template);
        }

        protected string ProcessMacros(object data, string template)
        {
            var result = template;
            var items = Regex.Matches(template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                var itemValue = item.Value.ToUpper();
                var macroVariable = _handlers.DefinitionHandler.GetMacroVariable(itemValue);
                if (macroVariable != null)
                {
                    string macroValue = null;

                    if (!string.IsNullOrEmpty(macroVariable.Type))
                    {
                        var reflectedType = Type.GetType($"{data.GetType().Namespace}.{macroVariable.Type}");
                        if (data.GetType().Name == macroVariable.Type)
                        {
                            macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(data).ToString();
                        }
                    }
                    else
                    {
                        macroValue = macroVariable.Value;
                    }

                    if (macroValue != null)
                    {
                        result = result.Replace(item.Value, macroValue);
                    }
                }
            }

            return result;
        }
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
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }
        }

        public override string GetFragment()
        {
            return ProcessMacros(_metric);
        }
    }

    // Handle "", "in Franklin, Hamilton and Saint Lawrence Counties"
    // [METRIC LOCATIONS]
    public class MetricLocationFragment : ClauseFragment
    {
        public MetricLocationFragment(AutoTextHandlers handlers, string template) : base(handlers, template) { }

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
        private readonly VariableData _variableData;
        private readonly MetricDefinition _metric;
        private readonly string _flatTemplate;

        public DataFragment(AutoTextHandlers handlers, string metricCode, string variableCode, string template, string flatTemplate) 
            : base(handlers, template) 
        {
            _variableData = _handlers.DataHandler.GetVariableData(metricCode, variableCode);
            if (_variableData == null)
            {
                throw new Exception($"Metric {metricCode} or variable {variableCode} not found"); // EZSTODO - needs correct exception
            }

            _metric = handlers.DefinitionHandler.GetMetricDefinition(metricCode);
            if (_metric == null)
            {
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }

            _flatTemplate = flatTemplate;
        }

        public override string GetFragment()
        {
            _handlers.DefinitionHandler.UpdateDirectionText(_metric, _variableData.Direction);
            var template = _variableData.Direction == DirectionType.FLAT ? _flatTemplate : _template;
            return ProcessMacros(_variableData, template);
        }
    }

    // Handle "for Single Family", "for Single Family and Townhouse/Condos"
    // [ACTUAL NAME/LONGNAME]
    public class VariableFragment : ClauseFragment
    {
        private readonly VariableDefinition _variable;

        public VariableFragment(AutoTextHandlers handlers, string variableCode, string template)
            : base(handlers, template)
        {
            _variable = handlers.DefinitionHandler.GetVariableDefinition(variableCode);
            if (_variable == null)
            {
                throw new Exception($"Variable not found {variableCode}"); // EZSTODO - needs correct exception
            }
        }

        public override string GetFragment()
        {
            return ProcessMacros(_variable);
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