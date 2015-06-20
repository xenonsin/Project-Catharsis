using Catharsis.InputEditor;
using Catharsis.UI.Control;
using UnityEngine;

namespace Catharsis.UI
{
    public class UIContext : SignalContext
    {
        public UIContext(MonoBehaviour contextView) : base(contextView){}

        protected override void mapBindings()
        {
            base.mapBindings();


            if (firstContext == this)
            {
                //Input Manager Signals                
                injectionBinder.Bind<InputManagerSavedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerConfigurationChangedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerConfigurationDirtySignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerRemoteUpdateSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerLoadedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerLoadDefaultInputSignal>().ToSingleton().CrossContext();

                //Input Manager Commands
                commandBinder.Bind<InputManagerLoadUserInputSignal>().To<LoadInputCommand>().Pooled();
                commandBinder.Bind<InputManagerSaveSignal>().To<SaveInputCommand>().Pooled();
                //commandBinder.Bind<InputManagerLoadedSignal>().To<StartInputManagerCommand>().Pooled();
                commandBinder.Bind<InputManagerLoadDefaultInputSignal>().To<LoadDefaultInputCommand>().Pooled();

                //Common Signals
                injectionBinder.Bind<GameInputSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<GameStartSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<GameEndSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<QuitApplicationSignal>().ToSingleton().CrossContext();

            }

            //Signals with no commands.    
            injectionBinder.Bind<ShowPauseMenuSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ShowControlPageSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ShowMainPageSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ClosePauseMenuSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ShowOptionPageSignal>().ToSingleton().CrossContext();

            //commands
            commandBinder.Bind<StartSignal>().To<UIStartCommand>();
            commandBinder.Bind<QuitGameSignal>().To<QuitGameCommand>(); //TODO: Might change this.
            commandBinder.Bind<PauseGameSignal>().To<PauseGameCommand>().Pooled();


            //mediators
            mediationBinder.Bind<PauseMenuView>().To<PauseMenuMediator>();
            mediationBinder.Bind<PausePanelMenuView>().To<PausePanelMenuMediator>();
            mediationBinder.Bind<OptionsPageView>().To<OptionsPageMediator>();
            mediationBinder.Bind<ControlPageView>().To<ControlPageMediator>();
        }

        protected override void postBindings()
        {
            base.postBindings();
        }
    }

}