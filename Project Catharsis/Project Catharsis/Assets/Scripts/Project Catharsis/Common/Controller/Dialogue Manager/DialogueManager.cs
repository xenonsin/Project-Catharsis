using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Model;
using Catharsis.DialogueEditor.Model.Nodes;

namespace Catharsis.DialogueEditor
{
    //TODO: Find a way to serialize the dialogue data....
    public class DialogueManager : IDialogueManager
    {
        private DialogueNode currentNode;
        private Dialogue currentDialogue;

        [Inject]
        public DialogueStartSignal StartSignal { get; set; }

        [Inject]
        public DialogueContinueSignal ContinueSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            StartSignal.AddListener(StartDialogue);
            ContinueSignal.AddListener(ContinueDialogue);
        }

        private void StartDialogue(int dialogueID)
        {
            
        }

        private void ContinueDialogue(int choice)
        {
            
        }
    }
}