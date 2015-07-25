namespace Catharsis.DialogueEditor.Model.Objects
{
    public class DialogueEditorSelectionObject
    {
        public int nodeId { get; private set; }
        public int outputIndex { get; private set; }
        public bool isStart { get; private set; }

        public DialogueEditorSelectionObject(int nodeId, int outputIndex)
        {
            if (nodeId < 0) nodeId = 0;
            if (outputIndex < 0) outputIndex = 0;
            this.nodeId = nodeId;
            this.outputIndex = outputIndex;
            this.isStart = false;
        }

        public DialogueEditorSelectionObject(bool isStart)
        {
            this.isStart = true;
            this.nodeId = int.MinValue;
            this.outputIndex = int.MinValue;
        }
    }
}