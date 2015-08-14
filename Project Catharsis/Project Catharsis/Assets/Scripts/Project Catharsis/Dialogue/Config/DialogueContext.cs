using strange.extensions.context.impl;
using UnityEngine;

namespace Catharsis.DialogueEditor.Config
{
    public class DialogueContext : SignalContext
    {
        public DialogueContext(MonoBehaviour contextView) : base(contextView) { }

        protected override void mapBindings()
        {
            commandBinder.Bind<StartSignal>().To<DialogueManagerStartCommand>().Once();

            injectionBinder.Bind<IDialogueSystem>().To<DialogueSystem>().ToSingleton();

            //This is called when a user loads a scenerio.
            injectionBinder.Bind<DialogueLoadSignal>().ToSingleton().CrossContext();
            //This is called when a user clicks an npc.. during the game context
            injectionBinder.Bind<DialogueStartSignal>().ToSingleton().CrossContext();
            //This is called by the dialogue manager to signal other listening components
            injectionBinder.Bind<DialogueGenericEventSignal>().ToSingleton().CrossContext();
            //This is called when a node ends.
            injectionBinder.Bind<DialogueNodeCompleteSignal>().ToSingleton();
            //This is called when the dialogue ends.
            injectionBinder.Bind<DialogueEndSignal>().ToSingleton();
            //This is called when wanting to go to next dialogue.
            injectionBinder.Bind<DialogueContinueSignal>().ToSingleton();
            //This is called by the Dialogue Manager whenever it's a message node.
            injectionBinder.Bind<DialogueMessageSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<DialogueOnSuddenlyEndedSignal>().ToSingleton();
            

            base.mapBindings();
        }

        protected override void postBindings()
        {
            base.postBindings();
        }
    }
}