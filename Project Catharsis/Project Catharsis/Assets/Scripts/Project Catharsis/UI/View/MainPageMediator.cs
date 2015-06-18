using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class MainPageMediator : Mediator
    {
        [Inject]
        public MainPageView view { get; set; }

        [Inject]
        public ShowControlPageSignal ShowControlPageSignal { get; set; }

        [Inject]
        public ClosePauseMenuSignal ClosePauseMenuSignal { get; set; }
        [Inject]
        public QuitGameSignal QuitGameSignal { get; set; }

        public override void OnRegister()
        {       
            view.ControlButtonClickSignal.AddListener(OnControlButtonClick);
            view.QuitButtonClickSignal.AddListener(OnQuickButtonClick);
            view.ResumeButtonClickSignal.AddListener(OnResumeButtonClick);

            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {
            view.ControlButtonClickSignal.RemoveListener(OnControlButtonClick);
            view.QuitButtonClickSignal.RemoveListener(OnQuickButtonClick);
            view.ResumeButtonClickSignal.RemoveListener(OnResumeButtonClick);

            view.Remove();
            base.OnRemove();
        }

        private void OnControlButtonClick()
        {
            ShowControlPageSignal.Dispatch();
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