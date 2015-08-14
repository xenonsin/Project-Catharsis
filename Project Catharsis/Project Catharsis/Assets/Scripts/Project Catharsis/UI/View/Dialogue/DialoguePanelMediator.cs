using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Model.Data;
using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class DialoguePanelMediator : Mediator
    {
        [Inject]
        public DialoguePanelView view { get; set; }

        [Inject]
        public DialogueMessageSignal DialogueMessageSignal { get; set; }

        [Inject]
        public DialogueContinueSignal DialogueContinueSignal { get; set; }

        public override void OnRegister()
        {
            view.ContinueButtonClickSignal.AddListener(Continue);
            DialogueMessageSignal.AddListener(DisplayMessage);
            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {
            view.ContinueButtonClickSignal.RemoveListener(Continue);
            DialogueMessageSignal.RemoveListener(DisplayMessage);
            view.Remove();
        
            base.OnRemove();
        }

        public void DisplayMessage(TextData textData)
        {
            view.DisplayMessage(textData);
        }

        public void Continue()
        {
            DialogueContinueSignal.Dispatch(0);
        }
    }
}