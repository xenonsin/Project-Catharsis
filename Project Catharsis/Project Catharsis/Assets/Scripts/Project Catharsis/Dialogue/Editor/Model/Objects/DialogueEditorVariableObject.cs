namespace Catharsis.DialogueEditor.Model.Objects
{
    [System.Serializable]
    public class DialogueEditorVariableObject
    {
        public string name;
        public string variable;
        public int id;

        public DialogueEditorVariableObject()
        {
            name = string.Empty;
            variable = string.Empty;
        }
    }
}