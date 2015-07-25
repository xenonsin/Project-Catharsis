using Catharsis.DialogueEditor.Model;
using Catharsis.DialogueEditor.Model.Data;
using strange.extensions.signal.impl;

namespace Catharsis.DialogueEditor.Config
{
    /// <summary>
    /// This signal is called to load a specific scenario.
    /// </summary>
    public class DialogueLoadSignal : Signal<string> { }

    /// <summary>
    /// This Signal is sent when the user initiates a dialogue, like clicking on an NPC.
    /// Params:
    /// int dialogue ID
    /// </summary>
    public class DialogueStartSignal : Signal<int> { }

    public class DialogueOnStartedSignal : Signal { }

    public class DialogueOnSuddenlyEndedSignal : Signal { }

    public class DialogueOnEndedSignal : Signal { }

    public class DialogueEndSignal : Signal { }

    /// <summary>
    /// This Signal is called when a node has completed
    /// Params:
    /// int nextNode id;
    /// </summary>
    public class DialogueNodeCompleteSignal : Signal<int> { }

    /// <summary>
    /// This is called by the Dialogue GUI, when the user continues the dialogue or selects a choice if the dialogue is branching.
    /// Params:
    /// int choice
    /// </summary>
    public class DialogueContinueSignal : Signal<int> { }

    /// <summary>
    /// This is sent to the Dialogue GUI with information of the text.
    /// </summary>
    public class DialogueMessageSignal : Signal<TextData> { }

    /// <summary>
    /// This Signal is sent when the event node is called. 
    /// Params:
    /// string eventName 
    /// string data 
    /// </summary>
    public class DialogueGenericEventSignal : Signal<string, string> { }
}