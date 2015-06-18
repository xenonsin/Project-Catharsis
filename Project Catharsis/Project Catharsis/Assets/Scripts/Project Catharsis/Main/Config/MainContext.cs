using Catharsis.InputEditor;
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
                injectionBinder.Bind<InputManagerLoadedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerSavedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerConfigurationChangedSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerConfigurationDirtySignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<InputManagerRemoteUpdateSignal>().ToSingleton().CrossContext();

                //Common Signals
                injectionBinder.Bind<GameInputSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<GameStartSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<GameEndSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<QuitApplicationSignal>().ToSingleton().CrossContext();

            }

            commandBinder.Bind<StartSignal>().To<MainStartupCommand>();
        }

    }
}