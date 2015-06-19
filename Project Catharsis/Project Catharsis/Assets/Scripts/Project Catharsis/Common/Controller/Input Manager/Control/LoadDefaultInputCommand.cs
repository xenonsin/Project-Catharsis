using System.IO;
using strange.extensions.command.impl;
using UnityEngine;

namespace Catharsis.InputEditor
{
    public class LoadDefaultInputCommand : Command
    {
        [Inject]
        public IInputManager InputManager { get; set; }

        [Inject]
        public InputManagerLoadedSignal LoadedSignal { get; set; }

        [Inject]
        public IPathUtility PathUtility { get; set; }

        public override void Execute()
        {

            //string path = PathUtility.GetDefaultInputSaveFolder() + "/input_config.xml";

            TextAsset asset = Resources.Load("InputManager/default_input") as TextAsset;
            
            if (asset != null)
            {
                InputLoaderXML loader = new InputLoaderXML(new StringReader(asset.text));
                InputManager.Load(loader);
                LoadSuccessful();
            }

            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("The Default file is missing. Something fucked up. ");
#endif
                LoadFailure();
            }
        }

        private void LoadSuccessful()
        {
#if UNITY_EDITOR
            Debug.Log("Input Config Loaded Successfully.");
#endif
            LoadedSignal.Dispatch();

        }

        private void LoadFailure()
        {
#if UNITY_EDITOR
            Debug.Log("You probably need to reinstall :D");
#endif
        }
    }
}