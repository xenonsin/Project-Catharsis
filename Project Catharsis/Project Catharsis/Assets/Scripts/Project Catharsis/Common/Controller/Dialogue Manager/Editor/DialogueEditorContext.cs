
using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Controller;
using strange.extensions.editor.impl;
using UnityEditor;

namespace Catharsis.DialogueEditor
{
    [InitializeOnLoad]
    public class DialogueEditorContext : EditorMVCSContext
    {
        //This static Constructor is called because of the InitilizedOnLoad
        //tag above. We use it to instantiate our Context.
        static DialogueEditorContext()
        {
            new DialogueEditorContext();
        }

        protected override void mapBindings()
        {
            base.mapBindings();


            //Injections
            //injectionBinder.Bind<IInputLoader>().To<InputLoaderXML>();
            //injectionBinder.Bind<IInputSaver>().To<InputSaverXML>();
            //injectionBinder.Bind<IPathUtility>().To<PathUtility>();
            //Commands
            commandBinder.Bind<DialogueEditorStartSignal>().To<InitDialogueCommand>();

            //Views
            mediationBinder.Bind<DialogueEditorView>().To<DialogueEditorMediator>();
        }

        public override void Launch()
        {
            injectionBinder.GetInstance<DialogueEditorStartSignal>().Dispatch();
        }
    }
}