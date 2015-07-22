using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using Catharsis.DialogueEditor.Model.VariableEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Objects
{
    [System.Serializable]
    public class DialogueEditorNodeObject
    {
        public int id;
        public DialogueEditorNodeTypes type;
        public string theme;
        public Vector2 position;

        //All Node Vars
        public List<int?> outs;
        public bool advanced;
        public string metadata;

        //Message Node Vars
        public string text;
        public string characterName;

        private Character _character;

        [XmlIgnore]
        public Character Character
        {
            get { return _character;}
            set
            {
                _character = value;
                characterName = value ? value.name : string.Empty;
            }
        }
        public int animSelectId;
        public string animName;

        public string audioName;
        private AudioClip _audio;
        [XmlIgnore]
        public AudioClip Audio
        {
            get { return _audio;}
            set
            {
                _audio = value;
                
                audioName = value ? value.name : string.Empty;
            }
        }

        public Rect rect;
        public bool waitForResponse;
        public DialogueEditorWaitTypes waitType;
        public float waitDuration;

        //Branching Message Node Var
        public List<string> choices;

        //Set Variable/Conditional Node Vars
        public VariableEditorScopes variableScope;
        public VariableEditorTypes variableType;
        public int variableId; //would probably need to remove this.. but then I would need a way to keep track of all the variables I'm creating?
        public Vector2 variableScrollPosition;

        // Set Variable Node SPECIFIC
        public VariableEditorSetEquation variableSetEquation;
        public string variableSetValue;

        // Conditional Node SPECIFIC
        public VariableEditorGetEquation variableGetEquation;
        public string variableGetValue;

        // Send Event Node
        public string eventName;

        public DialogueEditorNodeObject()
        {
            type = DialogueEditorNodeTypes.EmptyNode;
			position = Vector2.zero;
			
			text = string.Empty;
			
			outs = new List<int?>();
            

			choices = new List<string>();
			
			waitType = DialogueEditorWaitTypes.Seconds;
		}

        public void AddNewOut()
        {
            outs.Add(null);
        }

        public void RemoveOut()
        {
            outs.RemoveAt(outs.Count - 1);
        }

        public void AddNewChoice()
        {
            AddNewOut();
            choices.Add(string.Empty);
        }

        public void RemoveChoice()
        {
            RemoveOut();
            choices.RemoveAt(choices.Count - 1);
        }
    }

    public enum DialogueEditorNodeTypes
    {
        MessageNode, BranchingMessageNode, ConditionalNode, SetVariableNode, GenericEventNode, EndNode, EmptyNode,
    }

    public class DialogueEditorNodeType
    {
        public DialogueEditorNodeTypes type;
        public string name;
        public string info;
        public Texture icon;

        public DialogueEditorNodeType(DialogueEditorNodeTypes type, string name, string info, Texture icon)
        {
			this.type = type;
			this.name = name;
			this.info = info;
            this.icon = icon;
        }

        public static Dictionary<int, DialogueEditorNodeType> GetNodes()
        {
            Dictionary<int,DialogueEditorNodeType> nodes = new Dictionary<int, DialogueEditorNodeType>();

            // Message Node
            DialogueEditorNodeType node = new DialogueEditorNodeType(
                DialogueEditorNodeTypes.MessageNode, 
                "Message",
                "Displays text with one out-path.",
                GetIcon("messageNode"));
            nodes.Add((int)DialogueEditorNodeTypes.MessageNode, node);

            // Branching Message Node
            node = new DialogueEditorNodeType(
                DialogueEditorNodeTypes.BranchingMessageNode,
                "Branched Message",
                "Displays text with multiple, selectable out-paths.",
                GetIcon("branchedMessageNode"));
            nodes.Add((int)DialogueEditorNodeTypes.BranchingMessageNode, node);

            // Set Variable Node
            node = new DialogueEditorNodeType(
                DialogueEditorNodeTypes.SetVariableNode,
                "Set Variable",
                "Set a local or global variable.",
                GetIcon("setVariableNode"));
            nodes.Add((int)DialogueEditorNodeTypes.SetVariableNode, node);

            // Conditional Node
            node = new DialogueEditorNodeType(
                DialogueEditorNodeTypes.ConditionalNode,
                "Condition",
                "Moves to an out-path based on a condition.",
                GetIcon("conditionalNode"));
            nodes.Add((int)DialogueEditorNodeTypes.ConditionalNode, node);

            // Generic Event Node
            node = new DialogueEditorNodeType(
                DialogueEditorNodeTypes.GenericEventNode,
                "Event",
                "Dispatch an event which can be easily listened to and handled.",
                GetIcon("eventNode"));
            nodes.Add((int)DialogueEditorNodeTypes.GenericEventNode, node);

            // End Node
            node = new DialogueEditorNodeType(
                DialogueEditorNodeTypes.EndNode,
                "End",
                "Ends the dialogue and calls the dialogue's callback.",
                GetIcon("endNode"));
            nodes.Add((int)DialogueEditorNodeTypes.EndNode, node);


            return nodes;
        }

        private static Texture GetIcon(string icon)
        {
            //string iconPath = DialogueEditorGUI.toolbarIconPath;
            string iconPath = "Textures/GUI/";
            //iconPath += (EditorGUIUtility.isProSkin) ? "Dark/" : "Light/";
            iconPath += "icon_" + icon;
            return Resources.Load(iconPath) as Texture; 
        }
    }

    public class DialogueEditorNodeTemplates
    {
        // Message
        public static DialogueEditorNodeObject NewMessageNode(int id)
        {
            DialogueEditorNodeObject node = new DialogueEditorNodeObject();

            node.id = id;
            node.type = DialogueEditorNodeTypes.MessageNode;

            node.position = Vector2.zero;

            node.advanced = false;
            node.metadata = string.Empty;

            node.Character = null;
            node.animSelectId = 0;
            node.animName = string.Empty;
            node.Audio = null;
            node.rect = new Rect(0,0,0,0);
            node.waitForResponse = true;
            node.waitDuration = 0;

            node.outs = new List<int?>();
            node.outs.Add(null);
            return node;
        }

        public static DialogueEditorNodeObject NewBranchedMessageNode(int id)
        {
            DialogueEditorNodeObject node = new DialogueEditorNodeObject();

            node.id = id;
            node.type = DialogueEditorNodeTypes.BranchingMessageNode;

            node.position = Vector2.zero;

            node.advanced = false;
            node.metadata = string.Empty;

            node.Character = null;
            node.animSelectId = 0;
            node.animName = string.Empty;
            node.Audio = null;
            node.rect = new Rect(0, 0, 0, 0);
            node.waitForResponse = true;
            node.waitDuration = 0;

            node.outs = new List<int?>();
            node.outs.Add(null);
            node.outs.Add(null);

            node.choices = new List<string>();
            node.choices.Add(string.Empty);
            node.choices.Add(string.Empty);

            return node;
        }

        public static DialogueEditorNodeObject NewSetVariableNode(int id)
        {
            DialogueEditorNodeObject node = new DialogueEditorNodeObject();

            node.id = id;
            node.type = DialogueEditorNodeTypes.SetVariableNode;

            node.position = Vector2.zero;

            node.advanced = false;
            node.metadata = string.Empty;

            node.outs = new List<int?>();
            node.outs.Add(null);

            node.variableScope = VariableEditorScopes.Local;
            node.variableType = VariableEditorTypes.Boolean;
            node.variableSetEquation = VariableEditorSetEquation.Equals;
            node.variableScrollPosition = new Vector2();
            node.variableId = 0;
            node.variableSetValue = string.Empty;

            return node;
        }

        public static DialogueEditorNodeObject NewConditionalNode(int id)
        {
            DialogueEditorNodeObject node = new DialogueEditorNodeObject();

            node.id = id;
            node.type = DialogueEditorNodeTypes.ConditionalNode;

            node.position = Vector2.zero;

            node.advanced = false;
            node.metadata = string.Empty;

            node.outs = new List<int?>();
            node.outs.Add(null);
            node.outs.Add(null);

            return node;
        }

        public static DialogueEditorNodeObject NewEventNode(int id)
        {
            DialogueEditorNodeObject node = new DialogueEditorNodeObject();

            node.id = id;
            node.type = DialogueEditorNodeTypes.GenericEventNode;

            node.position = Vector2.zero;

            node.advanced = false;
            node.metadata = string.Empty;

            node.outs = new List<int?>();
            node.outs.Add(null);
            node.eventName = string.Empty;

            return node;
        }

        public static DialogueEditorNodeObject NewEndNode(int id)
        {
            DialogueEditorNodeObject node = new DialogueEditorNodeObject();

            node.id = id;
            node.type = DialogueEditorNodeTypes.EndNode;

            node.position = Vector2.zero;

            return node;
        }

    }

}