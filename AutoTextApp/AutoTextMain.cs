using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public interface IAutoTextMain
    {
        IDefinitionProvider DefinitionsProvider { get; }

        IDataProvider DataProvider { get; }

        IMacroVariables MacroVariables { get; }

        IDirection Direction { get;     }
    }

    public class AutoTextMain : IAutoTextMain
    {
        public IDefinitionProvider DefinitionsProvider { get; private set; }
        public IDataProvider DataProvider { get; private set; }
        public IMacroVariables MacroVariables { get; private set; }
        public IDirection Direction { get; private set; }

        public AutoTextMain(AutoTextDefinitions definitions, AutoTextData data)
        {
            DefinitionsProvider = new AutoTextDefinitionsProvider(definitions);
            DataProvider = new AutoTextDataProvider(data);

            // Setup macro variables
            MacroVariables = new MacroVariables();
            MacroVariables.Add(definitions.MacroVariables);

            // Setup direction
            Direction = new Direction(definitions.Synonyms);
        }
    }
}
