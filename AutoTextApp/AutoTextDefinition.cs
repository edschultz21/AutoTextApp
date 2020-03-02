using System.Xml.Serialization;

namespace AutoTextApp
{
    public class MetricDefinition
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

    public class Synonyms
    {
        [XmlArrayItem(typeof(string), ElementName = "Word")]
        public string[] Positive { get; set; }

        [XmlArrayItem(typeof(string), ElementName = "Word")]
        public string[] Negative { get; set; }

        [XmlArrayItem(typeof(string), ElementName = "Word")]
        public string[] Flat { get; set; }
    }

    public class AutoTextDefinition
    {
        [XmlArrayItem(typeof(MetricDefinition), ElementName = "Metric")]
        public MetricDefinition[] Metrics { get; set; }

        public Synonyms Synonyms { get; set; }
    }
}
