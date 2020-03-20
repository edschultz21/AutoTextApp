using System;
using System.Collections.Generic;

namespace AutoTextApp
{
    public interface IClauseFragment
    {
        IFragmentData Data { get; }
    }

    public abstract class ClauseFragment : IClauseFragment
    {
        public abstract IFragmentData Data { get; }
    }

    public class MetricFragment : ClauseFragment
    {
        public string MetricCode { get; private set; }
        public MetricDefinition Metric { get; set; }
        public override IFragmentData Data { get { return Metric; } }

        public MetricFragment(MetricDefinition metric, string metricCode)
        {
            Metric = metric;
            MetricCode = metricCode;
        }
    }

    public class MetricLocationFragment : ClauseFragment
    {
        public string MetricLocation { get; private set; }
        public override IFragmentData Data { get { return null; } }

        public MetricLocationFragment(IFragmentData data, string metricLocation)
        {
            MetricLocation = metricLocation;
        }
    }

    public class DataFragment : ClauseFragment
    {
        public string VariableCode { get; set; }
        public VariableData VariableData { get; set; }
        public VariableFragment VariableFragment { get; set; }
        public DirectionType Direction { get; set; }
        public string Units { get; set; } = "";
        public override IFragmentData Data { get { return VariableData; } }

        public DataFragment(
            VariableData variableData, 
            string variableCode, 
            VariableFragment variableFragment, 
            DirectionType direction)
        {
            VariableData = variableData;
            VariableCode = variableCode;
            VariableFragment = variableFragment;
            Direction = direction;
        }
    }

    public class VariableFragment : ClauseFragment
    {
        public string VariableCode { get; private set; }
        public VariableDefinition Variable { get; set; }
        public override IFragmentData Data { get { return Variable; } }

        public VariableFragment(VariableDefinition variable, string variableCode)
        {
            Variable = variable;
            VariableCode = variableCode;
        }
    }

    public interface ISentenceFragment
    {
        MetricFragment MetricFragment { get; }

        List<DataFragment> DataFragments { get; }

        void AddMetric(MetricFragment metric);

        void AddData(DataFragment data);
    }

    public class SentenceFragment : ISentenceFragment
    {
        public MetricFragment MetricFragment { get; set; }
        public List<DataFragment> DataFragments { get; set; } = new List<DataFragment>();

        public void AddMetric(MetricFragment metric)
        {
            if (MetricFragment != null)
            {
                throw new Exception("Sentence already has a metric fragment."); // EZSTODO - proper exception
            }
            MetricFragment = metric;
        }

        public void AddData(DataFragment data)
        {
            DataFragments.Add(data);
        }
    }
}
