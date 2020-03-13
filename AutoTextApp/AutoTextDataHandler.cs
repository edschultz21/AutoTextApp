using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public class AutoTextDataHandler
    {
        private readonly AutoTextData _data;
        private Dictionary<string, int> _metricToId;
        private Dictionary<string, int> _variableToId;

        public Block[] Blocks { get { return _data.Blocks; } } // EZSTODO - remove?

        public AutoTextDataHandler(AutoTextData data)
        {
            _data = data;

            // Normalize casing
            Array.ForEach(_data.Blocks,
                x => Array.ForEach(x.BlockItems, y =>
                {
                    y.MetricCode = y.MetricCode.ToUpper();
                    y.VariableCodes = y.VariableCodes == null ? null : Array.ConvertAll(y.VariableCodes, z => z.ToUpper());
                }));
            Array.ForEach(_data.Metrics, x => x.Code = x.Code.ToUpper());
            Array.ForEach(_data.Variables, x => x.Code = x.Code.ToUpper());

            // Init lookups
            _metricToId = _data.Metrics.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);
            _variableToId = _data.Variables.ToDictionary(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase);

            // Set direction based on value
            foreach (var metricData in _data.MetricData)
            {
                foreach (var variableData in metricData.VariableData)
                {
                    variableData.Direction = DirectionType.FLAT;
                    if (variableData.PercentChange >= 0.05)
                    {
                        var isPositive = (variableData.CurrentValue - variableData.PreviousValue) > 0;
                        variableData.Direction = isPositive ? DirectionType.POSITIVE : DirectionType.NEGATIVE;
                    }
                }
            }
        }

        // EZSTODO - need to handle case of no variable
        // EZSTODO - need to handle cases of missing metric/variable
        public VariableData GetVariableData(string metricCode, string variableCode)
        {
            MetricData metricData = null;
            if (_metricToId.TryGetValue(metricCode, out int metricId))
            {
                metricData = _data.MetricData.FirstOrDefault(x => x.Id == metricId);
            }
            if (metricData != null)
            {
                if (string.IsNullOrEmpty(variableCode))
                {
                    return metricData.VariableData.FirstOrDefault();
                }
                else if (_variableToId.TryGetValue(variableCode, out int variableId))
                {
                    return metricData.VariableData.FirstOrDefault(x => x.Id == variableId);
                }
            }


            return null;
        }
    }
}
