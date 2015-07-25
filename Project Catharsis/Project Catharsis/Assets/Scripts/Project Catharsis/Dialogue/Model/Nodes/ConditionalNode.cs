using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Data;
using Catharsis.DialogueEditor.Model.VariableEditor;
using Catharsis.GlobalVariablesManager;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public class ConditionalNode : DialogueNode
    {
        [Inject]
        public IGlobalVariableManager GlobalVariableManager { get; set; }

        public readonly VariableEditorScopes scope;
		public readonly VariableEditorTypes type;
		public readonly int variableId;
		public readonly VariableEditorGetEquation equation;
		public readonly string getValue;
		
		private bool _parsedBool;
		private bool _checkBool;
		
		private float _parsedFloat;
		private float _checkFloat;
		
		private string _parsedString;
		private string _checkString;

        public ConditionalNode(VariableEditorScopes scope, VariableEditorTypes type, int variableId, VariableEditorGetEquation equation, string getValue, List<int?> outs)
            : base(outs)
        {
			this.scope = scope;
			this.type = type;
			this.variableId = variableId;
			this.equation = equation;
			this.getValue = getValue;
		}
		
		protected override void OnStart(){
			
			bool success = true;
			
			switch(type){
			case VariableEditorTypes.Boolean:
				success = bool.TryParse(getValue, out _parsedBool);
				if(!success) Debug.LogError("[ConditionalPhase] Could Not Parse Bool: "+getValue);
				if(scope == VariableEditorScopes.Local){
					_checkBool = _localVariables.booleans[variableId];
				}else{
                    _checkBool = GlobalVariableManager.GetBool(variableId);
				}
			break;
				
			case VariableEditorTypes.Float:
				success = float.TryParse(getValue, out _parsedFloat);
				if(!success) Debug.LogError("[ConditionalPhase] Could Not Parse Float: "+getValue);
				if(scope == VariableEditorScopes.Local){
					_checkFloat = _localVariables.floats[variableId];
				}else{
                    _checkFloat = GlobalVariableManager.GetFloat(variableId);
				}
			break;
				
			case VariableEditorTypes.String:
				_parsedString = getValue;
				if(scope == VariableEditorScopes.Local){
					_checkString = _localVariables.strings[variableId];
				}else{
                    _checkString = GlobalVariableManager.GetString(variableId);
				}
			break;
			}
			
			bool isTrue = false;
			
			switch(type){
			case VariableEditorTypes.Boolean:
				switch(equation){
				case VariableEditorGetEquation.Equals:
					if(_parsedBool == _checkBool) isTrue = true;
				break;
					
				case VariableEditorGetEquation.NotEquals:
					if(_parsedBool != _checkBool) isTrue = true;
				break;
				}
			break;
			
			case VariableEditorTypes.Float:
				switch(equation){
				case VariableEditorGetEquation.Equals:
					if(_checkFloat == _parsedFloat) isTrue = true;
				break;
					
				case VariableEditorGetEquation.NotEquals:
					if(_checkFloat != _parsedFloat) isTrue = true;
				break;
					
				case VariableEditorGetEquation.EqualOrGreaterThan:
					if(_checkFloat >= _parsedFloat) isTrue = true;
				break;
				
				case VariableEditorGetEquation.EqualOrLessThan:
					if(_checkFloat <= _parsedFloat) isTrue = true;
				break;
					
				case VariableEditorGetEquation.GreaterThan:
					if(_checkFloat > _parsedFloat) isTrue = true;
					//Debug.Log ("[ConditionalPhase] " +_checkFloat+" > "+_parsedFloat+" = "+isTrue);
				break;
					
				case VariableEditorGetEquation.LessThan:
					if(_checkFloat < _parsedFloat) isTrue = true;
				break;
				}
			break;
				
			case VariableEditorTypes.String:
				switch(equation){
				case VariableEditorGetEquation.Equals:
					if(_parsedString == _checkString) isTrue = true;
				break;
					
				case VariableEditorGetEquation.NotEquals:
					if(_parsedString != _checkString) isTrue = true;
				break;
				}
			break;
			
			}
			
			if(isTrue){
				//Debug.Log ("[ConditionalPhase] Continue 0");
				Continue(0);
			}else{
				//Debug.Log ("[ConditionalPhase] Continue 1");
				Continue(1);
			}
			
			state = NodeState.Complete;
		}
    }
}