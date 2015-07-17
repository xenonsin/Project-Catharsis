using Catharsis.DialogueEditor.Model;
using strange.extensions.signal.impl;

namespace Catharsis.DialogueEditor.Config
{
    /// <summary>
    /// This Signal is sent when the user initiates a dialogue, like clicking on an NPC.
    /// Params:
    /// int dialogue ID
    /// </summary>
    public class DialogueStartSignal : Signal<int> { }

    public class DialogueEndSignal : Signal { }

    /// <summary>
    /// This is called by the Dialogue GUI, when the user continues the dialogue or selects a choice if the dialogue is branching.
    /// Params:
    /// int choice
    /// </summary>
    public class DialogueContinueSignal : Signal<int> { }

    /// <summary>
    /// This is sent to the Dialogue GUI with information of the text.
    /// </summary>
    public class DialogueMessageSignal : Signal<DialogueMessage> {}

    /// <summary>
    /// This Signal is sent when the event node is called. 
    /// Params:
    /// string eventName 
    /// string data 
    /// </summary>
    public class DialogueGenericEventSignal : Signal<string, string> { }
}