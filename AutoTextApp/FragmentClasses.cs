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
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }

            _template = template;
        }

        public override string GetFragment()
        {
            return AutoTextUtils.ProcessMacros(_metric, _template, _handlers.DefinitionHandler.GetMacroVariable);
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

#if NOTYET
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
#endif

    // Handle "for Single Family", "for Single Family and Townhouse/Condos"
    // [ACTUAL NAME/LONGNAME]
    public class VariableFragment : ClauseFragment
    {
        private readonly VariableData _variableData;
        private readonly MetricDefinition _metric;
        private readonly string _flatTemplate;

        public VariableFragment(AutoTextHandlers handlers, string metricCode, string variableCode, string template, string flatTemplate) 
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
            return AutoTextUtils.ProcessMacros(_variableData, template, _handlers.DefinitionHandler.GetMacroVariable);
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