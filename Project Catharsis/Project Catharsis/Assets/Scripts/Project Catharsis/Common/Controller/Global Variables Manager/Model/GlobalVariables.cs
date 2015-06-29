using System;
using System.Collections.Generic;

namespace Catharsis.GlobalVariablesManager.Model
{
    [Serializable]
    public class GlobalVariables
    {
        public List<bool> booleans;
        public List<float> floats;
        public List<string> strings;

        public GlobalVariables(List<bool> booleans, List<float> floats, List<string> strings)
        {
            this.booleans = booleans;
            this.floats = floats;
            this.strings = strings;
        }
    }
}