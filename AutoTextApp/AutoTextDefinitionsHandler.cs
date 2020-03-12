using System;
using System.Linq;

namespace AutoTextApp
{
    public class AutoTextDefinitionsHandler
    {
        private const string DIR_TEXT = "DIR";

        private AutoTextDefinitions _definitions;
        private MacroVariableKeyedDictionary _macroVariables; // Macro -> Value
        private Random _random = new Random(381654729);

        public AutoTextDefinitionsHandler(AutoTextDefinitions definitions)
        {
            _definitions = definitions;

            // Normalize casing
            Array.ForEach(_definitions.Metrics, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_definitions.Variables, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_definitions.MacroVariables, x => x.Name = x.Name.ToUpper());

            // Setup macro variables
            _macroVariables = new MacroVariableKeyedDictionary();
            _macroVariables.Add(new MacroVariable { Name = DIR_TEXT, Value = "" });
            if (_definitions.MacroVariables != null)
            {
                Array.ForEach(_definitions.MacroVariables, x => _macroVariables.Add(x));
            }
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
            return _macroVariables.FirstOrDefault(x => x.Name == macroName.TrimStart('[').TrimEnd(']'));
        }

        private DirectionType GetDirection(MetricDefinition metric, DirectionType direction)
        {
            if (direction != DirectionType.FLAT && !metric.IsIncreasePostive)
            {
                if (direction == DirectionType.NEGATIVE)
                {
                    return DirectionType.POSITIVE;
                }
                return DirectionType.NEGATIVE;
            }

            return direction;
        }

        public void UpdateDirectionText(MetricDefinition metric, DirectionType direction)
        {
            var directionText = "";

            direction = GetDirection(metric, direction);
            if (direction == DirectionType.FLAT)
            {
                var index = _random.Next(_definitions.Synonyms.Flat.Length);
                directionText = _definitions.Synonyms.Flat[index];
            }
            else if (direction == DirectionType.POSITIVE)
            {
                var index = _random.Next(_definitions.Synonyms.Positive.Length);
                directionText = _definitions.Synonyms.Positive[index];
            }
            else // direction == DirectionType.NEGATIVE
            {
                var index = _random.Next(_definitions.Synonyms.Negative.Length);
                directionText = _definitions.Synonyms.Negative[index];
            }

            _macroVariables[DIR_TEXT].Value = directionText;
        }

        // EZSTODO - Remove
        public string GetDirectionText(MetricDefinition metric, DirectionType direction, Random random)
        {
            direction = GetDirection(metric, direction);
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
