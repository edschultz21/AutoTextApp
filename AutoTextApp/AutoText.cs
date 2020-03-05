using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;


namespace AutoTextApp
{
    public class AutoText
    {
        private AutoTextDefinition _definitions;
        private AutoTextData _data;
        private AutoTextData1 _data1;
        private Dictionary<string, string> _macroVariables; // Macro -> Value
        private Dictionary<string, int> _metricToId;
        private Dictionary<string, int> _variableToId;

        // EZSTODO - need to ensure all codes are in UPPER case
        public AutoText(AutoTextDefinition definitions, AutoTextData data, AutoTextData1 data1)
        {
            _definitions = definitions;
            _data = data;
            _data1 = data1;

            _macroVariables = _definitions.MacroVariables?.ToDictionary(x => $"[{x.Name}]", y => string.IsNullOrEmpty(y?.Value) ? "" : y.Value) ?? new Dictionary<string, string>();
            _metricToId = _data1.Metrics.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            _variableToId = _data1.Variables.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            SetDirection();
        }

        // EZSTODO - short/long name
        // EZSTODO - what if missing?
        private string GetVariableName(string code)
        {
            return _definitions.Variables.FirstOrDefault(x => x.Code == code).ShortName;
        }

        // EZSTODO - need to handle case of no variable
        // EZSTODO - need to handle cases of missing metric/variable
        private VariableData GetVariableData(string metricCode, string variableCode)
        {
            if (_metricToId.TryGetValue(metricCode, out int metricId) && _variableToId.TryGetValue(variableCode, out int variableId))
            {
                var metricData = _data1.MetricData.FirstOrDefault(x => x.Id == metricId);
                if (metricData != null)
                {
                    return metricData.VariableData.FirstOrDefault(x => x.Id == variableId);
                }
            }

            return null;
        }

        private void SetDirection()
        {
            foreach (var paragraph in _data.Paragraphs)
            {
                foreach (var sentence in paragraph.Sentences)
                {
                    foreach (var propertyValue in sentence.PropertyValues)
                    {
                        propertyValue.Direction = DirectionType.FLAT;
                        if (propertyValue.PercentChange >= 0.05)
                        {
                            var isPositive = (propertyValue.CurrentValue - propertyValue.PreviousValue) > 0;

                            var metric = _definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == sentence.Code.ToUpper());
                            if (metric != null)
                            {
                                isPositive = metric.IsIncreasePostive ? isPositive : !isPositive;
                            }
                            
                            propertyValue.Direction = isPositive ? DirectionType.POSITIVE : DirectionType.NEGATIVE;
                        }
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

        private string GetSentenceFragment(string metricCode, MetricData_Org metricDataOrg, string template, int seed)
        {
            var random = new Random(seed);
            var metric = _definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == metricCode.ToUpper());
            if (metric == null)
            {
                //throw new Exception($"Metric not found {data.Name}"); - EZSTODO
                return $"Metric not found {metricDataOrg.Name}";
            }

            var result = template;

            var items = Regex.Matches(template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                switch (item.Value.ToUpperInvariant())
                {
                    case "[METRIC CODE]":
                        result = result.Replace(item.Value, metric.Code);
                        break;
                    case "[METRIC NAME]":
                        result = result.Replace(item.Value, metric.ShortName);
                        break;
                    case "[METRIC LONGNAME]":
                        result = result.Replace(item.Value, metric.LongName);
                        break;
                    case "[ACTUAL VALUE]":
                        result = result.Replace(item.Value, metricDataOrg.CurrentValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[PREVIOUS VALUE]":
                        result = result.Replace(item.Value, metricDataOrg.PreviousValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[ACTUAL NAME]":
                        result = result.Replace(item.Value, metricDataOrg.Name);
                        break;
                    case "[PCT]":
                        result = result.Replace(item.Value, $"{metricDataOrg.PercentChange}%");
                        break;
                    case "[DIR]":
                        result = result.Replace(item.Value, GetDirection(metricDataOrg.Direction, random));
                        break;
                    default:
                        if (_macroVariables.ContainsKey(item.Value))
                        {
                            result = result.Replace(item.Value, _macroVariables[item.Value]);
                        }
                        break;
                }
            }

            return result;
        }

        // EZSTODO - consider passing in variable code instead of MetricData
        // EZSTODO - how to handle complicated fragments
        // EZSTODO - figure out what we should pass here
        private string GetSentenceFragment1(string metricCode, string variableCode, string template, int seed)
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
                    case "[METRIC CODE]":
                        result = result.Replace(item.Value, metric.Code);
                        break;
                    case "[METRIC NAME]":
                        result = result.Replace(item.Value, metric.ShortName);
                        break;
                    case "[METRIC LONGNAME]":
                        result = result.Replace(item.Value, metric.LongName);
                        break;
                    case "[ACTUAL VALUE]":
                        result = result.Replace(item.Value, variableData.CurrentValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[PREVIOUS VALUE]":
                        result = result.Replace(item.Value, variableData.PreviousValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[ACTUAL NAME]":
                        result = result.Replace(item.Value, GetVariableName(variableCode));
                        break;
                    case "[PCT]":
                        result = result.Replace(item.Value, $"{variableData.PercentChange}%");
                        break;
                    case "[DIR]":
                        result = result.Replace(item.Value, GetDirection(variableData.Direction, random));
                        break;
                    default:
                        if (_macroVariables.ContainsKey(item.Value))
                        {
                            result = result.Replace(item.Value, _macroVariables[item.Value]);
                        }
                        break;
                }
            }

            return result;
        }

        public void Run()
        {
            Random random = new Random(381654729);

            var ezs4 = File.ReadAllText("CRMLS_Data.json");
            var ezs3 = (AutoTextData1)JsonConvert.DeserializeObject(ezs4, typeof(AutoTextData1));

            var results1 = new Dictionary<VariableData, string>();
            foreach (var block in _data1.Blocks)
            {
                foreach (var blockItem in block.BlockItems)
                {
                    var seed = random.Next(int.MaxValue);

                    foreach (var variable in blockItem.Variables)
                    {
                        var variableData = GetVariableData(blockItem.MetricCode, variable);
                        if (variableData != null)
                        {
                            var result = GetSentenceFragment1(blockItem.MetricCode, variable, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
                            if (!results1.ContainsKey(variableData))
                            {
                                results1.Add(variableData, result);
                            }
                        }
                    }
                    // EZSTODO - sort property values by percent change (largest to smallest)
                    // - need to handle conjunction correctly. Above needs to be sorted by pos/neg change. All positive/negatives are
                    // "and"ed while the two are "or"ed.
                    //foreach (var propertyValue in sentence.PropertyValues)
                    //{
                    //    var result = GetSentenceFragment(blockItem.MetricCode, propertyValue, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
                    //    results.Add(propertyValue, result);
                    //}
                }
            }



            var results = new Dictionary<MetricData_Org, string>();
            foreach (var paragraph in _data.Paragraphs)
            {
                // EZSTODO - sort sentences by percent change (largest to smallest)
                foreach (var sentence in paragraph.Sentences)
                {
                    var seed = random.Next(int.MaxValue);

                    // EZSTODO - sort property values by percent change (largest to smallest)
                    // - need to handle conjunction correctly. Above needs to be sorted by pos/neg change. All positive/negatives are
                    // "and"ed while the two are "or"ed.
                    foreach (var propertyValue in sentence.PropertyValues)
                    {
                        var result = GetSentenceFragment(sentence.Code, propertyValue, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
                        results.Add(propertyValue, result);
                    }
                }
            }
            //var ezs = GetSentenceFragment(definitions, data.Paragraphs[0].Sentences[0].Code, data.Paragraphs[0].Sentences[0].PropertyValues[0], "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE]");
        }
    }
}
