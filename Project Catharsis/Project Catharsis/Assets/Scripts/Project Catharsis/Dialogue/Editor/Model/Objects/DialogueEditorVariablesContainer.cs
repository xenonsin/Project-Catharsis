using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Catharsis.DialogueEditor.Model.Objects
{
    [System.Serializable]
    public class DialogueEditorVariablesContainer
    {
        //This is a generic version of a dictionary. Saves the id and the value both as strings.
        //Still not sure how this is used.

        public List<DialogueEditorVariableObject> variables;
        public List<int> abandonedIds; 
        public int selection;

        public DialogueEditorVariablesContainer()
        {
            selection = 0;
            variables = new List<DialogueEditorVariableObject>();
            abandonedIds = new List<int>();
        }

        public void addVariable()
        {
            DialogueEditorVariableObject newVariableObject = new DialogueEditorVariableObject();
            newVariableObject.id = GetId();

            variables.Add(newVariableObject);

            selection = variables.Count - 1;
        }

        public void removeVariable()
        {

            if (variables.Count < 1) return;
            abandonedIds.Add(variables[selection].id);
            variables.RemoveAt(selection);

            abandonedIds.Sort();

            if (selection > (variables.Count - 1)) selection = variables.Count - 1;
        }

        private int GetId()
        {
            for (int i = 0; i < abandonedIds.Count; i++)
            {
                int id = 0;
                if (abandonedIds[i] <= variables.Count)
                {
                    id = abandonedIds[i];
                    abandonedIds.RemoveAt(i);
                    return id;
                }
            }

            return variables.Count;
        }
    }
}