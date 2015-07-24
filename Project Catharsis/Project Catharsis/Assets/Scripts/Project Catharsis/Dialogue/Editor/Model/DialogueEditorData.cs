using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Objects;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model
{
    [System.Serializable]
    public class DialogueEditorData
    {
        public string scenarioName;

        public List<DialogueEditorDialogueObject> dialogues;
        public DialogueEditorGlobalVariablesContainer globals;
        public List<int> abandonedIds; 
        public int DialogueCount { get { return dialogues.Count; } }

        private int _currentDialogueId;


        public DialogueEditorData()
        {
            scenarioName = "newScenario";
            dialogues = new List<DialogueEditorDialogueObject>();
            globals = new DialogueEditorGlobalVariablesContainer();
            abandonedIds = new List<int>();
        }


        //Adds Dialogue at the end of the list.
        //BUG: Currently this just gets the count then continues from there.. even if the id already exists.
        public void AddDialogue(int newDialogueCount, out int newCurrentID)
        {
            newCurrentID = 0;
            for (int i = 0; i < newDialogueCount; i += 1)
            {
                DialogueEditorDialogueObject newDialogueObject = new DialogueEditorDialogueObject();
                newDialogueObject.id = GetID();
                //Debug.Log ("Adding Entry: "+num);
                dialogues.Add(newDialogueObject);

                newCurrentID = dialogues.Count - 1;
            }
        }

        //TODO: Find a way to get the correct selection index
        public void RemoveDialogue(int index, out int newCurrentID)
        {
            newCurrentID = (index - 1) < 0 ? 0: (index-1);
            abandonedIds.Add(dialogues[index].id);
            dialogues.RemoveAt(index);
            

            abandonedIds.Sort();

        }

        private int GetID()
        {
            
            for (int i = 0; i < abandonedIds.Count; i++)
            {
 
                int id = 0;
                if (abandonedIds[i] <= dialogues.Count)
                {
                    id = abandonedIds[i];
                    abandonedIds.RemoveAt(i);
                    return id;
                }
            }

            return dialogues.Count;
        }
    }
}