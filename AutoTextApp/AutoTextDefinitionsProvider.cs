using System;
using System.Linq;

namespace AutoTextApp
{
    public interface IDefinitionProvider
    {
        MetricDefinition GetMetricDefinition(string metricCode);

        VariableDefinition GetVariableDefinition(string variableCode);

        Templates Templates { get; }
    }

    public class AutoTextDefinitionsProvider : IDefinitionProvider
    {
        private AutoTextDefinitions _definitions;

        public Templates Templates { get { return _definitions.Templates; } }

        public AutoTextDefinitionsProvider(AutoTextDefinitions definitions)
        {
            _definitions = definitions;

            // Normalize casing
            Array.ForEach(_definitions.Metrics, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_definitions.Variables, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_definitions.MacroVariables, x => x.Name = x.Name.ToUpper());
        }

        public MetricDefinition GetMetricDefinition(string code)
        {
            return _definitions.Metrics.FirstOrDefault(x => x.Code == code);
        }

        public VariableDefinition GetVariableDefinition(string variable)
        {
            return _definitions.Variables.FirstOrDefault(x => x.Code == variable);
        }
    }
}
