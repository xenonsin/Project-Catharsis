using Catharsis.DialogueEditor;
using Catharsis.DialogueEditor.Config;
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

                //Dialogue Signals
                injectionBinder.Bind<IDialogueSystem>().To<DialogueSystem>().ToSingleton();
                injectionBinder.Bind<DialogueLoadSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<DialogueStartSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<DialogueGenericEventSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<DialogueNodeCompleteSignal>().ToSingleton();
                injectionBinder.Bind<DialogueEndSignal>().ToSingleton();
                injectionBinder.Bind<DialogueContinueSignal>().ToSingleton();
                injectionBinder.Bind<DialogueMessageSignal>().ToSingleton().CrossContext();
                injectionBinder.Bind<DialogueOnSuddenlyEndedSignal>().ToSingleton();

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
 
            //Control Panel
            injectionBinder.Bind<ShowPauseMenuSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ShowControlPageSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ShowMainPageSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ClosePauseMenuSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<ShowOptionPageSignal>().ToSingleton().CrossContext();

            mediationBinder.Bind<PauseMenuView>().To<PauseMenuMediator>();
            mediationBinder.Bind<PausePanelMenuView>().To<PausePanelMenuMediator>();
            mediationBinder.Bind<OptionsPageView>().To<OptionsPageMediator>();
            mediationBinder.Bind<ControlPageView>().To<ControlPageMediator>();


            //Dialogue Panel
            injectionBinder.Bind<ShowDialoguePageSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<CloseDialoguePageSignal>().ToSingleton().CrossContext();

            mediationBinder.Bind<DialoguePanelView>().To<DialoguePanelMediator>();
            mediationBinder.Bind<DialoguePageView>().To<DialoguePageMediator>();

            //commands
            commandBinder.Bind<StartSignal>().To<UIStartCommand>();
            commandBinder.Bind<QuitGameSignal>().To<QuitGameCommand>(); //TODO: Might change this.
            commandBinder.Bind<PauseGameSignal>().To<PauseGameCommand>().Pooled();


            //mediators
            
        }

        protected override void postBindings()
        {
            base.postBindings();
        }
    }

}