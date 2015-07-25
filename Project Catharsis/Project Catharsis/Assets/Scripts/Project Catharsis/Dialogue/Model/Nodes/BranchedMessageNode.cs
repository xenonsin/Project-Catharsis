using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.VariableEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public class BranchedMessageNode : MessageNode
    {
        public readonly List<string> choices;

        public BranchedMessageNode(string text, string charName, string animName, string audioName, string metadata, bool waitForResponse, float waitDuration, DialogueEditorWaitTypes waitType, Rect rect, List<int?> outs, List<string> choices = null) 
            : base(text, charName, animName, audioName, metadata, waitForResponse, waitDuration, waitType, rect, outs, choices)
        {
            this.choices = choices;
        }
    }
}