using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AutoTextApp
{
    public class AutoTextUtils
    {
        public static T ReadXmlData<T>(string filename)
        {
            StreamReader streamReader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(streamReader);
        }

        public static string ProcessMacros(object data, string template, Func<string, MacroVariable> macroVariables)
        {
            var result = template;
            var items = Regex.Matches(template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                var itemValue = item.Value.ToUpper();
                var macroVariable = macroVariables(itemValue);
                if (macroVariable != null)
                {
                    string macroValue = null;

                    if (!string.IsNullOrEmpty(macroVariable.Type))
                    {
                        var reflectedType = Type.GetType($"{data.GetType().Namespace}.{macroVariable.Type}");
                        if (data.GetType().Name == macroVariable.Type)
                        {
                            macroValue = reflectedType.GetProperty(macroVariable.Value).GetValue(data).ToString();
                        }
                    }
                    else
                    {
                        macroValue = macroVariable.Value;
                    }

                    if (macroValue != null)
                    {
                        result = result.Replace(item.Value, macroValue);
                    }
                }
            }

            return result;
        }
    }
}
