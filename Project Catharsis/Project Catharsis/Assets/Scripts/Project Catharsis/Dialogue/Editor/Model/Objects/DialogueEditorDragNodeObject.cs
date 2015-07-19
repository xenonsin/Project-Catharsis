using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Objects
{
    public class DialogueEditorDragNodeObject
    {
        public int nodeId;
		public Vector2 mouseOffset;

        public DialogueEditorDragNodeObject(int nodeId, Vector2 mouseOffset)
        {
			this.nodeId = nodeId;
			this.mouseOffset = mouseOffset;
		}
    }
}