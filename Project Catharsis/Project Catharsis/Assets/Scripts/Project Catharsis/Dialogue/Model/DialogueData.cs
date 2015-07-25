using System.Collections.Generic;

namespace Catharsis.DialogueEditor.Model
{
    public class DialogueData
    {
        public DialogueGlobalVariables globalVariables;
        public List<Dialogue> dialogues;

        public DialogueData(DialogueGlobalVariables globalVariables, List<Dialogue> dialogues){
			this.globalVariables = globalVariables;
			this.dialogues = dialogues;

		}
    }
}