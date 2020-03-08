using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AutoTextApp
{
    public class AutoText
    {
        private const string DIR_TEXT = "[DIR]";
        private AutoTextDefinition _definitions;
        private AutoTextData _data;
        private Dictionary<string, MacroVariable> _macroVariables; // Macro -> Value (consider using KeyedCollection)
        private Dictionary<string, int> _metricToId;
        private Dictionary<string, int> _variableToId;

        public AutoText(AutoTextDefinition definitions, AutoTextData data)
        {
            _definitions = definitions;
            _data = data;

            NormalizeCasing();

            _macroVariables = _definitions.MacroVariables?.ToDictionary(x => $"[{x.Name}]", y => y) ?? new Dictionary<string, MacroVariable>();
            _metricToId = _data.Metrics.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            _variableToId = _data.Variables.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            SetDirection();
        }

        private void NormalizeCasing()
        {
            // Definitions
            Array.ForEach(_definitions.Metrics, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_definitions.Variables, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_definitions.MacroVariables, x => x.Name = x.Name.ToUpper());

            // Data
            Array.ForEach(_data.Blocks,
                x => Array.ForEach(x.BlockItems, y =>
                {
                    y.MetricCode = y.MetricCode.ToUpper();
                    y.Variables = Array.ConvertAll(y.Variables, z => z.ToUpper());
                }));
            Array.ForEach(_data.Metrics, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_data.Variables, x => x.Code = x.Code.ToUpper());
        }

        private void SetDirection()
        {
            foreach (var metricData in _data.MetricData)
            {
                foreach (var variableData in metricData.VariableData)
                {
                    variableData.Direction = DirectionType.FLAT;
                    if (variableData.PercentChange >= 0.05)
                    {
                        var isPositive = (variableData.CurrentValue - variableData.PreviousValue) > 0;

                        var metricIdCode = _data.Metrics.FirstOrDefault(x => x.Id == metricData.Id);
                        if (metricIdCode != null)
                        {
                            var metric = GetMetricDefinition(metricIdCode.Code);
                            if (metric != null)
                            {
                                isPositive = metric.IsIncreasePostive ? isPositive : !isPositive;
                            }
                        }

                        variableData.Direction = isPositive ? DirectionType.POSITIVE : DirectionType.NEGATIVE;
                    }
                }
            }
        }

        private MetricDefinition GetMetricDefinition(string code)
        {
            return _definitions.Metrics.FirstOrDefault(x => x.Code == code);
        }

        private VariableDefinition GetVariableDefinition(string variable)
        {
            return _definitions.Variables.FirstOrDefault(x => x.Code == variable);
        }

        private MacroVariable GetMacroVariable(string macroName)
        {
            return _definitions.MacroVariables.FirstOrDefault(x => x.Name == macroName);
        }

        private bool ContainsValue(string text)
        {
            return text.EndsWith("Value");
        }

        // EZSTODO - need to handle case of no variable
        // EZSTODO - need to handle cases of missing metric/variable
        private VariableData GetVariableData(string metricCode, string variableCode)
        {
            if (_metricToId.TryGetValue(metricCode, out int metricId) && _variableToId.TryGetValue(variableCode, out int variableId))
            {
                var metricData = _data.MetricData.FirstOrDefault(x => x.Id == metricId);
                if (metricData != null)
                {
                    return metricData.VariableData.FirstOrDefault(x => x.Id == variableId);
                }
            }

            return null;
        }

        private string GetDirection(DirectionType direction, Random random)
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

        // If we have a direction that is flat, we need to remove some descriptive text that
        // would be there otherwise. Currently we assume that we have a "for" before the variable
        // so that we need to know how much to remove. If this does not work out, we need to be 
        // able to bracket the text that needs to be removed.
        // For example, this:
        // [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME]
        // needs to change to this:
        // [DIR] for [ACTUAL NAME]
        private string FixDirectionText(string text)
        {
            var dirIndex = text.IndexOf(DIR_TEXT, StringComparison.InvariantCultureIgnoreCase);
            // Sanity check
            if (dirIndex != -1)
            {
                var forIndex = text.IndexOf(" for", dirIndex + 1, StringComparison.InvariantCultureIgnoreCase);
                if (forIndex != -1)
                {
                    var temp = text.Substring(0, dirIndex + DIR_TEXT.Length) + text.Substring(forIndex);
                    return temp;
                }
            }

            return text;
        }

        // EZSTODO - how to handle complicated fragments
        // EZSTODO - figure out what we should pass here
        // EZSTODO - private?
        public string GetSentenceFragment(string metricCode, string variableCode, string template, int seed)
        {
            metricCode = metricCode.ToUpper();
            variableCode = variableCode.ToUpper();

            var random = new Random(seed);
            var metric = GetMetricDefinition(metricCode);
            if (metric == null)
            {
                //throw new Exception($"Metric not found {metricCode}"); - EZSTODO
                return $"Metric not found {metricCode}";
            }
            var variableData = GetVariableData(metricCode, variableCode);
            if (variableData == null)
            {
                //throw new Exception($"Metric not found {variableCode}"); - EZSTODO
                return $"Variable not found {variableCode}";
            }

            var result = template;

            var items = Regex.Matches(template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                var itemValue = item.Value.ToUpper();

                switch (itemValue)
                {
                    case DIR_TEXT:
                        if (variableData.Direction == DirectionType.FLAT)
                        {
                            result = FixDirectionText(result);
                        }
                        result = result.Replace(item.Value, GetDirection(variableData.Direction, random));
                        break;
                    default:
                        if (_macroVariables.TryGetValue(itemValue, out MacroVariable macro))
                        {
                            string macroValue = _macroVariables[itemValue]?.Value;

                            if (!string.IsNullOrEmpty(macro.Type))
                            {
                                var reflectedType = Type.GetType($"{GetType().Namespace}.{macro.Type}");
                                var macroVariable = GetMacroVariable(macro.Name);
                                if (macroVariable != null)
                                {
                                    if (metric.GetType().Name == macro.Type)
                                    {
                                        macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(metric).ToString();
                                    }
                                    else if (variableData.GetType().Name == macro.Type)
                                    {
                                        macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(variableData).ToString();
                                        if (!string.IsNullOrEmpty(variableData.DataFormat) && ContainsValue(macroVariable.Value))
                                        {
                                            macroValue = string.Format(variableData.DataFormat, float.Parse(macroValue));
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(variableCode))
                                    {
                                        var variable = GetVariableDefinition(variableCode);
                                        if (variable?.GetType().Name == macro.Type)
                                        {
                                            macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(variable).ToString();
                                        }
                                    }
                                }
                            }

                            if (macroValue != null)
                            {
                                if (!string.IsNullOrEmpty(macro.Format))
                                {
                                    macroValue = string.Format(macro.Format, macroValue);
                                }
                                result = result.Replace(item.Value, macroValue);
                            }
                        }
                        break;
                }
            }

            return result;
        }

        public void Run()
        {
            Random random = new Random(381654729);

            var results = new Dictionary<VariableData, string>();
            foreach (var block in _data.Blocks)
            {
                foreach (var blockItem in block.BlockItems)
                {
                    var seed = random.Next(int.MaxValue);

                    // EZSTODO - sort property values by percent change (largest to smallest)
                    // - need to handle conjunction correctly. Above needs to be sorted by pos/neg change. All positive/negatives are
                    // "and"ed while the two are "or"ed.
                    foreach (var variable in blockItem.Variables)
                    {
                        var variableData = GetVariableData(blockItem.MetricCode, variable);
                        if (variableData != null)
                        {
                            var result = GetSentenceFragment(blockItem.MetricCode, variable, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
                            if (!results.ContainsKey(variableData))
                            {
                                results.Add(variableData, result);
                            }
                        }
                    }
                }
            }
        }
    }
}
