﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.9136
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.1432.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class AutoTextData {
    
    private AutoTextDataParagraph[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Paragraph", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public AutoTextDataParagraph[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class AutoTextDataParagraph {
    
    private AutoTextDataParagraphSentence[] sentenceField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Sentence", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public AutoTextDataParagraphSentence[] Sentence {
        get {
            return this.sentenceField;
        }
        set {
            this.sentenceField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class AutoTextDataParagraphSentence {
    
    private string codeField;
    
    private AutoTextDataParagraphSentencePropertyValue[] propertyValueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Code {
        get {
            return this.codeField;
        }
        set {
            this.codeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("PropertyValue", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public AutoTextDataParagraphSentencePropertyValue[] PropertyValue {
        get {
            return this.propertyValueField;
        }
        set {
            this.propertyValueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class AutoTextDataParagraphSentencePropertyValue {
    
    private string nameField;
    
    private string currentValueField;
    
    private string previousValueField;
    
    private string percentChangeField;
    
    private string consecutivePeriodsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CurrentValue {
        get {
            return this.currentValueField;
        }
        set {
            this.currentValueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PreviousValue {
        get {
            return this.previousValueField;
        }
        set {
            this.previousValueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PercentChange {
        get {
            return this.percentChangeField;
        }
        set {
            this.percentChangeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ConsecutivePeriods {
        get {
            return this.consecutivePeriodsField;
        }
        set {
            this.consecutivePeriodsField = value;
        }
    }
}