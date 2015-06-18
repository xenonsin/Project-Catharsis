using Catharsis.UI.Control;
using UnityEngine;

namespace Catharsis.UI
{
    public class UIContext : SignalContext
    {
        public UIContext(MonoBehaviour contextView) : base(contextView)
        {
        }

        protected override void mapBindings()
        {
            base.mapBindings();


            //Signals with no commands.    
            injectionBinder.Bind<ShowPauseMenuSignal>().ToSingleton();
            injectionBinder.Bind<ShowControlPageSignal>().ToSingleton();
            injectionBinder.Bind<ShowMainPageSignal>().ToSingleton();
            injectionBinder.Bind<ClosePauseMenuSignal>().ToSingleton();

            //commands
            commandBinder.Bind<StartSignal>().To<UIStartCommand>();
            commandBinder.Bind<QuitGameSignal>().To<QuitGameCommand>(); //TODO: Might change this.
            commandBinder.Bind<PauseGameSignal>().To<PauseGameCommand>().Pooled();


            //mediators
            mediationBinder.Bind<PauseMenuView>().To<PauseMenuMediator>();
            mediationBinder.Bind<MainPageView>().To<MainPageMediator>();
            mediationBinder.Bind<ControlPageView>().To<ControlPageMediator>();
        }

        protected override void postBindings()
        {
            base.postBindings();
        }
    }

}