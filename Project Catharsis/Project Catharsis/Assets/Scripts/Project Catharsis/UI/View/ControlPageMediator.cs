using Catharsis.InputEditor;
using strange.extensions.mediation.impl;

namespace Catharsis.UI
{
    public class ControlPageMediator : Mediator
    {
         [Inject]
        public ControlPageView view { get; set; }

        [Inject]
         public ShowMainPageSignal ShowMainPageSignal { get; set; }

        [Inject]
        public InputManagerLoadedSignal LoadedSignal { get; set; }

        [Inject]
        public InputManagerConfigurationDirtySignal DirtySignal { get; set; }

        [Inject]
        public IInputManager InputManager { get; set; }


        //TODO: create a default command & signal
        public override void OnRegister()
        {          
            view.BackButtonClickSignal.AddListener(Back);
            LoadedSignal.AddListener(InputManagerDoneLoading);
            DirtySignal.AddListener(InputManagerHandleDirty);
            view.Init(InputManager);
            base.OnRegister();
        }

        public override void OnRemove()
        {
            view.BackButtonClickSignal.RemoveListener(Back);
            LoadedSignal.RemoveListener(InputManagerDoneLoading);
            DirtySignal.RemoveListener(InputManagerHandleDirty);


            view.Remove();
            base.OnRemove();
        }

        private void Back()
        {
            ShowMainPageSignal.Dispatch();
        }

        private void InputManagerDoneLoading()
        {
            view.Loaded();
        }

        private void InputManagerHandleDirty(string name)
        {
            view.HandleDirty(name);
        }
    }
}