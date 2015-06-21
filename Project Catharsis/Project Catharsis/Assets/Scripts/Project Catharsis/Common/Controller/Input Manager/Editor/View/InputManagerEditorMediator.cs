using System.Collections.Generic;
using System.IO;
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


        //[Inject]
        //public IInputLoader InputLoader { get; set; }

        //[Inject]
        //public IInputSaver InputSaver { get; set; }

        [Inject]
        public IPathUtility PathUtility { get; set; }
        public override void OnRegister()
        {
            base.OnRegister();
            view.LoadInputConfigSignal.AddListener(OnLoadInputSignal);
            view.SaveInputConfigSignal.AddListener(OnSaveInputSignal);
            //LoadInputSignal.AddListener(OnLoadInputSignal);
            //SaveInputSignal.AddListener(OnSaveInputSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
         //   view.SendLoadPathSignal.RemoveListener(OnLoadInputSignal);
           // SaveInputSignal.RemoveListener(OnSaveInputSignal);
        }

        //Would be great if these were commands.......
        private void OnLoadInputSignal()
        {

            TextAsset textAsset = Resources.Load("InputManager/default_input") as TextAsset;
            if (textAsset != null)
            {
                string defaultConfig = "";
                List<InputConfiguration> configs = new List<InputConfiguration>();

                //Currently Not using the injections..
                InputLoaderXML loader = new InputLoaderXML(new StringReader(textAsset.text));
                loader.Load(out configs,out defaultConfig);
                view.SetCurrentConfigurations(configs,defaultConfig);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Failed to load input configurations. The resource file might have been deleted or renamed.", "OK");
            }
        }

        private void OnSaveInputSignal(List<InputConfiguration> configs, string defaultConfig )
        {
            string saveFolder = PathUtility.GetDefaultInputSaveFolder();
            if (!System.IO.Directory.Exists(saveFolder))
                System.IO.Directory.CreateDirectory(saveFolder);

            //Currently Not using the injections..
            InputSaverXML saver = new InputSaverXML(saveFolder + "/default_input.xml");
            saver.Save(configs,defaultConfig);
		
        }

        private void OnExportInputSignal(List<InputConfiguration> configs, string defaultConfig)
        {
            
        }

        private void OnImportInputSignal()
        {
            
        }



    }
}