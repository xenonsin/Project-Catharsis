using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Objects;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model
{
    public class DialogueEditorData
    {


        public List<DialogueEditorDialogueObject> dialogues;

        public int DialogueCount { get { return dialogues.Count; } }

        private int _currentDialogueId;

        public int CurrentDialogueId
        {
            get { return _currentDialogueId; }
            set
            {
                _currentDialogueId = Mathf.Clamp(value, 0, DialogueCount - 1);
            }
        }
        public DialogueEditorData()
        {
            dialogues = new List<DialogueEditorDialogueObject>();
        }


        public void Load()
        {
            
        }

        public void Save()
        {
            
        }

        //Adds Dialogue at the end of the list.
        //BUG: Currently this just gets the count then continues from there.. even if the id already exists.
        public void AddDialogue(int newDialogueCount, out int newCurrentID)
        {
            newCurrentID = 0;
            for (int i = 0; i < newDialogueCount; i += 1)
            {
                int num = dialogues.Count;
                //Debug.Log ("Adding Entry: "+num);
                dialogues.Add(new DialogueEditorDialogueObject());
                dialogues[num].id = num;
                newCurrentID = dialogues[num].id;
            }
        }

        //TODO: Find a way to get the correct selection index
        public void RemoveDialogue(int index, out int newCurrentID)
        {
            newCurrentID = (index - 1) < 0 ? 0: (index-1);
            dialogues.RemoveAt(index);

        }
    }
}