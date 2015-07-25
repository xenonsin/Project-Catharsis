using System;
using System.Collections.Generic;

namespace Catharsis.GlobalVariablesManager.Model
{
    [Serializable]
    public class GlobalVariables
    {
        //TODO CANT SERIALIZE DICTIONARIESSS

        public Dictionary<int, bool> booleans;
        public Dictionary<int, float> floats;
        public Dictionary<int, string> strings;

        public GlobalVariables()
        {
            booleans = new Dictionary<int, bool>();
            floats = new Dictionary<int, float>();
            strings = new Dictionary<int, string>();
        }

        public GlobalVariables(Dictionary<int, bool> booleans, Dictionary<int, float> floats, Dictionary<int, string> strings)
        {
            this.booleans = booleans;
            this.floats = floats;
            this.strings = strings;
        }
    }
}