using System.Collections.Generic;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Objects
{
    [System.Serializable]
    public class DialogueEditorDialogueObject
    {
        public int id;
        public string name;
        public int? startPage;
        public Vector2 scrollPosition;
        public List<DialogueEditorNodeObject> nodes;
        public DialogueEditorVariablesContainer floats;
        public DialogueEditorVariablesContainer strings;
        public DialogueEditorVariablesContainer booleans;

        public DialogueEditorDialogueObject()
        {
            name = "sup";
            nodes = new List<DialogueEditorNodeObject>();
            floats = new DialogueEditorVariablesContainer();
            strings = new DialogueEditorVariablesContainer();
            booleans = new DialogueEditorVariablesContainer();
        }

        public void AddNode(DialogueEditorNodeTypes nodeType, Vector2 newNodePosition)
        {
            switch (nodeType)
            {

                case DialogueEditorNodeTypes.MessageNode:
                    nodes.Add(DialogueEditorNodeTemplates.NewMessageNode(nodes.Count));
                    break;

                case DialogueEditorNodeTypes.BranchingMessageNode:
                    nodes.Add(DialogueEditorNodeTemplates.NewBranchedMessageNode(nodes.Count));
                    break;

                case DialogueEditorNodeTypes.SetVariableNode:
                    nodes.Add(DialogueEditorNodeTemplates.NewSetVariableNode(nodes.Count));
                    break;

                case DialogueEditorNodeTypes.ConditionalNode:
                    nodes.Add(DialogueEditorNodeTemplates.NewConditionalNode(nodes.Count));
                    break;

                case DialogueEditorNodeTypes.GenericEventNode:
                    nodes.Add(DialogueEditorNodeTemplates.NewEventNode(nodes.Count));
                    break;

                case DialogueEditorNodeTypes.EndNode:
                    nodes.Add(DialogueEditorNodeTemplates.NewEndNode(nodes.Count));
                    break;
            }

            nodes[nodes.Count - 1].position = newNodePosition;
        }

        public void RemoveNode(int id)
        {
            for (int p = 0; p < nodes.Count; p += 1)
            {
                DialogueEditorNodeObject node = nodes[p];

                for (int o = 0; o < node.outs.Count; o += 1)
                {
                    if (node.outs[o].HasValue && node.outs[o] >/*=*/ id)
                    {
                        node.outs[o] -= 1;
                    }
                    else if (node.outs[o].HasValue && node.outs[o] == id)
                    {
                        node.outs[o] = null;
                    }

                }

                if (startPage.HasValue && startPage == id)
                {
                    startPage = null;
                }

                if (p > id)
                {
                    node.id -= 1;
                }
            }
            nodes.RemoveAt(id);
        }
    }
}