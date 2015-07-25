namespace Catharsis.DialogueEditor.Model.Objects
{
    [System.Serializable]
    public class DialogueEditorVariableObject
    {
        public string variableName;
        public string variable;
        public int id;

        public DialogueEditorVariableObject()
        {
            variableName = string.Empty;
            variable = string.Empty;
        }
    }
}