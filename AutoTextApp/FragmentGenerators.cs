using System;

namespace AutoTextApp
{
    public interface IFragmentGenerators
    {
        MetricFragment CreateMetricFragment(string metricCode);

        VariableFragment CreateVariableFragment(string variableCode);

        DataFragment CreateDataFragment(
            VariableFragment variableFragment,
            string metricCode,
            string variableCode);

        SentenceFragment CreateSentenceFragment(string metricCode, string[] variableCodes);
    }

    public class FragmentGenerators : IFragmentGenerators
    {
        private IDefinitionProvider _definitionProvider;
        private IDataProvider _dataProvider;

        public FragmentGenerators(
            IDefinitionProvider definitionProvider, 
            IDataProvider dataProvider)
        {
            _definitionProvider = definitionProvider;
            _dataProvider = dataProvider;
        }

        public MetricFragment CreateMetricFragment(string metricCode)
        {
            var metric = _definitionProvider.GetMetricDefinition(metricCode);
            if (metric == null)
            {
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }

            return new MetricFragment(metric, metricCode);
        }

        public VariableFragment CreateVariableFragment(string variableCode)
        {
            var variable = _definitionProvider.GetVariableDefinition(variableCode);
            if (variable == null)
            {
                throw new Exception($"Variable not found {variableCode}"); // EZSTODO - needs correct exception
            }

            return new VariableFragment(variable, variableCode);
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
            string variableCode)
        {
            var metric = _definitionProvider.GetMetricDefinition(metricCode);
            if (metric == null)
            {
                throw new Exception($"Metric not found {metricCode}"); // EZSTODO - needs correct exception
            }

            var variableData = _dataProvider.GetVariableData(metricCode, variableCode);
            var actualDirection = GetDirection(variableData.Direction, metric.IsIncreasePostive);

            return new DataFragment(variableData, variableCode, variableFragment, actualDirection);
        }

        public SentenceFragment CreateSentenceFragment(string metricCode, string[] variableCodes)
        {
            var sentenceFragment = new SentenceFragment();

            var metric = CreateMetricFragment(metricCode);
            sentenceFragment.AddMetric(metric);
            if (variableCodes != null)
            {
                foreach (var variableCode in variableCodes)
                {
                    var variable = CreateVariableFragment(variableCode);
                    var data = CreateDataFragment(variable, metricCode, variableCode);
                    sentenceFragment.AddData(data);
                }
            }

            return sentenceFragment;
        }
    }


}
