using System.Collections.Generic;
using Catharsis.InputEditor.Service;
using strange.extensions.editor.impl;
using UnityEditor;
using UnityEngine;

namespace Catharsis.InputEditor.View
{
    public class InputManagerEditorMediator : EditorMediator
    {
         [Inject]
        public InputManagerEditorView view { get; set; }


        [Inject]
        public IInputLoader InputLoader { get; set; }
        public override void OnRegister()
        {
            base.OnRegister();
            view.SendLoadPathSignal.AddListener(OnLoadInputSignal);
            //view.SaveInputSignal.AddListener(OnSaveInputSignal());
            //LoadInputSignal.AddListener(OnLoadInputSignal);
            //SaveInputSignal.AddListener(OnSaveInputSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            view.SendLoadPathSignal.RemoveListener(OnLoadInputSignal);
           // SaveInputSignal.RemoveListener(OnSaveInputSignal);
        }

        private void OnLoadInputSignal(string path)
        {
            string defaultConfig = "";
            List<InputConfiguration> config;

            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null)
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(textAsset.text))
                {
                    InputLoader.Load(textAsset,out config, out defaultConfig);
                    view.ReplaceCurrentConfigurations(defaultConfig,config);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Failed to load input configurations. The resource file might have been deleted or renamed.", "OK");
            }
        }

        private void OnSaveInputSignal(string path)
        {
            
        }



    }
}