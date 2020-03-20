using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace AutoTextApp
{
    public interface IFragmentData
    {
    }

    public class MetricDefinition : IFragmentData
    {
        public string Code { get; set; }

        public string ShortName { get; set; }

        public string LongName { get; set; }

        public string Units { get; set; } = "";

        public bool IsPlural { get; set; }

        public bool IsIncreasePostive { get; set; }

        public override string ToString()
        {
            return $"{Code}: {ShortName}, {LongName}, {Units}, IsPlural:{IsPlural}, IsIncreasePositive:{IsIncreasePostive}";
        }
    }

    public class VariableDefinition : IFragmentData
    {
        public string Code { get; set; }

        public string ShortName { get; set; }

        public string LongName { get; set; }

        public string Units { get; set; } = "";

        public override string ToString()
        {
            return $"{Code}: {ShortName}, {LongName}, {Units}";
        }
    }

    public class Synonyms
    {
        [XmlArrayItem(typeof(string), ElementName = "Word")]
        public string[] Positive { get; set; }

        [XmlArrayItem(typeof(string), ElementName = "Word")]
        public string[] Negative { get; set; }

        [XmlArrayItem(typeof(string), ElementName = "Word")]
        public string[] Flat { get; set; }
    }

    public class AutoTextDefinitions
    {
        [XmlArrayItem(typeof(MetricDefinition), ElementName = "Metric")]
        public MetricDefinition[] Metrics { get; set; }

        [XmlArrayItem(typeof(VariableDefinition), ElementName = "Variable")]
        public VariableDefinition[] Variables { get; set; }

        public Synonyms Synonyms { get; set; }

        public MacroVariable[] MacroVariables { get; set; }

        public Templates Templates { get; set; }

    }
}
