using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoTextApp
{
    public enum DirectionType
    {
        POSITIVE,
        NEGATIVE,
        FLAT
    }

    public class PropertyValue
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

    public class Sentence
    {
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Code { get; set; }

        [XmlElementAttribute("PropertyValue")] 
        public PropertyValue[] PropertyValues { get; set; }

        public override string ToString()
        {
            var temp = string.Join(" :: ", (IEnumerable<PropertyValue>) PropertyValues);
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
