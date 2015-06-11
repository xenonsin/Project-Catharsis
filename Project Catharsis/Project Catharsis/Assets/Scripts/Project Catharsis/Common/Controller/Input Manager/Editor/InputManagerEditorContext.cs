using UnityEngine;
using System.Collections;
using Catharsis.InputEditor.Controller;
using Catharsis.InputEditor.Service;
using Catharsis.InputEditor.View;
using UnityEditor;
using strange.extensions.editor.impl;

namespace Catharsis.InputEditor
{
    [InitializeOnLoad]
    public class InputManagerEditorContext : EditorMVCSContext
    {
        //This static Constructor is called because of the InitilizedOnLoad
        //tag above. We use it to instantiate our Context.
        static InputManagerEditorContext()
        {
            new InputManagerEditorContext();
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            
        
            //Injections
            injectionBinder.Bind<ScriptReloadSignal>().ToSingleton();
            injectionBinder.Bind<ScriptReloadService>().ToSingleton();
            //Commands
            commandBinder.Bind<InputManagerEditorStartSignal>().To<InitInputManagerCommand>();

            //Views
            mediationBinder.Bind<InputManagerEditorView>().To<InputManagerEditorMediator>();
        }

        public override void Launch()
        {
            injectionBinder.GetInstance<InputManagerEditorStartSignal>().Dispatch();
        }
    }

}   