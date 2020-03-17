using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutoTextApp
{
    public interface IMacroVariables
    {
        void Add(IEnumerable<MacroVariable> macros);

        void AddOrUpdate(MacroVariable macro);

        MacroVariable Get(string name);
    }

    public class MacroVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Value}, {Type}";
        }
    }

    public class MacroVariableKeyedDictionary : KeyedCollection<string, MacroVariable>
    {
        protected override string GetKeyForItem(MacroVariable item)
        {
            return item.Name;
        }
    }

    public class MacroVariables : IMacroVariables
    {
        private MacroVariableKeyedDictionary _macroVariables = new MacroVariableKeyedDictionary(); // Macro -> Value

        public void Add(IEnumerable<MacroVariable> macros)
        {
            foreach (var macro in macros)
            {
                AddOrUpdate(macro);
            }
        }

        public void AddOrUpdate(MacroVariable macro)
        {
            if (_macroVariables.Contains(macro.Name))
            {
                _macroVariables.Remove(macro.Name);
                _macroVariables.Add(macro);
            }
            else
            {
                _macroVariables.Add(macro);
            }
        }

        public MacroVariable Get(string macroName)
        {
            return _macroVariables.FirstOrDefault(x => x.Name == macroName.TrimStart('[').TrimEnd(']'));
        }
    }
}
