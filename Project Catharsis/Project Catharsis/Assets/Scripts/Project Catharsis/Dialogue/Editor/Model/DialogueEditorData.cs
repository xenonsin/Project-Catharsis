using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Objects;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model
{
    [System.Serializable]
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
        //Allow to reorder?
        public void AddDialogue(int newDialogueCount)
        {
            for (int i = 0; i < newDialogueCount; i += 1)
            {
                int num = dialogues.Count;
                //Debug.Log ("Adding Entry: "+num);
                dialogues.Add(new DialogueEditorDialogueObject());
                dialogues[num].id = num;
                CurrentDialogueId = dialogues[num].id;
            }
        }

        //TODO: Find a way to get the correct selection index
        public void RemoveDialogue(int index)
        {

            dialogues.RemoveAt(index);
            
            //?
            CurrentDialogueId = CurrentDialogueId;
        }
    }
}