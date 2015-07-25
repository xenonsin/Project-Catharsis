using System.Collections.Generic;

namespace Catharsis.DialogueEditor.Model
{
    public class DialogueGlobalVariables
    {
         public List<bool> booleans;
		public List<float> floats;
		public List<string> strings;
		
		public DialogueGlobalVariables(){
			this.booleans = new List<bool>();
			this.floats = new List<float>();
			this.strings = new List<string>();
		}
		
		public DialogueGlobalVariables(List<bool> booleans, List<float> floats, List<string> strings){
			this.booleans = booleans;
			this.floats = floats;
			this.strings = strings;
		}
    }
}