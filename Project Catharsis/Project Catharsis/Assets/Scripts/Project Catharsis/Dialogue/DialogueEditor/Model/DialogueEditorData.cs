using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Nodes;
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
        private List<int> abandonedIds; 
        public int DialogueCount { get { return dialogues.Count; } }


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

        public DialogueData GetDialogueData()
        {

            #region Global Variables
            List<bool> globalBooleans = new List<bool>();
            List<float> globalFloats = new List<float>();
            List<string> globalStrings = new List<string>();

            for (int i = 0; i < globals.booleans.variables.Count; i += 1)
            {
                bool parsedBoolean;
                bool success = bool.TryParse(globals.booleans.variables[i].variable, out parsedBoolean);
                if (!success) Debug.LogWarning("Global Boolean " + i + " did not parse correc tly, defaulting to false");
                globalBooleans.Add(parsedBoolean);
            }

            for (int i = 0; i < globals.floats.variables.Count; i += 1)
            {
                float parsedFloat;
                bool success = float.TryParse(globals.floats.variables[i].variable, out parsedFloat);
                if (!success) Debug.LogWarning("Global Float " + i + " did not parse correctly, defaulting to 0");
                globalFloats.Add(parsedFloat);
            }

            for (int i = 0; i < globals.strings.variables.Count; i += 1)
            {
                globalStrings.Add(globals.strings.variables[i].variable);
            }

            DialogueGlobalVariables newGlobalVariables = new DialogueGlobalVariables(globalBooleans, globalFloats, globalStrings);
            #endregion

            #region Dialogues
            List<Dialogue> newDialogues = new List<Dialogue>();

            // Loop through Dialogues
            for (int d = 0; d < this.dialogues.Count; d += 1)
            {
                DialogueEditorDialogueObject dialogue = dialogues[d];

                #region Dialogue Phases
                List<DialogueNode> newNodes = new List<DialogueNode>();
                // Loop through phases
                for (int p = 0; p < dialogue.nodes.Count; p += 1)
                {
                    DialogueEditorNodeObject node = dialogue.nodes[p];

                    switch (node.type)
                    {

                        case DialogueEditorNodeTypes.MessageNode:
                            newNodes.Add(new MessageNode(node.text, node.characterName, node.animName, node.audioName, node.metadata, node.waitForResponse, node.waitDuration, node.waitType, node.rect, node.outs));
                            break;

                        case DialogueEditorNodeTypes.BranchingMessageNode:
                            newNodes.Add(new BranchedMessageNode(node.text, node.characterName, node.animName, node.audioName, node.metadata, node.waitForResponse, node.waitDuration, node.waitType, node.rect, node.outs));
                            break;

                        case DialogueEditorNodeTypes.SetVariableNode:
                            newNodes.Add(new SetVariableNode(node.variableScope, node.variableType, node.variableId, node.variableSetEquation, node.variableSetValue, node.outs));
                            break;

                        case DialogueEditorNodeTypes.ConditionalNode:
                            newNodes.Add(new ConditionalNode(node.variableScope, node.variableType, node.variableId, node.variableGetEquation, node.variableGetValue, node.outs));
                            break;

                        case DialogueEditorNodeTypes.GenericEventNode:
                            newNodes.Add(new GenericEventNode(node.eventName, node.metadata, node.outs));
                            break;

                        case DialogueEditorNodeTypes.EndNode:
                            newNodes.Add(new EndNode());
                            break;

                        default:
                            newNodes.Add(new EmptyNode());
                            break;

                    }
                }
                #endregion

                #region Dialogue Variables
                //Booleans
                List<bool> localBooleans = new List<bool>();
                for (int i = 0; i < dialogue.booleans.variables.Count; i += 1)
                {
                    bool newBoolean;
                    bool success = bool.TryParse(dialogue.booleans.variables[i].variable, out newBoolean);
                    if (!success) Debug.Log("Dialogue " + d + ": Boolean " + i + " not formatted correctly. Defaulting to false");
                    localBooleans.Add(newBoolean);
                }

                //Floats
                List<float> localFloats = new List<float>();
                for (int i = 0; i < dialogue.floats.variables.Count; i += 1)
                {
                    float newFloat;
                    bool success = float.TryParse(dialogue.floats.variables[i].variable, out newFloat);
                    if (!success) Debug.Log("Dialogue " + d + ": Float " + i + " not formatted correctly. Defaulting to 0");
                    localFloats.Add(newFloat);
                }

                //Strings
                List<string> localStrings = new List<string>();
                for (int i = 0; i < dialogue.strings.variables.Count; i += 1)
                {
                    localStrings.Add(dialogue.strings.variables[i].variable);
                }

                DialogueVariables localVariables = new DialogueVariables(localBooleans, localFloats, localStrings);
                #endregion

                Dialogue newDialogue = new Dialogue(dialogue.dialogueName, dialogue.startPage.Value, localVariables, newNodes);
                //Debug.Log(newDialogue.ToString());
                newDialogues.Add(newDialogue);
            }
            #endregion

            DialogueData newData = new DialogueData(newGlobalVariables, newDialogues);
            return newData;
        }
    }
}