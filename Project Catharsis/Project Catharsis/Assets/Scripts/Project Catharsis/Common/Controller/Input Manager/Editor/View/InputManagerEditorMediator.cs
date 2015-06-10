using strange.extensions.editor.impl;

namespace Catharsis.InputEditor.View
{
    public class InputManagerEditorMediator : EditorMediator
    {
         [Inject]
        public InputManagerEditorView view { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

    }
}