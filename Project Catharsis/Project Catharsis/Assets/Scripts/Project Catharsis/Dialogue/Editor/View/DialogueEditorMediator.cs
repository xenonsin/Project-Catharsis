using strange.extensions.editor.impl;

namespace Catharsis.DialogueEditor
{
    public class DialogueEditorMediator : EditorMediator
    {
        [Inject]
        public DialogueEditorView view { get; set; }

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