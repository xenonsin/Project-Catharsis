using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Model;
using Catharsis.DialogueEditor.Model.Nodes;
using UnityEngine;

namespace Catharsis.DialogueEditor
{
    //TODO: Find a way to serialize the dialogue data....
    public class DialogueSystem : IDialogueSystem
    {
        //private DialogueNode currentNode;
        //private Dialogue currentDialogue;

        private DialogueData _data;

        private DialogueNode currentNode;
        private Dialogue currentDialogue;

        [Inject]
        public DialogueLoadSignal LoadSignal { get; set; }

        [Inject]
        public DialogueStartSignal StartSignal { get; set; }

        [Inject]
        public DialogueContinueSignal ContinueSignal { get; set; }

        [Inject]
        public DialogueMessageSignal MessageSignal { get; set; }

        [Inject]
        public DialogueEndSignal EndSignal { get; set; }

        [Inject]
        public DialogueOnSuddenlyEndedSignal OnSuddenlyEndedSignal { get; set; }

        [Inject]
        public DialogueNodeCompleteSignal NodeCompleteSignal { get; set; }

        

        [PostConstruct]
        public void PostConstruct()
        {
            LoadSignal.AddListener(LoadScenario);
            StartSignal.AddListener(StartDialogue);
            ContinueSignal.AddListener(ContinueDialogue);
            NodeCompleteSignal.AddListener(NodeComplete);
        }

        //Called by a signal
        private void LoadScenario(string scenarioPath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(DialogueEditorData));
            XmlReader xmlReader = XmlReader.Create(new StringReader((Resources.Load(scenarioPath) as TextAsset).text));
            DialogueEditorData editorData = (DialogueEditorData)deserializer.Deserialize(xmlReader);
            _data = editorData.GetDialogueData();
 
        }

        //Called by a signal
        private void StartDialogue(int dialogueID)
        {
            if (currentDialogue != null) 
                OnSuddenlyEndedSignal.Dispatch();


            currentDialogue = GetDialogueById(dialogueID);
            currentDialogue.Reset();

            SetUpNode(currentDialogue.startNodeid);
        }

        //Called bya signal
        private void ContinueDialogue(int choice)
        {
            currentNode.Continue(choice);
        }

        private void EndDialogue()
        {
            EndSignal.Dispatch();

            currentDialogue.Reset();
            Reset();
        }

        private void SetUpNode(int nextNodeId)
        {
            if (currentDialogue == null) return;

            DialogueNode node = currentDialogue.nodes[nextNodeId];

            if (node is EndNode)
            {
                EndDialogue();
                return;
            }

            if (node is MessageNode || node is BranchedMessageNode)
                MessageSignal.Dispatch((node as MessageNode).data);
            

            currentNode = node;
            node.Start(currentDialogue.LocalVariables);
        }

        private void NodeComplete(int nextNodeId)
        {
            SetUpNode(nextNodeId);
        }

        private void Reset()
        {
            currentNode = null;
            currentDialogue = null;
        }

        public Dialogue GetDialogueById(int dialogueId)
        {
            if (_data.dialogues.Count <= dialogueId)
            {
                Debug.LogWarning("Dialogue [" + dialogueId + "] does not exist.");
                return null;
            }

            return _data.dialogues[dialogueId];
        }
    }
}