using System.Collections.Generic;

namespace Catharsis.DialogueEditor.Model
{
    [System.Serializable]
    public class DialogueVariables
    {
        public readonly List<bool> booleans;
		public readonly List<float> floats;
		public readonly List<string> strings;
		
		public DialogueVariables(List<bool> booleans, List<float> floats, List<string> strings){
			this.booleans = booleans;
			this.floats = floats;
			this.strings = strings;
		}
		
		public DialogueVariables Clone(){
			List<bool> newBooleans = new List<bool>();
			for(int i=0; i<booleans.Count; i+=1){
				newBooleans.Add(booleans[i]);
			}
			
			List<float> newFloats = new List<float>();
			for(int i=0; i<floats.Count; i+=1){
				newFloats.Add(floats[i]);
			}
			
			List<string> newStrings = new List<string>();
			for(int i=0; i<strings.Count; i+=1){
				newStrings.Add(strings[i]);
			}
			
			return new DialogueVariables(newBooleans, newFloats, newStrings);
		}
	
    }
}