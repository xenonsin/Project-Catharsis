using System;
using System.Collections.Generic;
using UnityEngine;

namespace Catharsis.InputEditor.Service
{
    public interface IInputLoader
    {
        void Load(TextAsset file, out List<InputConfiguration> inputConfigurations, out string defaultConfig);
        InputConfiguration LoadSelective(string inputConfigName);
    }
}