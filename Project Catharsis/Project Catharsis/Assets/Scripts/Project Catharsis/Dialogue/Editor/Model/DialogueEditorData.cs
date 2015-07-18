using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Objects;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model
{
    [System.Serializable]
    public class DialogueEditorData
    {


        public List<DialogueEditorDialogueObject> dialogues;

        public int count { get { return dialogues.Count; } }

        private int _currentDialogueId;

        public int CurrentDialogueId
        {
            get { return _currentDialogueId; }
            set
            {
                _currentDialogueId = Mathf.Clamp(value, 0, count - 1);
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

        public void RemoveDialogue(int removeCount)
        {
            if (count < 1) return;

            for (int i = 0; i < removeCount; i += 1)
            {
                int num = dialogues.Count - 1;
                //Debug.Log ("Removing Entry: "+num);
                dialogues.RemoveAt(num);
            }

            CurrentDialogueId = CurrentDialogueId;
        }
    }
}