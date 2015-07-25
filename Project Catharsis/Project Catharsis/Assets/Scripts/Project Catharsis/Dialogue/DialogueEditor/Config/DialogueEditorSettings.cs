using UnityEngine;

namespace Catharsis.DialogueEditor.Config
{
    [System.Serializable]
    public class DialogueEditorSettings : ScriptableObject
    {
        public string lastScenarioName;

        public DialogueEditorSettings()
        {
            lastScenarioName = "New Scenario";
        }
    }
}