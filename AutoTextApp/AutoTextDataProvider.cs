using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoTextApp
{
    public interface IDataProvider
    {
        VariableData GetVariableData(string metricCode, string variableCode);
    }

    public class AutoTextDataProvider : IDataProvider
    {
        private readonly MetricData[] _metricData;
        private Dictionary<string, int> _metricToId;
        private Dictionary<string, int> _variableToId;

        public Block[] Blocks { get; private set; }

        public AutoTextDataProvider(AutoTextData dataIn)
        {
            var data = dataIn;
            _metricData = data.MetricData;
            Blocks = data.Blocks;

            // Normalize casing
            Array.ForEach(data.Blocks,
                x => Array.ForEach(x.BlockItems, y =>
                {
                    y.MetricCode = y.MetricCode.ToUpper();
                    y.VariableCodes = y.VariableCodes == null ? null : Array.ConvertAll(y.VariableCodes, z => z.ToUpper());
                }));
            Array.ForEach(data.Metrics, x => x.Code = x.Code.ToUpper());
            Array.ForEach(data.Variables, x => x.Code = x.Code.ToUpper());

            // Init lookups
            _metricToId = data.Metrics.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            _variableToId = data.Variables.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);

            // Set direction based on value
            foreach (var metricData in data.MetricData)
            {
                foreach (var variableData in metricData.VariableData)
                {
                    variableData.Direction_Old = DirectionType.FLAT;
                    if (variableData.PercentChange >= 0.05)
                    {
                        var isPositive = (variableData.CurrentValue - variableData.PreviousValue) > 0;
                        variableData.Direction_Old = isPositive ? DirectionType.POSITIVE : DirectionType.NEGATIVE;
                    }
                }
            }
        }

        // EZSTODO - need to handle case of no variable
        // EZSTODO - need to handle cases of missing metric/variable
        public VariableData GetVariableData(string metricCode, string variableCode)
        {
            MetricData actualMetricData = null;
            if (_metricToId.TryGetValue(metricCode, out int metricId))
            {
                actualMetricData = _metricData.FirstOrDefault(x => x.Id == metricId);
            }
            if (actualMetricData != null)
            {
                if (string.IsNullOrEmpty(variableCode))
                {
                    return actualMetricData.VariableData.FirstOrDefault();
                }
                else if (_variableToId.TryGetValue(variableCode, out int variableId))
                {
                    return actualMetricData.VariableData.FirstOrDefault(x => x.Id == variableId);
                }
            }

            return null;
        }
    }
}
