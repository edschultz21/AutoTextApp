using Newtonsoft.Json;
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
        [JsonProperty("Variables")]
        public string[] VariableCodes { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BlockItemType Type { get; set; }
        public AutoTextTemplate Templates { get; set; }

        public override string ToString()
        {
            var temp = string.Join(",", VariableCodes);
            return $"{MetricCode}->{temp}";
        }
    }

    public class AutoTextTemplate
    {
        public string Metric { get; set; }
        public string MetricLocation { get; set; }
        public string Data { get; set; }
        public string FlatData { get; set; }
        public string Variable { get; set; }

        public override string ToString()
        {
            return $"{Metric}; {MetricLocation}; {Data}; {FlatData}; {Variable}";
        }
    }

    public class AutoTextData
    {
        public IdCode[] Metrics { get; set; }
        public IdCode[] Variables { get; set; }
        public Block[] Blocks { get; set; }
        public MetricData[] MetricData { get; set; }
    }

    public class VariableData : IFragmentData
    {
        public int Id { get; set; }

        public float CurrentValue { get; set; }

        public float PreviousValue { get; set; }

        public float PercentChange { get; set; }

        public int ConsecutivePeriods { get; set; }

        public string DataFormat { get; set; }

        [JsonIgnore]
        public DirectionType Direction_Old { get; set; }

        public override string ToString()
        {
            return $"{Id}: {CurrentValue}, {PreviousValue}, {PercentChange}%, {ConsecutivePeriods}";
        }
    }

    public class MetricData
    {
        public int Id { get; set; }

        [JsonProperty("Variables")]
        public VariableData[] VariableData { get; set; }
    }
}
