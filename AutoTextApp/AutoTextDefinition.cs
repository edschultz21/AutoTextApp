using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public class AutoTextDefinition
    {
        private AutoTextDefinitions _definitions;
        private MacroVariableKeyedDictionary _macroVariables; // Macro -> Value

        public AutoTextDefinition(AutoTextDefinitions definitions)
        {
            _definitions = definitions;

            // Setup macro variables
            _macroVariables = new MacroVariableKeyedDictionary();
            if (_definitions.MacroVariables != null)
            {
                Array.ForEach(_definitions.MacroVariables, x => _macroVariables.Add(x));
            }

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

        public MacroVariable GetMacroVariable(string macroName)
        {
            return _definitions.MacroVariables.FirstOrDefault(x => x.Name == macroName.TrimStart('[').TrimEnd(']'));
        }

        public string GetDirection(DirectionType direction, Random random)
        {
            if (direction == DirectionType.FLAT)
            {
                var index = random.Next(_definitions.Synonyms.Flat.Length);
                return _definitions.Synonyms.Flat[index];
            }
            else if (direction == DirectionType.POSITIVE)
            {
                var index = random.Next(_definitions.Synonyms.Positive.Length);
                return _definitions.Synonyms.Positive[index];
            }
            else // direction == DirectionType.NEGATIVE
            {
                var index = random.Next(_definitions.Synonyms.Negative.Length);
                return _definitions.Synonyms.Negative[index];
            }
        }
    }
}
