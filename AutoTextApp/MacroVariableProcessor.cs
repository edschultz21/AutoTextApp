using System;
using System.Text.RegularExpressions;

namespace AutoTextApp
{
    public class MacroVariableProcessor
    {
        public static string ProcessMacros(IMacroVariables macroVariables, IClauseFragment fragment)
        {
            return ProcessMacros(macroVariables, fragment, fragment.Template);
        }

        public static string ProcessMacros(IMacroVariables macroVariables, IClauseFragment fragment, string template)
        {
            if (string.IsNullOrEmpty(template))
            {
                return "";
            }

            var result = template;
            var items = Regex.Matches(template, @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                var itemValue = item.Value.ToUpper();
                var macroVariable = macroVariables.Get(itemValue);
                if (macroVariable != null)
                {
                    string macroValue = null;

                    if (!string.IsNullOrEmpty(macroVariable.Type))
                    {
                        if (macroVariable.Type == fragment.GetType().Name)
                        {
                            var fragmentData = fragment.Data;
                            var property = fragmentData.GetType().GetProperty(macroVariable.Value);
                            if (property != null)
                            {
                                macroValue = property.GetValue(fragmentData)?.ToString();
                            }
                            else
                            {
                                property = fragment.GetType().GetProperty(macroVariable.Value);
                                macroValue = property.GetValue(fragment)?.ToString();
                            }
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

