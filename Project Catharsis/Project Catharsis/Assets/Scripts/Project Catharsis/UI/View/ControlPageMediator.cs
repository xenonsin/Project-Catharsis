using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class ControlPageMediator : Mediator
    {
         [Inject]
        public ControlPageView view { get; set; }

        [Inject]
         public ShowMainPageSignal ShowMainPageSignal { get; set; }


        //TODO: create a default command signal
        public override void OnRegister()
        {
            view.BackButtonClickSignal.AddListener(Back);

            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {
            view.BackButtonClickSignal.RemoveListener(Back);
            view.Remove();
            base.OnRemove();
        }

        private void Back()
        {
            ShowMainPageSignal.Dispatch();
        }
    }
}