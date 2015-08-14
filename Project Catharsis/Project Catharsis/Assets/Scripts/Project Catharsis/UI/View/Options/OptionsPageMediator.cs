using Catharsis.InputEditor;
using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class OptionsPageMediator : Mediator
    {
        [Inject]
        public OptionsPageView view { get; set; }

        [Inject]
        public ShowControlPageSignal ShowControlPageSignal { get; set; }

        [Inject]
        public ShowMainPageSignal ShowMainPageSignal { get; set; }



        public override void OnRegister()
        {
            view.ControlsButtonClickSignal.AddListener(OnControlButtonClick);
            view.BackButtonClickSignal.AddListener(OnBackButtonClick);
            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {
            view.ControlsButtonClickSignal.RemoveListener(OnControlButtonClick);
            view.BackButtonClickSignal.RemoveListener(OnBackButtonClick);

            view.Remove();
            base.OnRemove();
        }

        private void OnControlButtonClick()
        {
            ShowControlPageSignal.Dispatch();
        }

        private void OnBackButtonClick()
        {
            ShowMainPageSignal.Dispatch();
        }

    }
}