using Catharsis.InputEditor;
using Catharsis.InputEditor.UI;
using Catharsis.Main.Controller;
using UnityEngine;
using strange.extensions.context.impl;
namespace Catharsis.Main.Config
{
    public class MainContext : SignalContext
    {
        public MainContext(MonoBehaviour contextView) : base(contextView) { }

        protected override void mapBindings()
        {
            base.mapBindings();

            if (Context.firstContext == this)
            {
                //Input Manager Signals                
                injectionBinder.Bind<InputManagerSavedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerConfigurationChangedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerConfigurationDirtySignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerRemoteUpdateSignal>().ToSingleton().CrossContext();

                //Input Manager Commands
                commandBinder.Bind<InputManagerLoadUserInputSignal>().To<LoadInputCommand>().Pooled();
                commandBinder.Bind<InputManagerSaveSignal>().To<SaveInputCommand>().Pooled();
                commandBinder.Bind<InputManagerLoadedSignal>().To<StartInputManagerCommand>().Pooled();
                commandBinder.Bind<InputManagerLoadDefaultInputSignal>().To<LoadDefaultInputCommand>().Pooled();

                //Common Signals
                injectionBinder.Bind<GameInputSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<GameStartSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<GameEndSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<QuitApplicationSignal>().ToSingleton().CrossContext();

            }
            //injectionBinder.injector.Inject(StandaloneInputModule);
            commandBinder.Bind<StartSignal>().To<MainStartupCommand>();
        }

    }
}