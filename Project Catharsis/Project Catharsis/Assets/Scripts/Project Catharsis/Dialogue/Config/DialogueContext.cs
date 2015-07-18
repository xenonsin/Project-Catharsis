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

            //This is called when a user clicks an npc.. during the game context
            injectionBinder.Bind<DialogueStartSignal>().ToSingleton().CrossContext();
            //This is called by the dialogue manager to signal other listening components
            injectionBinder.Bind<DialogueGenericEventSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<DialogueEndSignal>().ToSingleton();
            injectionBinder.Bind<DialogueContinueSignal>().ToSingleton();
            injectionBinder.Bind<DialogueMessageSignal>().ToSingleton();
            

            base.mapBindings();
        }

        protected override void postBindings()
        {
            base.postBindings();
        }
    }
}