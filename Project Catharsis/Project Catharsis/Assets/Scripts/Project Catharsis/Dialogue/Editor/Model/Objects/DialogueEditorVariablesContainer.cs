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
        public int selection;

        public DialogueEditorVariablesContainer()
        {
            selection = 0;
            variables = new List<DialogueEditorVariableObject>();
        }

        public void addVariable()
        {
            int count = variables.Count;
            variables.Add(new DialogueEditorVariableObject());
            variables[count].id = count;
            selection = variables.Count - 1;
        }

        public void removeVariable()
        {
            if (variables.Count < 1) return;
            variables.RemoveAt(variables.Count - 1);
            if (selection > (variables.Count - 1)) selection = variables.Count - 1;
        }
    }
}