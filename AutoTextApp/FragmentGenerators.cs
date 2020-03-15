using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public class FragmentGenerators
    {
        private IDefinitionProvider _definitionProvider;
        private IDataProvider _dataProvider;

        public FragmentGenerators(IDefinitionProvider definitionProvider, IDataProvider dataProvider)
        {
            _definitionProvider = definitionProvider;
            _dataProvider = dataProvider;
        }

        public MetricFragment CreateMetricFragment(string metricCode, string template)
        {
            var metric = _definitionProvider.GetMetricDefinition(metricCode);
            if (metric == null)
            {
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }

            return new MetricFragment(metric, template, metricCode);
        }

        public VariableFragment CreateVariableFragment(string variableCode, string template)
        {
            var variable = _definitionProvider.GetVariableDefinition(variableCode);
            if (variable == null)
            {
                throw new Exception($"Variable not found {variableCode}"); // EZSTODO - needs correct exception
            }

            return new VariableFragment(variable, template, variableCode);
        }

        private DirectionType GetDirection(bool isIncreasePostive, DirectionType direction)
        {
            if (direction != DirectionType.FLAT && !isIncreasePostive)
            {
                if (direction == DirectionType.NEGATIVE)
                {
                    return DirectionType.POSITIVE;
                }
                return DirectionType.NEGATIVE;
            }

            return direction;
        }

        public DataFragment CreateDataFragment(
            VariableFragment variableFragment,
            string metricCode,
            string variableCode,
            string template,
            string flatTemplate)
        {
            var metric = _definitionProvider.GetMetricDefinition(metricCode);
            if (metric == null)
            {
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }

            var variableData = _dataProvider.GetVariableData(metricCode, variableCode);
            var actualDirection = GetDirection(metric.IsIncreasePostive, variableData.Direction_Old);
            var actualTemplate = variableData.Direction_Old == DirectionType.FLAT ? flatTemplate : template;

            return new DataFragment(variableData, actualTemplate, variableCode, variableFragment, actualDirection);
        }
    }
}
