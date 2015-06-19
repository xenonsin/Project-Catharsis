using strange.extensions.command.impl;
using UnityEngine;
namespace Catharsis.InputEditor
{
    public class LoadInputCommand : Command
    {

        [Inject]
        public IInputManager InputManager { get; set; }

        [Inject]
        public InputManagerLoadedSignal LoadedSignal { get; set; }
        [Inject]
        public InputManagerLoadDefaultInputSignal LoadDefaultInputSignal { get; set; }

        [Inject]
        public IPathUtility PathUtility { get; set; }

        public override void Execute()
        {

            string path = PathUtility.GetUserInputSaveFolder() + "/input_config.xml";
            if (System.IO.File.Exists(path))
            {
                InputLoaderXML loader = new InputLoaderXML(path);
                InputManager.Load(loader);
                LoadSuccessful();
            }

            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("The Input Config File Hasn't Been Created Yet! This can't be done on runtime.");
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
            LoadDefaultInputSignal.Dispatch();

#if UNITY_EDITOR
            Debug.Log("Input Config Failed To Load, will now attempt to look for default InputConfig.");
#endif
        }
    }
}