using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public class AutoTextHandlers
    {
        public AutoTextDefinitionsProvider DefinitionHandler { get; private set; }
        public AutoTextDataProvider DataHandler { get; private set; }

        public AutoTextHandlers(AutoTextDefinitionsProvider definitionHandler, AutoTextDataProvider dataHandler)
        {
            DefinitionHandler = definitionHandler;
            DataHandler = dataHandler;
        }
    }
}
