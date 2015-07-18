using System;
using System.Collections.Generic;

namespace Catharsis.GlobalVariablesManager.Model
{
    [Serializable]
    public class GlobalVariables
    {
        public Dictionary<string, bool> booleans;
        public Dictionary<string, float> floats;
        public Dictionary<string, string> strings;

        public GlobalVariables(Dictionary<string, bool> booleans, Dictionary<string, float> floats, Dictionary<string, string> strings)
        {
            this.booleans = booleans;
            this.floats = floats;
            this.strings = strings;
        }
    }
}