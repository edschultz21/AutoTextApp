using System;
using System.Collections.Generic;

namespace AutoTextApp
{
    public interface IClauseFragment
    {
        string Template { get; }

        IFragmentData Data { get; }
    }

    public abstract class ClauseFragment : IClauseFragment
    {
        public string Template { get; private set; }
        public abstract IFragmentData Data { get; }

        public ClauseFragment(string template)
        {
            Template = template;
        }
    }

    public class MetricFragment : ClauseFragment
    {
        public string MetricCode { get; private set; }
        public MetricDefinition Metric { get; set; }
        public override IFragmentData Data { get { return Metric; } }

        public MetricFragment(MetricDefinition metric, string template, string metricCode) :
            base(template)
        {
            Metric = metric;
            MetricCode = metricCode;
        }
    }

    public class MetricLocationFragment : ClauseFragment
    {
        public string MetricLocation { get; private set; }
        public override IFragmentData Data { get { return null; } }

        public MetricLocationFragment(IFragmentData data, string template, string metricLocation) :
            base(template)
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
        public string DirectionText { get; set; }
        public override IFragmentData Data { get { return VariableData; } }

        public DataFragment(
            VariableData variableData, 
            string template, 
            string variableCode, 
            VariableFragment variableFragment, 
            DirectionType direction,
            string directionText) :
            base(template)
        {
            VariableData = variableData;
            VariableCode = variableCode;
            VariableFragment = variableFragment;
            Direction = direction;
            DirectionText = directionText;
        }
    }

    public class VariableFragment : ClauseFragment
    {
        public string VariableCode { get; private set; }
        public VariableDefinition Variable { get; set; }
        public override IFragmentData Data { get { return Variable; } }

        public VariableFragment(VariableDefinition variable, string template, string variableCode) :
            base(template)
        {
            Variable = variable;
            VariableCode = variableCode;
        }
    }

    public interface ISentenceFragment
    {
        void AddMetric(MetricFragment metric);

        void AddData(DataFragment data);
    }

    public class SentenceFragment : ISentenceFragment
    {
        public MetricFragment MetricFragment { get; set; }
        public List<IClauseFragment> DataFragments { get; set; } = new List<IClauseFragment>();

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
