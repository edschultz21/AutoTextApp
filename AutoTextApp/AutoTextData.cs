using System.Xml.Serialization;

namespace AutoTextApp
{
    public class PropertyValue
    {
        public string Name { get; set; }
        
        public float CurrentValue { get; set; }
        
        public float PreviousValue { get; set; }
        
        public float PercentChange { get; set; }
        
        public int ConsecutivePeriods { get; set; }
    }

    public class Sentence
    {
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Code { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("PropertyValue")] 
        public PropertyValue[] PropertyValues { get; set; }
    }

    public class Paragraph
    {
        [System.Xml.Serialization.XmlElementAttribute("Sentence")]
        public Sentence[] Sentences { get; set; }
    }

    public class AutoTextData
    {
        [System.Xml.Serialization.XmlElementAttribute("Paragraph")]
        public Paragraph[] Paragraphs { get; set; }
    }
}
