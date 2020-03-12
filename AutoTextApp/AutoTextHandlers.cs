﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public class AutoTextHandlers
    {
        public AutoTextDefinitionsHandler DefinitionHandler { get; private set; }
        public AutoTextDataHandler DataHandler { get; private set; }

        public AutoTextHandlers(AutoTextDefinitionsHandler definitionHandler, AutoTextDataHandler dataHandler)
        {
            DefinitionHandler = definitionHandler;
            DataHandler = dataHandler;
        }
    }
}
