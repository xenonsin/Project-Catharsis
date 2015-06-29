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

        //Todo: make command that calls this at gamestart, when the savefile is picked?
        #region Serialization
        //Load default variables???
        //Make this a command????
        public void loadGlobalVariables(string globalVariablesXml)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(GlobalVariables));
            XmlReader xmlReader = XmlReader.Create(new StringReader(globalVariablesXml));
            GlobalVariables newGlobalVariables = (GlobalVariables) deserializer.Deserialize(xmlReader);

            //Bools
            for (int i = 0; i < newGlobalVariables.booleans.Count; i++)
            {
                _data.booleans[i] = newGlobalVariables.booleans[i];
            }

            //Floats
            for (int i = 0; i < newGlobalVariables.floats.Count; i++)
            {
                _data.floats[i] = newGlobalVariables.floats[i];
            }

            //String
            for (int i = 0; i < newGlobalVariables.strings.Count; i++)
            {
                _data.strings[i] = newGlobalVariables.strings[i];
            }
        }

        //Todo: when person saves game?
        public void saveGlobalVariables()
        {

        }
        #endregion

        #region Getters and Setters

        public float GetGlobalFloat(int id)
        {
            return _data.floats[id];
        }

        public void SetGlobalFloat(int id, float value)
        {
            _data.floats[id] = value;
        }

        public bool GetGlobalBool(int id)
        {
            return _data.booleans[id];
        }

        public void SetGlobalBool(int id, bool value)
        {
            _data.booleans[id] = value;
        }

        public string GetGlobalString(int id)
        {
            return _data.strings[id];
        }

        public void SetGlobalString(int id, string value)
        {
            _data.strings[id] = value;
        }

        #endregion
    }
}