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

        [Inject]
        public DialogueLoadSignal LoadSignal { get; set; }

        [Inject]
        public DialogueStartSignal StartSignal { get; set; }

        [Inject]
        public DialogueContinueSignal ContinueSignal { get; set; }

        

        [PostConstruct]
        public void PostConstruct()
        {
            LoadSignal.AddListener(LoadScenario);
            StartSignal.AddListener(StartDialogue);
            ContinueSignal.AddListener(ContinueDialogue);
        }

        private void LoadScenario(string scenarioPath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(DialogueEditorData));
            XmlReader xmlReader = XmlReader.Create(new StringReader((Resources.Load("dialoguer_data") as TextAsset).text));
            DialogueEditorData editorData = (DialogueEditorData)deserializer.Deserialize(xmlReader);

            _data = editorData.getDialogueData();
        }

        private void StartDialogue(int dialogueID)
        {
            
        }

        private void ContinueDialogue(int choice)
        {
            
        }
    }
}