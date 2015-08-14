using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class DialoguePageMediator : Mediator
    {
        [Inject]
        public DialoguePageView view { get; set; }

        [Inject]
        public ShowDialoguePageSignal ShowDialoguePageSignal { get; set; }

        [Inject]
        public CloseDialoguePageSignal CloseDialoguePageSignal { get; set; }

        public override void OnRegister()
        {
            ShowDialoguePageSignal.AddListener(Show);
            CloseDialoguePageSignal.AddListener(Close);
            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {

            ShowDialoguePageSignal.RemoveListener(Show);
            CloseDialoguePageSignal.RemoveListener(Close);
            base.OnRemove();
        }

        private void Show()
        {
            view.Show();
        }

        private void Close()
        {
            view.Close();
        }
    }
}