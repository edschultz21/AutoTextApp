using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json.Converters;

namespace AutoTextApp
{
    // EZSTODO - determine what values we need here
    public enum BlockType
    {
        Sentence,
        Fragments,
        Auto
    }

    // EZSTODO - determine what values we need here
    public enum BlockItemType
    {
        Standard,
        MaxVariable,
        MinVariable
    }

    public enum DirectionType
    {
        POSITIVE,
        NEGATIVE,
        FLAT
    }

    public class IdCode
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return $"{Code}: {Id}";
        }
    }

    public class MetricData_Org
    {
        public string Name { get; set; }

        public float CurrentValue { get; set; }

        public float PreviousValue { get; set; }

        public float PercentChange { get; set; }

        public int ConsecutivePeriods { get; set; }

        [XmlIgnore]
        public DirectionType Direction { get; set; }

        public override string ToString()
        {
            return $"{Name}: {CurrentValue}, {PreviousValue}, {PercentChange}%, {ConsecutivePeriods}";
        }
    }

    public class VariableData
    {
        public int Id { get; set; }

        public float CurrentValue { get; set; }

        public float PreviousValue { get; set; }

        public float PercentChange { get; set; }

        public int ConsecutivePeriods { get; set; }

        [JsonIgnore]
        public DirectionType Direction { get; set; }

        public override string ToString()
        {
            return $"{Id}: {CurrentValue}, {PreviousValue}, {PercentChange}%, {ConsecutivePeriods}";
        }
    }

    public class AutoTextData1
    {
        public IdCode[] Metrics { get; set; }
        public IdCode[] Variables { get; set; }
        public Block[] Blocks { get; set; }
        public MetricData[] MetricData { get; set; }
    }

    public class MetricData
    {
        public int Id { get; set; }

        [JsonProperty("Variables")]
        public VariableData[] VariableData { get; set; }
    }

    public class Block
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public BlockType BlockType { get; set; }
        public BlockItem[] BlockItems { get; set; }

        public override string ToString()
        {
            var temp = string.Join(" :: ", (object[])BlockItems);
            return $"{temp}";
        }
    }

    public class BlockItem
    {
        public string MetricCode { get; set; }
        public string[] Variables { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BlockItemType Type { get; set; }

        public override string ToString()
        {
            var temp = string.Join(",", Variables);
            return $"{MetricCode}->{temp}";
        }
    }

    public class Sentence
    {
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Code { get; set; }

        [XmlElementAttribute("PropertyValue")] 
        public MetricData_Org[] PropertyValues { get; set; }

        public override string ToString()
        {
            var temp = string.Join(" :: ", (IEnumerable<MetricData_Org>) PropertyValues);
            return $"{Code}: {temp}";
        }
    }

    public class Paragraph
    {
        [XmlElementAttribute("Sentence")]
        public Sentence[] Sentences { get; set; }
    }

    public class AutoTextData
    {
        [XmlElementAttribute("Paragraph")]
        public Paragraph[] Paragraphs { get; set; }
    }
}
