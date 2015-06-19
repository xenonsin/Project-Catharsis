using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class PausePanelMenuMediator : Mediator
    {
        [Inject]
        public PausePanelMenuView view { get; set; }


        [Inject]
        public ShowOptionPageSignal ShowOptionPageSignal { get; set; }

        [Inject]
        public ClosePauseMenuSignal ClosePauseMenuSignal { get; set; }
        [Inject]
        public QuitGameSignal QuitGameSignal { get; set; }

        public override void OnRegister()
        {

            view.OptionsButtonClickSignal.AddListener(OnControlButtonClick);
            view.QuitButtonClickSignal.AddListener(OnQuickButtonClick);
            view.ResumeButtonClickSignal.AddListener(OnResumeButtonClick);

            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {

            view.OptionsButtonClickSignal.RemoveListener(OnControlButtonClick);
            view.QuitButtonClickSignal.RemoveListener(OnQuickButtonClick);
            view.ResumeButtonClickSignal.RemoveListener(OnResumeButtonClick);

            view.Remove();
            base.OnRemove();
        }

        private void OnControlButtonClick()
        {
            ShowOptionPageSignal.Dispatch();
        }

        private void OnQuickButtonClick()
        {
            QuitGameSignal.Dispatch();
        }

        private void OnResumeButtonClick()
        {
            ClosePauseMenuSignal.Dispatch();
        }
    }
}