using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public interface IFragmentGenerators
    {
        MetricFragment CreateMetricFragment(string metricCode, string template);

        VariableFragment CreateVariableFragment(string variableCode, string template);

        DataFragment CreateDataFragment(
            VariableFragment variableFragment,
            string metricCode,
            string variableCode,
            string template,
            string flatTemplate);

        SentenceFragment CreateSentenceFragment(string metricCode, string[] variableCodes, AutoTextTemplate templates);
    }

    public class FragmentGenerators : IFragmentGenerators
    {
        private IDefinitionProvider _definitionProvider;
        private IDataProvider _dataProvider;
        private IDirection _direction;

        public FragmentGenerators(
            IDefinitionProvider definitionProvider, 
            IDataProvider dataProvider,
            IDirection direction)
        {
            _definitionProvider = definitionProvider;
            _dataProvider = dataProvider;
            _direction = direction;
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

        private DirectionType GetDirection(DirectionType direction, bool isIncreasePostive)
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
            var actualDirection = GetDirection(variableData.Direction_Old, metric.IsIncreasePostive);
            var actualTemplate = variableData.Direction_Old == DirectionType.FLAT ? flatTemplate : template;
            var directionText = _direction.GetDirectionText(actualDirection, metric.IsIncreasePostive);

            return new DataFragment(variableData, actualTemplate, variableCode, variableFragment, actualDirection, directionText);
        }

        public SentenceFragment CreateSentenceFragment(string metricCode, string[] variableCodes, AutoTextTemplate templates)
        {
            var sentenceFragment = new SentenceFragment();

            var metric = CreateMetricFragment(metricCode, templates.Metric);
            sentenceFragment.AddMetric(metric);
            if (variableCodes != null)
            {
                foreach (var variableCode in variableCodes)
                {
                    var variable = CreateVariableFragment(variableCode, templates.Variable);
                    var data = CreateDataFragment(variable, metricCode, variableCode, templates.Data, templates.FlatData);
                    sentenceFragment.AddData(data);
                }
            }

            return sentenceFragment;
        }
    }


}
