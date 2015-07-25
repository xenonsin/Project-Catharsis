using System;
using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.VariableEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Data
{
    public class TextData
    {
         
	    /// <summary>
	    /// The raw, unformatted text
	    /// </summary>
	    public readonly string rawText;

	    /// <summary>
	    /// The name field
	    /// </summary>
	    public readonly Character character;

        public readonly int animId;
        public readonly string animName;

	    /// <summary>
	    /// The metadata field
	    /// </summary>
	    public readonly string metadata;


	    /// <summary>
	    /// The audio field
	    /// </summary>
	    public readonly AudioClip audio;

	    /// <summary>
	    /// The position rect field
	    /// </summary>
	    public readonly Rect rect;


        public readonly bool waitForResponse;

        public readonly DialogueEditorWaitTypes waitType;

        public readonly float waitDuration;

	    /// <summary>
	    /// The branched-text node's choices
	    /// </summary>
	    public readonly string[] choices;



	    /// <summary>
	    /// Get the fotmatted text, with in-line variables
	    /// </summary>
	    public string text{
		    get{
			    return InsertMessageTextStringVariables(rawText);
		    }
	    }

	    /// <summary>
	    /// Returns whether or not the rect field was used for this node
	    /// </summary>
	    public bool usingPositionRect{
		    get{
			    return (!(rect.x == 0 && rect.y == 0 && rect.width == 0 && rect.height == 0));
		    }
	    }

	    /// <summary>
	    /// The type of TextPhase belonging to the current node
	    /// </summary>
	    public MessageNodeType windowType{
		    get{
                return (choices == null) ? MessageNodeType.Message : MessageNodeType.BranchedMessage;
		    }
	    }


        public TextData(string text, string charName, string animName, string audioName, string metadata, bool waitForResponse, float waitDuration, DialogueEditorWaitTypes waitType, Rect rect, List<string> choices)
        {
		    this.rawText = text;

            if (charName != string.Empty)
                this.character = Resources.Load(charName) as Character;

            this.animName = animName;

            if (audioName != string.Empty)
                this.audio = Resources.Load(audioName) as AudioClip;
		    this.metadata = metadata;
            this.waitForResponse = waitForResponse;
            this.waitDuration = waitDuration;
            this.waitType = waitType;

		    this.rect = new Rect(rect.x, rect.y, rect.width, rect.height);
		    if(choices != null){
			    string[] choicesClone = choices.ToArray();
			    this.choices = choicesClone.Clone() as string[];
		    }
	    }

        private static Dictionary<VariableEditorScopes, string> scopeStrings = new Dictionary<VariableEditorScopes, string>() { { VariableEditorScopes.Global, NodeVarSubStrings.GLOBAL }, { VariableEditorScopes.Local, NodeVarSubStrings.LOCAL } };
        private static Dictionary<VariableEditorTypes, string> typeStrings = new Dictionary<VariableEditorTypes, string>() { { VariableEditorTypes.Boolean, NodeVarSubStrings.BOOLEAN }, { VariableEditorTypes.Float, NodeVarSubStrings.FLOAT }, { VariableEditorTypes.String, NodeVarSubStrings.STRING } };

        public static string InsertMessageTextStringVariables(string input)
        {
            int dialogueId = 0; // TAKE THIS OUT IT YOU'RE NOT GOING TO IMPLEMENT INSERTING LOCAL STRINGS
            string output = input;
            output = SubstituteStringVariable(output, VariableEditorScopes.Global, VariableEditorTypes.Boolean, dialogueId);
            output = SubstituteStringVariable(output, VariableEditorScopes.Global, VariableEditorTypes.Float, dialogueId);
            output = SubstituteStringVariable(output, VariableEditorScopes.Global, VariableEditorTypes.String, dialogueId);
            //output = substituteStringVariable(output, VariableEditorScopes.Local, VariableEditorTypes.Boolean, dialogueId);
            //output = substituteStringVariable(output, VariableEditorScopes.Local, VariableEditorTypes.Float, dialogueId);
            //output = substituteStringVariable(output, VariableEditorScopes.Local, VariableEditorTypes.String, dialogueId);
            return output;
        }

        private static string SubstituteStringVariable(string input, VariableEditorScopes scope, VariableEditorTypes type, int dialogueId)
        {

            string output = string.Empty;

            string[] subStartString = new string[] { "<" + scopeStrings[scope] + typeStrings[type] + ">" };
            string[] subEndString = new string[] { "</" + scopeStrings[scope] + typeStrings[type] + ">" };


            //char[] subStartChars = new char[4]{'<',scopeStrings[scope],typeStrings[type],'>'};
            //char[] subEndChars = new char[5]{'<','/',scopeStrings[scope],typeStrings[type],'>'};

            //Debug.Log ("[DialoguerUtils] startString: "+string.Join("",subStartString)+" - endString: "+string.Join("",subEndString));

            string[] pieces = input.Split(subStartString, StringSplitOptions.None);

            //Debug.Log ("[DialoguerUtils] pieces count: "+pieces.Length+" - (should be 2)");

            for (int i = 0; i < pieces.Length; i += 1)
            {
                string[] subPieces = pieces[i].Split(subEndString, StringSplitOptions.None);

                //Debug.Log("[DialoguerUtils] subPieces[0] = "+subPieces[0]);

                int variableId;
                bool success = int.TryParse(subPieces[0], out variableId);
                if (success)
                {
                    switch (scope)
                    {
                        case VariableEditorScopes.Global:
                            switch (type)
                            {
                                case VariableEditorTypes.Boolean:
                                    //subPieces[0] = Dialoguer.GetGlobalBoolean(variableId).ToString();
                                    break;

                                case VariableEditorTypes.Float:
                                   // subPieces[0] = Dialoguer.GetGlobalFloat(variableId).ToString();
                                    break;

                                case VariableEditorTypes.String:
                                    //subPieces[0] = Dialoguer.GetGlobalString(variableId);
                                    break;
                            }
                            break;

                        case VariableEditorScopes.Local:
                            Debug.Log("Local Variable string substitutions not yet supported");
                            switch (type)
                            {
                                case VariableEditorTypes.Boolean:

                                    break;

                                case VariableEditorTypes.Float:

                                    break;

                                case VariableEditorTypes.String:

                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    //subPieces[0] = "_invalid_variable_id_";
                }

                output += string.Join("", subPieces);
            }

            return output;
        }
    }
}