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
        private AutoTextDefinition _definitions;
        private AutoTextData _data;
        // EZSTODO - use collection dictionary
        private Dictionary<string, MacroVariable> _macroVariables; // Macro -> Value
        private Dictionary<string, int> _metricToId;
        private Dictionary<string, int> _variableToId;

        // EZSTODO - need to ensure all codes are in UPPER case
        public AutoText(AutoTextDefinition definitions, AutoTextData data1)
        {
            _definitions = definitions;
            _data = data1;

            _macroVariables = _definitions.MacroVariables?.ToDictionary(x => $"[{x.Name}]", y => y) ?? new Dictionary<string, MacroVariable>();
            _metricToId = _data.Metrics.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            _variableToId = _data.Variables.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            SetDirection();
        }

        // EZSTODO - what if missing?
        private string GetVariableName(string code)
        {
            return _definitions.Variables.FirstOrDefault(x => x.Code == code).ShortName;
        }

        private string GetVariableLongName(string code)
        {
            return _definitions.Variables.FirstOrDefault(x => x.Code == code).LongName;
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
                            var metric = _definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == metricIdCode.Code.ToUpper());
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

        // EZSTODO - consider passing in variable code instead of MetricData
        // EZSTODO - how to handle complicated fragments
        // EZSTODO - figure out what we should pass here
        // EZSTODO - private?
        public string GetSentenceFragment(string metricCode, string variableCode, string template, int seed)
        {
            var random = new Random(seed);
            var metric = _definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == metricCode.ToUpper());
            if (metric == null)
            {
                //throw new Exception($"Metric not found {data.Name}"); - EZSTODO
                return $"Metric not found {metricCode}";
            }
            var variableData = GetVariableData(metricCode, variableCode);
            if (variableData == null)
            {
                //throw new Exception($"Metric not found {data.Name}"); - EZSTODO
                return $"Variable not found {variableCode}";
            }

            var result = template;

            var items = Regex.Matches(template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                switch (item.Value.ToUpperInvariant())
                {
                    case "[DIR]":
                        result = result.Replace(item.Value, GetDirection(variableData.Direction, random));
                        break;
                    default:
                        if (_macroVariables.TryGetValue(item.Value, out MacroVariable macro))
                        {
                            string macroValue = _macroVariables[item.Value]?.Value;

                            if (!string.IsNullOrEmpty(macro.Type))
                            {
                                var reflectedType = Type.GetType($"{GetType().Namespace}.{macro.Type}");
                                var macroVariable = _definitions.MacroVariables.FirstOrDefault(x => x.Name.ToUpper() == macro.Name.ToUpper());
                                if (metric.GetType().Name == macro.Type)
                                {
                                    macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(metric).ToString();
                                }
                                else if (variableData.GetType().Name == macro.Type)
                                {
                                    macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(variableData).ToString();
                                }
                                else if (!string.IsNullOrEmpty(variableCode))
                                {
                                    var variable = _definitions.Variables.FirstOrDefault(x => x.Code == variableCode);
                                    if (variable.GetType().Name == macro.Type)
                                    {
                                        macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(variable).ToString();
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
