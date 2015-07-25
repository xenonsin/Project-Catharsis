using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Data;
using Catharsis.DialogueEditor.Model.VariableEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public class MessageNode : DialogueNode
    {
        public readonly TextData data;
        //MessageNode(node.text, node.theme, node.characterName, node.animName, node.audioName, node.metadata, node.waitForResponse, node.waitDuration,node.waitType, node.rect, node.outs)
        public MessageNode(string text, string charName, string animName, string audioName, string metadata,
            bool waitForResponse, float waitDuration, DialogueEditorWaitTypes waitType, Rect rect, List<int?> outs,
            List<string> choices = null) : base(outs)
        {
            data = new TextData(text, charName, animName, audioName, metadata, waitForResponse, waitDuration, waitType, rect, choices);
        }

        protected override void OnStart()
        {
            // Override and do nothing, wait for user to "continue" dialogue
            //TODO: Need to account for waitforresponse
        }

        public override void Continue(int nextNodeId)
        {
            base.Continue(nextNodeId);
            state = NodeState.Complete;
        }
    }
}