/*
 * The purpose of the global variables manager is to keep track of the "global" variables that can exist within a single game. For example,
 * it can keep track of the quest objectives the player has completed, or the choices a player has made during a dialogue choice.
 * 
 * At it's current state, it only saves and loads floats, strings, and booleans. In the future, I could extend this to keep track of map data.
 * Currently, this module is being used by the Dialogue System.
 * 
 * Work in progress features:
 * Saving to save folder.
 * Loading.
 * Loading default.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Catharsis.GlobalVariablesManager.Model;
using strange.extensions.injector.api;

namespace Catharsis.GlobalVariablesManager
{
    [Serializable]
    [Implements(typeof(IGlobalVariableManager), InjectionBindingScope.CROSS_CONTEXT)]

    public class GlobalVariableManager : IGlobalVariableManager
    {
        private GlobalVariables _data;

        public GlobalVariableManager() { _data = new GlobalVariables();}

        //Todo: make command that calls this at gamestart, when the savefile is picked?
        #region Serialization
        public void loadGlobalVariables(string globalVariablesXml)
        {
            //XmlSerializer deserializer = new XmlSerializer(typeof(GlobalVariables));
           // XmlReader xmlReader = XmlReader.Create(new StringReader(globalVariablesXml));
            //GlobalVariables newGlobalVariables = (GlobalVariables) deserializer.Deserialize(xmlReader);

            //Bools
            //for (int i = 0; i < newGlobalVariables.booleans.Count; i++)
            //{
            //    _data.booleans[i] = newGlobalVariables.booleans[i];
            //}

            ////Floats
            //for (int i = 0; i < newGlobalVariables.floats.Count; i++)
            //{
            //    _data.floats[i] = newGlobalVariables.floats[i];
            //}

            ////String
            //for (int i = 0; i < newGlobalVariables.strings.Count; i++)
            //{
            //    _data.strings[i] = newGlobalVariables.strings[i];
            //}
        }

        //Todo: when person saves game?
        public void saveGlobalVariables()
        {

        }
        #endregion

        #region Getters and Setters

        public float GetGlobalFloat(string id)
        {
            float value;
            if (_data.floats.TryGetValue("id", out value))
                return value;

            return 0f; 
        }

        public void SetGlobalFloat(string id, float value)
        {
            if (_data.floats.ContainsKey(id))
                _data.floats[id] = value;
            else
                _data.floats.Add("id", value);
        }

        public bool GetGlobalBool(string id)
        {
            bool value;
            if (_data.booleans.TryGetValue("id", out value))
                return value;
            
            return false;
        }

        public void SetGlobalBool(string id, bool value)
        {
            if (_data.booleans.ContainsKey(id))
                _data.booleans[id] = value;
            else
                _data.booleans.Add("id", value);
        }

        public string GetGlobalString(string id)
        {
            string value;
            if (_data.strings.TryGetValue("id", out value))
                return value;

            return default(string);
        }

        public void SetGlobalString(string id, string value)
        {
            if (_data.strings.ContainsKey(id))
                _data.strings[id] = value;
            else
                _data.strings.Add("id", value);
        }

        #endregion
    }
}