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
        public class Parameters
        {
            public string MetricCode { get; set; }
            public string Template { get; set; }
        }

        private readonly MetricDefinition _metric;

        public MetricFragment(AutoTextHandlers handlers, Parameters parameters) : base(handlers, parameters.Template)
        {
            _metric = handlers.DefinitionHandler.GetMetricDefinition(parameters.MetricCode);
            if (_metric == null)
            {
                throw new Exception($"Metric not found {parameters.MetricCode}"); // EZSTODO - needs correct exception
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
        public class Parameters
        {
            public string MetricCode { get; set; }
            public string VariableCode { get; set; }
            public AutoTextTemplate Templates { get; set; }
        }

        private readonly VariableData _variableData;
        private readonly MetricDefinition _metric;
        private readonly VariableFragment _variableFragment;
        private readonly string _flatTemplate;

        public DataFragment(AutoTextHandlers handlers, Parameters parameters)
            : base(handlers, parameters.Templates.Data)
        {
            _variableData = _handlers.DataHandler.GetVariableData(parameters.MetricCode, parameters.VariableCode);
            if (_variableData == null)
            {
                throw new Exception($"Metric {parameters.MetricCode} or variable {parameters.VariableCode} not found"); // EZSTODO - needs correct exception
            }

            _metric = handlers.DefinitionHandler.GetMetricDefinition(parameters.MetricCode);
            if (_metric == null)
            {
                throw new Exception($"Metric not found {parameters.MetricCode}"); // EZSTODO - needs correct exception
            }

            _variableFragment = new VariableFragment(handlers, 
                new VariableFragment.Parameters 
                { 
                    VariableCode = parameters.VariableCode, 
                    Template = parameters.Templates.Variable
                });
            _flatTemplate = parameters.Templates.FlatData;
        }

        public override string GetFragment()
        {
            _handlers.DefinitionHandler.UpdateDirectionText(_metric, _variableData.Direction);
            var template = _variableData.Direction == DirectionType.FLAT ? _flatTemplate : _template;
            return ProcessMacros(_variableData, template) + _variableFragment.GetFragment();
        }
    }

    // Handle "for Single Family", "for Single Family and Townhouse/Condos"
    // [ACTUAL NAME/LONGNAME]
    public class VariableFragment : ClauseFragment
    {
        public class Parameters
        {
            public string VariableCode { get; set; }
            public string Template { get; set; }
        }

        private readonly VariableDefinition _variable;

        public VariableFragment(AutoTextHandlers handlers, Parameters parameters)
            : base(handlers, parameters.Template)
        {
            _variable = handlers.DefinitionHandler.GetVariableDefinition(parameters.VariableCode);
            if (_variable == null)
            {
                throw new Exception($"Variable not found {parameters.VariableCode}"); // EZSTODO - needs correct exception
            }
        }

        public override string GetFragment()
        {
            return ProcessMacros(_variable);
        }
    }
}