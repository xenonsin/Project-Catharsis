using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public class EmptyNode : DialogueNode
    {
        public EmptyNode(): base(null)
        {
			Debug.LogWarning("Something went wrong, phase is EmptyPhase");
		}
    }
}