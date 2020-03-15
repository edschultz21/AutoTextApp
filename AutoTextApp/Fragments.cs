using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public interface IClauseFragment
    {
        string Template { get; }
    }

    public class ClauseFragment : IClauseFragment
    {
        public string Template { get; private set; }

        public ClauseFragment(string template)
        {
            Template = template;
        }
    }

    public class MetricFragment : ClauseFragment
    {
        public string MetricCode { get; private set; }
        public MetricDefinition Metric { get; set; }

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

        public DataFragment(VariableData variableData, string template, string variableCode, VariableFragment variableFragment, DirectionType direction) :
            base(template)
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

        public VariableFragment(VariableDefinition variable, string template, string variableCode) :
            base(template)
        {
            Variable = variable;
            VariableCode = variableCode;
        }
    }
}
