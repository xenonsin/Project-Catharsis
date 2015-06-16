using System;
using System.Collections.Generic;
using UnityEngine;

namespace Catharsis.InputEditor.Service
{
    public class InputLoaderJSON : IInputLoader
    {
        public void Load(TextAsset file, out List<InputConfiguration> inputConfigurations, out string defaultConfig)
        {
            inputConfigurations = new List<InputConfiguration>();
            defaultConfig = string.Empty;
            var n = SimpleJSON.JSON.Parse(file.text);

            //defaultConfig = n["Input"]
        }

        public InputConfiguration LoadSelective(string inputConfigName)
        {
            throw new NotImplementedException();
        }

        
    }
}