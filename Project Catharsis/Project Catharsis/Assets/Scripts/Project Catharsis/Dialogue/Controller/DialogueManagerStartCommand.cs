using Catharsis.DialogueEditor.Config;
using strange.extensions.command.impl;
using UnityEngine;

namespace Catharsis.DialogueEditor
{
    public class DialogueManagerStartCommand : Command
    {
        [Inject]
        public IDialogueSystem DialogueSystem { get; set; }
        //Debug
        [Inject]
        public DialogueLoadSignal LoadSignal { get; set; }

        [Inject]
        public DialogueStartSignal StartSignal { get; set; }
        public override void Execute()
        {
            Debug.Log("Dialogue Manager started.");
            LoadSignal.Dispatch("testScenario");
            StartSignal.Dispatch(0);
        }
    }
}