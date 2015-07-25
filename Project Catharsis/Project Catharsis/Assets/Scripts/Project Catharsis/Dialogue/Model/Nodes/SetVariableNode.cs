using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Data;
using Catharsis.DialogueEditor.Model.VariableEditor;
using Catharsis.GlobalVariablesManager;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public class SetVariableNode : DialogueNode
    {
        [Inject]
        public IGlobalVariableManager GlobalVariableManager { get; set; }
        public readonly VariableEditorScopes scope;
		public readonly VariableEditorTypes type;
		public readonly int variableId;
		public readonly VariableEditorSetEquation equation;
		public readonly string setValue;
		
		private bool _setBool;
		private float _setFloat;
		private string _setString;

        public SetVariableNode(VariableEditorScopes scope, VariableEditorTypes type, int variableId, VariableEditorSetEquation equation, string setValue, List<int?> outs)
            : base(outs)
        {
			this.scope = scope;
			this.type = type;
			this.variableId = variableId;
			this.equation = equation;
			this.setValue = setValue;
		}
		
		protected override void OnStart(){
			
			bool success = false;
			
			switch(type){
			case VariableEditorTypes.Boolean:
				success = bool.TryParse(setValue, out _setBool);
				switch(equation){
				case VariableEditorSetEquation.Equals:
					if(scope == VariableEditorScopes.Local){
						_localVariables.booleans[variableId] = _setBool;
					}else{
                        GlobalVariableManager.SetBool(variableId, _setBool);
					}
				break;
					
				case VariableEditorSetEquation.Toggle:
					if(scope == VariableEditorScopes.Local){
						_localVariables.booleans[variableId] = !_localVariables.booleans[variableId];
					}else{
                        GlobalVariableManager.SetBool(variableId, !GlobalVariableManager.GetBool(variableId));
					}
					success = true;
				break;
				}
			break;
				
			case VariableEditorTypes.Float:
				success = float.TryParse(setValue, out _setFloat);
				switch(equation){
				case VariableEditorSetEquation.Equals:
					if(scope == VariableEditorScopes.Local){
						_localVariables.floats[variableId] = _setFloat;
					}else{
                        GlobalVariableManager.SetFloat(variableId, _setFloat);
					}
				break;
				
				case VariableEditorSetEquation.Add:
					if(scope == VariableEditorScopes.Local){
						_localVariables.floats[variableId] += _setFloat;
					}else{
                        GlobalVariableManager.SetFloat(variableId, GlobalVariableManager.GetFloat(variableId) + _setFloat);
					}
				break;
					
				case VariableEditorSetEquation.Subtract:
					if(scope == VariableEditorScopes.Local){
						_localVariables.floats[variableId] -= _setFloat;
					}else{
                        GlobalVariableManager.SetFloat(variableId, GlobalVariableManager.GetFloat(variableId) - _setFloat);
					}
				break;
					
				case VariableEditorSetEquation.Multiply:
					if(scope == VariableEditorScopes.Local){
						_localVariables.floats[variableId] *= _setFloat;
					}else{
                        GlobalVariableManager.SetFloat(variableId, GlobalVariableManager.GetFloat(variableId) * _setFloat);
					}
				break;
					
				case VariableEditorSetEquation.Divide:
					if(scope == VariableEditorScopes.Local){
						_localVariables.floats[variableId] /= _setFloat;
					}else{
                        GlobalVariableManager.SetFloat(variableId, GlobalVariableManager.GetFloat(variableId) / _setFloat);
					}
				break;
					
				}
			break;
			
			case VariableEditorTypes.String:
				success = true;
				_setString = setValue;
				switch(equation){
				case VariableEditorSetEquation.Equals:
					if(scope == VariableEditorScopes.Local){
						_localVariables.strings[variableId] = _setString;
					}else{
                        GlobalVariableManager.SetString(variableId, _setString);
					}
				break;
					
				case VariableEditorSetEquation.Add:
					if(scope == VariableEditorScopes.Local){
						_localVariables.strings[variableId] += _setString;
					}else{
                        GlobalVariableManager.SetString(variableId, GlobalVariableManager.GetString(variableId) + _setString);
					}
				break;
				}
			break;
			}
			
			if(!success) Debug.LogWarning("[SetVariablePhase] Could not parse setValue");
			
			Continue(0);
			state = NodeState.Complete;
		}
    }
}