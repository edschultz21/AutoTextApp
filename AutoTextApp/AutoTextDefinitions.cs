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

        public bool IsPlural { get; set; }

        public bool IsIncreasePostive { get; set; }

        public override string ToString()
        {
            return $"{Code}: {ShortName}, {LongName}, IsPlural:{IsPlural}, IsIncreasePositive:{IsIncreasePostive}";
        }
    }

    public class VariableDefinition : IFragmentData
    {
        public string Code { get; set; }

        public string ShortName { get; set; }

        public string LongName { get; set; }

        public override string ToString()
        {
            return $"{Code}: {ShortName}, {LongName}";
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

    // EZSTODO - Does not belong here
    public class MacroVariableKeyedDictionary : KeyedCollection<string, MacroVariable>
    {
        protected override string GetKeyForItem(MacroVariable item)
        {
            return item.Name;
        }
    }

    public class MacroVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }
    }

    public class AutoTextDefinitions
    {
        [XmlArrayItem(typeof(MetricDefinition), ElementName = "Metric")]
        public MetricDefinition[] Metrics { get; set; }

        [XmlArrayItem(typeof(VariableDefinition), ElementName = "Variable")]
        public VariableDefinition[] Variables { get; set; }

        public Synonyms Synonyms { get; set; }

        public MacroVariable[] MacroVariables { get; set; }

    }
}
