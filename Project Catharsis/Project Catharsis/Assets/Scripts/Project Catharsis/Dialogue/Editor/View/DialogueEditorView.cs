using System.Collections.Generic;
using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Model;
using Catharsis.DialogueEditor.Model.Objects;
using Catharsis.DialogueEditor.Model.VariableEditor;
using strange.extensions.editor.impl;
using UnityEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor
{
    public class DialogueEditorView : EditorView
    {

        #region Menu Options
        public enum FileMenuOptions
        {
            SaveScenario = 0, SaveScenarioAs, NewScenario, LoadScenario, NewDialogue, AddMessageNode, AddBranchingMessageNode, AddConditionalNode, AddSetVariableNode, AddGenericEventNode, AddEndNode,
        }

        public enum EditMenuOptions
        {
            Duplicate, Delete, DeleteAll, Copy, Paste,
        }



        #endregion



        //probably put this in the mediator?
        [SerializeField]
        private DialogueEditorDataManager _dialogueData;
        [SerializeField]
        private string currentScenario;

        [SerializeField]
        private Vector2 _hierarchyScrollPos = Vector2.zero;
        [SerializeField]
        private float _hierarchyPanelWidth = _menuWidth * 2;

        [SerializeField] 
        private int currentDialogue;
        [SerializeField]
        private int currentNode;
        [SerializeField]
        private Texture2D _highlightTexture;

        [SerializeField] private Texture _bgTexture;

        private bool loaded = false;
        private bool newScenario = true;

        [SerializeField]
        private Dictionary<int, DialogueEditorNodeType> _nodeTypes;

        [SerializeField] private DialogueEditorSettings _settings;

        // Dragging vars
        private Vector2 _panningMousePosition;
        //private Vector2 __panningPreviousPosition;
        private bool _panning;

        // Canvas scroll width object
        private Vector2 _scrollLimits;

        // Tool Info
        private string _toolInfoName;
        private string _toolInfoDescription;
        private string _tooltipInfo;

        //Window Dimensions
        private float _toolbarHeight = 18.0f;
        private float _hierarchyItemHeight = 20.0f;
        private const float _minWindowRectWidth = 800.0f;
        private const float _minWindowRectHeight = 400.0f;
        private const float _menuWidth = 100.0f;
        private const float _minHierarchyPanelWidth = 150.0f;
        private float nodeButtonSize = 30.0f;

        // Selection Objects
        DialogueEditorSelectionObject _outputSelection;
        DialogueEditorDragNodeObject _dragSelection;
         
        [MenuItem("Catharsis/Dialogue Manager/Dialogue Editor", false, 0)]
        public static void OpenWindow()
        {
            DialogueEditorView window = EditorWindow.GetWindow<DialogueEditorView>("Dialogue Editor");
            window.minSize = new Vector2(_minWindowRectWidth, _minWindowRectHeight);
            //window.title
            
            //window.Show();
            window.Init();
        }

        void OnEnable()
        {
            base.OnEnable();
            InitNodeTypes();
            if (_dialogueData == null)
            {
                //_dialogueData = new DialogueEditorDataManager();

                LoadSettings();
                New();
                if(_settings.lastScenarioName != string.Empty)
                    LoadScenario(_settings.lastScenarioName);


                loaded = true;
            }
            //Init();
        }



        private void Init()
        {     
            _outputSelection = null;

            if (_nodeTypes == null)
                InitNodeTypes();
            GetNodeScrollLimits();

            if (_highlightTexture == null)
                CreateHighlightTexture();

            if (_bgTexture == null)
                LoadBgTexture();


        }

        private void InitNodeTypes()
        {
            _nodeTypes = DialogueEditorNodeType.GetNodes();
        }

        void OnDisable()
        {
            base.OnDisable();
            Texture2D.DestroyImmediate(_highlightTexture);
            _highlightTexture = null;
        }

        void OnGUI()
        {
            if (loaded)
            {
                SetToolInfo(string.Empty, string.Empty);
                SetTooltip(string.Empty);

                DisplayNodePanel();
                DisplayHierarchyPanel();
             
            }
            DisplayToolBar();
        }

        private void LoadSettings()
        {
            _settings = Resources.Load("DialogueEditorSettings") as DialogueEditorSettings;

            //currentScenario = _settings.lastScenarioName;
        }

        private void New()
        {
            _dialogueData = new DialogueEditorDataManager();
            newScenario = true;
        }

        private void LoadScenario(string nameOfScenario)
        {
            bool assetExists = System.IO.File.Exists(nameOfScenario);
            if (assetExists)
            {
                _dialogueData.Load(nameOfScenario);
            }
            else
            {
                Debug.LogWarning("name of Asset doesn't exist!");
            }

            newScenario = false;
        }

        private void LoadScenario()
        {
            string file = EditorUtility.OpenFilePanel("Import input profile", "", "xml");
            if (string.IsNullOrEmpty(file))
                return;
            _dialogueData.Load(file);

            Repaint();

            _settings.lastScenarioName = file;

            newScenario = false;
        }
        private void Save()
        {
            if (newScenario)
                SaveAs();
            else
                _dialogueData.Save(_settings.lastScenarioName);

            newScenario = false;
        }

        private void SaveAs()
        {
            string file = EditorUtility.SaveFilePanel("Export input profile", "", "newScenario.xml", "xml");
            if (string.IsNullOrEmpty(file))
                return;

            _dialogueData.Save(file);

            newScenario = false;
            _settings.lastScenarioName = file;

        }


        private void LoadBgTexture()
        {
            _bgTexture = Resources.Load("Textures/GUI/scrollbox_bg") as Texture;
        }

        private void CreateHighlightTexture()
        {
            _highlightTexture = new Texture2D(1, 1);
            _highlightTexture.SetPixel(0, 0, new Color32(50, 125, 255, 255));
            _highlightTexture.Apply();
        }

        #region NodeEditor
        void DisplayNodePanel()
        {
            Rect screenRect = new Rect(_hierarchyPanelWidth + 5.0f, _toolbarHeight + 5,
                                        position.width - (_hierarchyPanelWidth + 5.0f),
                                        position.height - _toolbarHeight - 5.0f);
            DisplayDialogueName(screenRect);
            DisplayNodeButtons(screenRect);
            DisplayToolTip(screenRect);
            DisplayCanvas(screenRect);
            
        }

        void DisplayDialogueName(Rect rect)
        {
            Rect nameRect = new Rect(rect.x + 5, rect.y + 6, 200, _toolbarHeight + 12);
            Rect nameShadowRect = new Rect(nameRect.x - 1, nameRect.y - 1, nameRect.width + 2, nameRect.height + 2);
            GUIStyle nameTextStyle = new GUIStyle(GUI.skin.GetStyle("textfield"));
            nameTextStyle.fontSize = 22;
            GUI.color = GUI.contentColor;

            GUI.Box(nameShadowRect, string.Empty);

            if (currentDialogue < _dialogueData.data.DialogueCount && currentDialogue >= 0)
            {
                _dialogueData.data.dialogues[currentDialogue].dialogueName = GUI.TextField(nameRect, _dialogueData.data.dialogues[currentDialogue].dialogueName, nameTextStyle);
                GUI.color = new Color(1, 1, 1, 0.25f);
                if (_dialogueData.data.dialogues[currentDialogue].dialogueName == "") 
                    GUI.Label(nameRect, "Dialogue Name", nameTextStyle);
            }

            GUI.color = GUI.contentColor;
        }

        void DisplayNodeButtons(Rect rect)
        {
            int numOfButtons = System.Enum.GetValues(typeof (DialogueEditorNodeTypes)).Length -1;
            Rect[] buttonRects = new Rect[numOfButtons];
            Rect toolbarInnerRect = new Rect(rect.x + 200 + 15, rect.y + 5, 5 + (nodeButtonSize * numOfButtons) + 5, _toolbarHeight + 12);

            //List<string> buttonType = new List<string>(new string[] { "Me", "Br", "Co", "Se", "Ev", "En" });

            for (int i = 0; i < numOfButtons; i++)
            {

                GUIStyle toolStyle;
                if (i == 0)
                {
                    toolStyle = EditorStyles.miniButtonLeft;
                }
                else if (i == numOfButtons - 1)
                {
                    toolStyle = EditorStyles.miniButtonRight;
                }
                else
                {
                    toolStyle = EditorStyles.miniButtonMid;
                }
			
                buttonRects[i] = new Rect(toolbarInnerRect.x + nodeButtonSize * i, toolbarInnerRect.y, nodeButtonSize, nodeButtonSize);
                if (GUI.Button(buttonRects[i], _nodeTypes[i].icon, toolStyle) && _dialogueData.data.DialogueCount > 0)
                {
                    Vector2 newNodePosition = Vector2.zero;
                    newNodePosition.x = _dialogueData.data.dialogues[currentDialogue].scrollPosition.x + 20;
                    newNodePosition.y = _dialogueData.data.dialogues[currentDialogue].scrollPosition.y + 20;
                    _dialogueData.data.dialogues[currentDialogue].AddNode(_nodeTypes[i].type, newNodePosition);
                    //Debug.Log(_dialogueData.dialogues[currentDialogue].nodes[0].outs[0]);

                }

                if (buttonRects[i].Contains(Event.current.mousePosition))
                {
                    string toolName = _nodeTypes[i].name;
                    string toolInfo = _nodeTypes[i].info;
                    SetToolInfo(toolName, toolInfo);
                }
            }

        }

        void DisplayToolTip(Rect rect)
        {
            int numOfButtons = System.Enum.GetValues(typeof(DialogueEditorNodeTypes)).Length - 1;

            Rect toolbarInnerRect = new Rect(rect.x + 200 + 15, rect.y + 5, 5 + (nodeButtonSize * numOfButtons) + 5, _toolbarHeight + 12);

            Rect toolInfoBg = new Rect(toolbarInnerRect.x + toolbarInnerRect.width, toolbarInnerRect.y, 300, toolbarInnerRect.height);

            //GUI.Box(toolInfoBg, string.Empty, DialogueEditorGUI.gui.GetStyle("box_outset"));
            
            GUI.Label(new Rect(toolInfoBg.x + 2, toolInfoBg.y + 2, toolInfoBg.width - 4, toolInfoBg.height - 4), _toolInfoName, EditorStyles.boldLabel);
            GUIStyle tooltipInfoStyle = new GUIStyle(EditorStyles.miniLabel);
            tooltipInfoStyle.alignment = TextAnchor.UpperLeft;
            GUI.Label(new Rect(toolInfoBg.x + 2, toolInfoBg.y + 15, toolInfoBg.width - 4, toolInfoBg.height - 15), _toolInfoDescription, tooltipInfoStyle);

            //Save Warning
            GUI.Label(new Rect(toolInfoBg.x + 400, toolInfoBg.y + 5, toolInfoBg.width - 4, toolInfoBg.height - 15), "Remember to SAVE!");

        }

        void DisplayCanvas(Rect rect)
        {
            Rect canvasRect = new Rect(rect.x + 5, rect.y + 40, rect.width, rect.height);
            Rect scrollRect = new Rect(canvasRect.x, canvasRect.y, canvasRect.width - 5, canvasRect.height - 40);
            Vector2 newScrollSize = Vector2.zero;
            newScrollSize.x = (_scrollLimits.x > canvasRect.width) ? _scrollLimits.x : canvasRect.width - 15;
            newScrollSize.y = (_scrollLimits.y > canvasRect.height) ? _scrollLimits.y : canvasRect.height - 15;

            GUI.Box(canvasRect, string.Empty, new GUIStyle(GUI.skin.GetStyle("ShurikenEffectBg")));
            //DrawBackGroundTexture(scrollRect, newScrollSize);
            
            //GUIStyle canvasStyle = new GUIStyle(GUI.skin.GetStyle("Scene"));
            

            if (_dialogueData.data.dialogues.Count < 1)
            {
                GUI.BeginScrollView(scrollRect, GetScrollPosition(), new Rect(0, 0, newScrollSize.x, newScrollSize.y), true, true);
                GUI.EndScrollView();
                return;
            }
            if (_panning)
            {
                GUI.BeginScrollView(scrollRect, GetScrollPosition(), new Rect(0, 0, newScrollSize.x, newScrollSize.y), true, true);
            }
            else
            {
                //GUI.BeginScrollView(canvasRect, GetScrollPosition(), new Rect(0, 0, newScrollSize.x, newScrollSize.y), true, true);

                _dialogueData.data.dialogues[currentDialogue].scrollPosition = GUI.BeginScrollView(scrollRect, GetScrollPosition(), new Rect(0, 0, newScrollSize.x, newScrollSize.y), true, true);
            }


            //Draw Connections
            DrawConnections();

            // Handle Middle Mouse Drag for Panning
            HandleCanvasMiddleMouseDrag();

            DrawStartBox();

            DrawCanvasContents();

            // Handle Dragging
            HandleDragging();

            // Handle Clicks
            HandleMouse();

            GUI.EndScrollView();
            DrawTooltip();

        }

        void DrawBackGroundTexture(Rect rect, Vector2 newScrollSize)
        {
            GUI.color = new Color(1, 1, 1, 0.25f);
            GUI.DrawTextureWithTexCoords(
            rect,
            _bgTexture,
            new Rect(0, 0, newScrollSize.x / _bgTexture.width, newScrollSize.y / _bgTexture.height));
            GUI.color = GUI.contentColor;
        }

        private void DrawConnections()
        {
            if (_dialogueData.data.dialogues.Count < 1 || currentDialogue >= _dialogueData.data.dialogues.Count) return;
            DialogueEditorDialogueObject dialogue = _dialogueData.data.dialogues[currentDialogue];
            Vector2 startButtonPosition = new Vector2(150, 20);
            if (_outputSelection != null)
            {
                if (_outputSelection.isStart)
                {
                    DialogueEditorCurve.draw(startButtonPosition, Event.current.mousePosition);
                }
                else
                {
                    DialogueEditorCurve.draw(GetNodeOutputPosition(_outputSelection.nodeId, _outputSelection.outputIndex), Event.current.mousePosition);
                }
            }

            if (dialogue.startPage.HasValue)
            {
                DialogueEditorNodeObject startNode = dialogue.nodes[dialogue.startPage.Value];
                DialogueEditorCurve.draw(new Vector2(150, 20), new Vector2(startNode.position.x + 12, startNode.position.y + 12));
            }

            for (int p = 0; p < dialogue.nodes.Count; p += 1)
            {
                for (int o = 0; o < dialogue.nodes[p].outs.Count; o += 1)
                {
                    if (
                        !dialogue.nodes[p].outs[o].HasValue
                        || dialogue.nodes[p].outs[o].Value >= dialogue.nodes.Count
                        || dialogue.nodes[dialogue.nodes[p].outs[o].Value] == null
                    )
                    {
                        continue;
                    }

                    int outputNodeId = dialogue.nodes[p].id;
                    Vector2 outputNodePos = GetNodeOutputPosition(outputNodeId, o);
                    //Debug.Log("type: "+ dialogue.phases[p].type.ToString() +"; phaseId: " + outputPhaseId + "; outputIndex: " + o +";");

                    Vector2 inputNodePos = new Vector2(dialogue.nodes[dialogue.nodes[p].outs[o].Value].position.x + 12, dialogue.nodes[dialogue.nodes[p].outs[o].Value].position.y + 12);
                    DialogueEditorCurve.draw(outputNodePos, inputNodePos);
                }
            }
        }

        private void DrawStartBox()
        {
            // START BOX
            Rect startRect = new Rect(10, 10, 150, 70);
            GUI.Box(startRect, string.Empty);
            Rect startLabelRect = new Rect(startRect.x + 4, startRect.y + 3, startRect.width, 20);
            GUI.Label(startLabelRect, "Start Page");
            //string startConnectorType = (_dialogueData.dialogues[currentDialogue].startPage != null) ? "connector_full" : "connector_empty";
            Rect startOutputButtonRect = new Rect(startRect.x + startRect.width - 19, startRect.y + 3, 16, 16);
            if (Event.current.type == EventType.MouseDown && startOutputButtonRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0)
                {
                    _dialogueData.data.dialogues[currentDialogue].startPage = null;
                    _outputSelection = new DialogueEditorSelectionObject(true);
                }
                else if (Event.current.button == 1)
                {
                    _dialogueData.data.dialogues[currentDialogue].startPage = null;
                    if (_outputSelection != null && _outputSelection.isStart)
                    {
                        _outputSelection = null;
                    }
                }


            }
            GUI.Button(startOutputButtonRect, string.Empty, new GUIStyle(GUI.skin.GetStyle("IN Foldout")));

            // Local Variables
            GUI.color = new Color(1, 1, 1, 0.5f);
            Rect countLabelsRect = new Rect(startLabelRect.x, startLabelRect.y + 18, startLabelRect.width, 22);
            GUI.Label(new Rect(countLabelsRect.x, countLabelsRect.y, countLabelsRect.width, countLabelsRect.height), "Local Booleans:	" + _dialogueData.data.dialogues[currentDialogue].booleans.variables.Count);
            GUI.Label(new Rect(countLabelsRect.x, countLabelsRect.y + 15, countLabelsRect.width, countLabelsRect.height), "Local Floats:		" + _dialogueData.data.dialogues[currentDialogue].floats.variables.Count);
            GUI.Label(new Rect(countLabelsRect.x, countLabelsRect.y + 30, countLabelsRect.width, countLabelsRect.height), "Local Strings:		" + _dialogueData.data.dialogues[currentDialogue].strings.variables.Count);
            GUI.color = GUI.contentColor;
        }

        private void DrawCanvasContents()
        {
            if (_dialogueData.data.DialogueCount > 0 && currentDialogue >= 0 && currentDialogue < _dialogueData.data.DialogueCount && _dialogueData.data.dialogues[currentDialogue].nodes.Count > 0)
            {
                for (int i = 0; i < _dialogueData.data.dialogues[currentDialogue].nodes.Count; i += 1)
                {
                    DialogueEditorNodeObject node = _dialogueData.data.dialogues[currentDialogue].nodes[i];
                    //Debug.Log(_dialogueData.dialogues[currentDialogue].nodes[i].outs[0]); fail
                    switch (node.type)
                    {
                        case DialogueEditorNodeTypes.MessageNode:
                            DrawMessage(node);                         
                            break;

                        case DialogueEditorNodeTypes.BranchingMessageNode:
                            DrawBranchedMessageNode(node);
                            break;

                        case DialogueEditorNodeTypes.SetVariableNode:
                            DrawSetVariableNode(node);
                            break;

                        case DialogueEditorNodeTypes.ConditionalNode:
                            DrawConditionalNode(node);
                            break;

                        case DialogueEditorNodeTypes.GenericEventNode:
                            DrawEventNode(node);
                            break;

                        case DialogueEditorNodeTypes.EndNode:
                            DrawEndNode(node);
                            break;

                        case DialogueEditorNodeTypes.EmptyNode:
                            Debug.LogWarning("Dialogue:" + currentDialogue + ", Page: " + i + " is an EmptyPhase. Something went wrong.");
                            break;
                    }
                }
            }
        }
        #endregion

        #region HierarchyPanel
        void DisplayHierarchyPanel()
        {
            Rect screenRect = new Rect(0.0f, _toolbarHeight - 5.0f, _hierarchyPanelWidth, position.height - _toolbarHeight + 10.0f);
            Rect scrollView = new Rect(screenRect.x, screenRect.y + 20, screenRect.width, position.height - screenRect.y);

            //ADD
            Rect addButtonRect = new Rect(screenRect.x + 5, screenRect.y + 15, (screenRect.width * 0.5f) - 5, 30);
            if (GUI.Button(addButtonRect, "Add", EditorStyles.miniButtonLeft))
                AddDialogue(1);

            // REMOVE
            Rect deleteButtonRect = new Rect((screenRect.width * 0.5f), screenRect.y + 15, (screenRect.width * 0.5f) - 5, 30);
            if (_dialogueData.data.DialogueCount > 0)
            {
                if (GUI.Button(deleteButtonRect, "Delete", EditorStyles.miniButtonRight))
                    RemoveDialogue(currentDialogue);

            }
            else
            {
                GUI.color = new Color(1, 1, 1, 0.25f);
                GUI.Button(deleteButtonRect, "Delete");
                GUI.color = GUI.contentColor;
            }


            GUILayout.BeginArea(scrollView);
            //GUI.Box(screenRect, "");
            GUI.Box(scrollView, "");
            GUILayout.Space(33.0f);

            _hierarchyScrollPos = EditorGUILayout.BeginScrollView(_hierarchyScrollPos);

            
            for (int i = 0; i < _dialogueData.data.DialogueCount; i++)
            {


                DisplayHierarchyDialogueItem(screenRect, i, _dialogueData.data.dialogues[i].dialogueName);

            }
            
            
            EditorGUILayout.EndScrollView();
            GUILayout.Space(20.0f);
            GUILayout.EndArea();
        }

        void DisplayHierarchyDialogueItem(Rect rect, int index, string name)
        {
            

            Vector2 mouseClickPosition = Vector2.zero;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                mouseClickPosition = new Vector2(Event.current.mousePosition.x - rect.x - 3, Event.current.mousePosition.y - rect.y - 3 + _hierarchyItemHeight);
            }

            Rect configPos = GUILayoutUtility.GetRect(new GUIContent(name), EditorStyles.label, GUILayout.Height(_hierarchyItemHeight));

            if (mouseClickPosition != Vector2.zero && configPos.Contains(mouseClickPosition))
            {
                currentDialogue = index;
            }
             //GUI.Box(configPos, string.Empty);
            //Display Highlight texture
            //if (configPos.Contains(Event.current.mousePosition))
            //{
            //    GUI.color = Color.grey;
            //    GUI.Box(configPos, string.Empty);
            //}

            //GUI.color = GUI.contentColor;


            if (currentDialogue == index) //_selectionIndex[0] == index
            {
                if (_highlightTexture == null)
                {
                    CreateHighlightTexture();
                }
                GUI.DrawTexture(configPos, _highlightTexture, ScaleMode.StretchToFill);

            }
            //GUI.Box(configPos,string.Empty);

            Rect labelNumberRow = new Rect(configPos.x + 2, configPos.y + 2, configPos.width - 4, configPos.height - 4);
            Rect labelNameRow = new Rect(labelNumberRow.x + 25, labelNumberRow.y, labelNumberRow.width - 25, labelNumberRow.height);

            GUI.Label(labelNumberRow, _dialogueData.data.dialogues[index].id.ToString());
            GUI.Label(labelNameRow, (_dialogueData.data.dialogues[index].dialogueName == string.Empty) ? "-" : _dialogueData.data.dialogues[index].dialogueName);

            Repaint();
           
        }
        #endregion

        #region Toolbar
        void DisplayToolBar()
        {
            //The rects constructed here determine the look of all the components hence forth.
            Rect screenRect = new Rect(0.0f, 0.0f, position.width, _toolbarHeight); //This rect encompasses the toolbars
            Rect fileMenuRect = new Rect(0.0f, 0.0f, _menuWidth, screenRect.height); //This rect is the dimensions of the FileMenu drop down
            Rect editMenuRect = new Rect(fileMenuRect.xMax, 0.0f, _menuWidth, screenRect.height); //this rect is the dimensiosn of the EditMenu drop down
            Rect saveFieldRect = new Rect(editMenuRect.xMax, 0.0f, _menuWidth, screenRect.height);

            Rect paddingLabelRect = new Rect(saveFieldRect.xMax, 0.0f, screenRect.width - _menuWidth * 2, screenRect.height); //This rect is the dimensions of the padding after the drop down menus



            GUI.BeginGroup(screenRect);
            DisplayFileMenu(fileMenuRect);
            DisplayEditMenu(editMenuRect);
            DisplaySaveButton(saveFieldRect);
            EditorGUI.LabelField(paddingLabelRect, "", EditorStyles.toolbarButton); //This displays the padding after the menus.
            GUI.EndGroup();
        }

        private void DisplayFileMenu(Rect rect)
        {
            EditorGUI.LabelField(rect, "File", EditorStyles.toolbarDropDown);

            //If someone clicks the dropdown box..
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                rect.Contains(Event.current.mousePosition))
            {
                CreateFileMenu(new Rect(rect.x, rect.yMax, 0.0f, 0.0f));
            }
        }

        private void CreateFileMenu(Rect rect)
        {
            GenericMenu fileMenu = new GenericMenu();

            fileMenu.AddItem(new GUIContent("New Scenario"), false, HandleFileMenuOption, FileMenuOptions.NewScenario);
            
            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("Save Scenario"), false, HandleFileMenuOption, FileMenuOptions.SaveScenario);
            fileMenu.AddItem(new GUIContent("Save Scenario As..."), false, HandleFileMenuOption, FileMenuOptions.SaveScenarioAs);

            fileMenu.AddItem(new GUIContent("Load Scenario"), false, HandleFileMenuOption, FileMenuOptions.LoadScenario);

            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("New Dialogue"), false, HandleFileMenuOption, FileMenuOptions.NewDialogue);
            fileMenu.AddSeparator("");

            if (_dialogueData.data.DialogueCount > 0)
            {
                fileMenu.AddItem(new GUIContent("Add Message Node"), false, HandleFileMenuOption,FileMenuOptions.AddMessageNode);
                fileMenu.AddItem(new GUIContent("Add Branching Message Node"), false, HandleFileMenuOption,FileMenuOptions.AddBranchingMessageNode);
                fileMenu.AddItem(new GUIContent("Add Conditional Node"), false, HandleFileMenuOption,FileMenuOptions.AddConditionalNode);
                fileMenu.AddItem(new GUIContent("Add Set Variable Node"), false, HandleFileMenuOption, FileMenuOptions.AddSetVariableNode);
                fileMenu.AddItem(new GUIContent("Add Generic Event Node"), false, HandleFileMenuOption, FileMenuOptions.AddGenericEventNode);
                fileMenu.AddItem(new GUIContent("Add End Node"), false, HandleFileMenuOption, FileMenuOptions.AddEndNode);

            }
            else
            {
                fileMenu.AddDisabledItem(new GUIContent("Add Message Node"));
                fileMenu.AddDisabledItem(new GUIContent("Add Branching Message Node"));
                fileMenu.AddDisabledItem(new GUIContent("Add Conditional Node"));
                fileMenu.AddDisabledItem(new GUIContent("Add Set Variable Node"));
                fileMenu.AddDisabledItem(new GUIContent("Add Generic Event Node"));
                fileMenu.AddDisabledItem(new GUIContent("Add End Node"));

                
            }



            fileMenu.DropDown(rect);
        }

        private void HandleFileMenuOption(object arg)
        {


            FileMenuOptions option = (FileMenuOptions)arg;
            switch (option)
            {
                case FileMenuOptions.NewScenario:
                    New();
                    break;
                case FileMenuOptions.SaveScenario:
                    Save();
                    break;
                case FileMenuOptions.SaveScenarioAs:
                    SaveAs();
                    break;
                case FileMenuOptions.LoadScenario:
                    LoadScenario();
                    break;
                case FileMenuOptions.NewDialogue:
                    AddDialogue(1);
                    break;
                case FileMenuOptions.AddMessageNode:
                    AddNode(DialogueEditorNodeTypes.MessageNode);
                    break;
                case FileMenuOptions.AddBranchingMessageNode:
                    AddNode(DialogueEditorNodeTypes.BranchingMessageNode);
                    break;
                case FileMenuOptions.AddConditionalNode:
                    AddNode(DialogueEditorNodeTypes.ConditionalNode);
                    break;
                case FileMenuOptions.AddSetVariableNode:
                    AddNode(DialogueEditorNodeTypes.SetVariableNode);
                    break;
                case FileMenuOptions.AddGenericEventNode:
                    AddNode(DialogueEditorNodeTypes.GenericEventNode);
                    break;
                case FileMenuOptions.AddEndNode:
                    AddNode(DialogueEditorNodeTypes.EndNode);
                    break;
            }
        }

        private void DisplayEditMenu(Rect rect)
        {
            EditorGUI.LabelField(rect, "Edit", EditorStyles.toolbarDropDown);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                rect.Contains(Event.current.mousePosition))
            {
                CreateEditMenu(new Rect(rect.x, rect.yMax, 0.0f, 0.0f));
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.D && Event.current.shift)
                {
                    //Duplicate();
                    Event.current.Use();
                }
                if (Event.current.keyCode == KeyCode.Delete)
                {
                    //Delete();
                    Event.current.Use();
                }
            }
        }

        private void CreateEditMenu(Rect rect)
        {
            GenericMenu editMenu = new GenericMenu();
            if (currentNode >= 1)
                editMenu.AddItem(new GUIContent("Duplicate          Shift+D"), false, HandleEditMenuOption, EditMenuOptions.Duplicate);
            else
                editMenu.AddDisabledItem(new GUIContent("Duplicate          Shift+D"));

            if (_dialogueData.data.DialogueCount > 0)
                editMenu.AddItem(new GUIContent("Delete                Del"), false, HandleEditMenuOption, EditMenuOptions.Delete);
            else
                editMenu.AddDisabledItem(new GUIContent("Delete                Del"));

            //if (_inputManager.GetInputConfigurationCount() > 0)
            //    editMenu.AddItem(new GUIContent("Delete All"), false, HandleEditMenuOption, EditMenuOptions.DeleteAll);
            //else
            //    editMenu.AddDisabledItem(new GUIContent("Delete All"));

            if (currentNode >= 1)
                editMenu.AddItem(new GUIContent("Copy"), false, HandleEditMenuOption, EditMenuOptions.Copy);
            else
                editMenu.AddDisabledItem(new GUIContent("Copy"));

            //if (_copySource != null && _selectionIndex.Count >= 2)
            //    editMenu.AddItem(new GUIContent("Paste"), false, HandleEditMenuOption, EditMenuOptions.Paste);
            //else
            //    editMenu.AddDisabledItem(new GUIContent("Paste"));

            //editMenu.AddSeparator("");
            editMenu.DropDown(rect);

        }

        private void HandleEditMenuOption(object arg)
        {
            EditMenuOptions option = (EditMenuOptions)arg;
            switch (option)
            {
                case EditMenuOptions.Duplicate:
                   // Duplicate();
                    break;
                case EditMenuOptions.Delete:
                    RemoveDialogue(currentDialogue);
                    break;
                case EditMenuOptions.DeleteAll:
                   // DeleteAll();
                    break;
                case EditMenuOptions.Copy:
                   // Copy();
                    break;
                case EditMenuOptions.Paste:
                   // Paste();
                    break;
            }
        }

        private void DisplaySaveButton(Rect rect)
        {
            if (GUI.Button(rect, "Save", EditorStyles.toolbarButton))
                Save();
        }
        #endregion


        #region Tooltips
        private void SetToolInfo(string toolInfoName, string toolInfoDescription)
        {
            _toolInfoName = toolInfoName;
            _toolInfoDescription = toolInfoDescription;
        }

        private void SetTooltip(string info)
        {
            _tooltipInfo = info;
        }

        private void DrawTooltip()
        {
            if (_tooltipInfo == string.Empty) return;
            GUIStyle tooltipStyle = new GUIStyle("label");
            tooltipStyle.clipping = TextClipping.Overflow;
            Rect tooltipRect = new Rect(Event.current.mousePosition.x + 10, Event.current.mousePosition.y + 10, 220, 40);
            //GUI.Box(tooltipRect, string.Empty, DialogueEditorGUI.gui.GetStyle("box_opaque"));
            GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 5, tooltipRect.width - 10, tooltipRect.height - 10), _tooltipInfo, tooltipStyle);
        }
        #endregion

        #region Node Utilities
        private void DrawOutputConnector(DialogueEditorNodeObject node, Vector2 position, int outputIndex)
        {
            Rect outputButtonRect = new Rect(position.x, position.y, 16, 16);
            //string outputButtonType = (node.outs[outputIndex] != null) ? "connector_full" : "connector_empty";
            if (Event.current.type == EventType.MouseDown && outputButtonRect.Contains(Event.current.mousePosition))
            {
               
                if (Event.current.button == 0)
                {
                    //Debug.Log(node.outs[].ToString());
                    node.outs[outputIndex] = null;
                    _outputSelection = new DialogueEditorSelectionObject(node.id, outputIndex);
                }
                else if (Event.current.button == 1)
                {
                    node.outs[outputIndex] = null;
                    if (_outputSelection != null && _outputSelection.nodeId == node.id && _outputSelection.outputIndex == outputIndex)
                    {
                        _outputSelection = null;
                    }
                }


            }
            GUI.Button(outputButtonRect, string.Empty, new GUIStyle(GUI.skin.GetStyle("IN Foldout")));
        }
        private Vector2 GetNodeOutputPosition(int id, int outputIndex)
        {
            DialogueEditorNodeObject node = _dialogueData.data.dialogues[currentDialogue].nodes[id];
            Vector2 position = node.position;

            switch (node.type)
            {
                case DialogueEditorNodeTypes.MessageNode:
                    position.x += 280;
                    position.y += 40;
                    break;

                case DialogueEditorNodeTypes.BranchingMessageNode:
                    //Debug.Log("phaseId: " + phaseId + "; outputIndex: " + outputIndex +";");
                    position.x += 280;
                    position.y += 147 + (60 * outputIndex);
                    break;

                case DialogueEditorNodeTypes.SetVariableNode:
                    position.x += 280;
                    position.y += 210;
                    break;

                case DialogueEditorNodeTypes.ConditionalNode:
                    position.x += 280;
                    position.y += 210 + (outputIndex * 80);
                    break;

                case DialogueEditorNodeTypes.GenericEventNode:
                    position.x += 180;
                    position.y += 40;
                    break;


            }

            return position;
        }

        private void GetNodeScrollLimits()
        {
            if (_dialogueData.data == null || _dialogueData.data.dialogues.Count < 1) return;

            int padding = 500;
            DialogueEditorDialogueObject dialogue = _dialogueData.data.dialogues[currentDialogue];
            _scrollLimits = Vector2.zero;
            for (int i = 0; i < dialogue.nodes.Count; i += 1)
            {
                if (dialogue.nodes[i].position.x > (_scrollLimits.x - padding)) _scrollLimits.x = dialogue.nodes[i].position.x + padding;
                if (dialogue.nodes[i].position.y > (_scrollLimits.y - padding)) _scrollLimits.y = dialogue.nodes[i].position.y + padding;
            }
        }

        private Vector2 GetScrollPosition()
        {
            Vector2 currentScrollPosition;
            if (_dialogueData.data.dialogues.Count > 0 && currentDialogue < _dialogueData.data.dialogues.Count)
            {
                currentScrollPosition = _dialogueData.data.dialogues[currentDialogue].scrollPosition;
            }
            else
            {
                currentScrollPosition = Vector2.zero;
            }
            return currentScrollPosition;
        }
        #endregion

        #region Handle Inputs
        private void HandleTabKey()
        {
            bool shift = Event.current.shift;
            if (Event.current.isKey && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab)
            {
                if (shift)
                {
                    currentDialogue -= 1;
                }
                else
                {
                    currentDialogue += 1;
                }
            }
        }

        private void HandleNodeInputClicked(int nodeId)
        {

            DialogueEditorDialogueObject dialogue = _dialogueData.data.dialogues[currentDialogue];
            if (_outputSelection != null)
            {
                if (_outputSelection.isStart)
                {
                    dialogue.startPage = nodeId;
                }
                else
                {
                    DialogueEditorNodeObject selectedNode = dialogue.nodes[_outputSelection.nodeId];
                    if (nodeId != selectedNode.id) selectedNode.outs[_outputSelection.outputIndex] = nodeId;
                }
            }
            _outputSelection = null;
        }

        private void HandleNodeRemoveClick(int nodeId)
        {
            DialogueEditorDialogueObject dialogue = _dialogueData.data.dialogues[currentDialogue];

            if (dialogue.startPage.HasValue)
            {
                if (dialogue.startPage.Value == nodeId) dialogue.startPage = null;
            }

            dialogue.RemoveNode(nodeId);
        }

        private void HandleDragging()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            DialogueEditorNodeObject node = null;
            if (Event.current.type == EventType.MouseDrag && _dragSelection != null)
            {
                node = _dialogueData.data.dialogues[currentDialogue].nodes[_dragSelection.nodeId];
                node.position = new Vector2((mousePosition.x + _dragSelection.mouseOffset.x), (mousePosition.y + _dragSelection.mouseOffset.y));
                if (node.position.x < 10) node.position.x = 10;
                if (node.position.y < 10) node.position.y = 10;
            }

            if (Event.current.type == EventType.MouseUp && _dragSelection != null)
            {
                node = _dialogueData.data.dialogues[currentDialogue].nodes[_dragSelection.nodeId];
                if (node != null)
                {
                    GetNodeScrollLimits();
                }
                _dragSelection = null;
            }
        }

        private void HandleMouse()
        {
            if (Event.current.type == EventType.MouseUp)
            {
                _dragSelection = null;
                _outputSelection = null;
            }
        }

        private void HandleCanvasMiddleMouseDrag()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                _panningMousePosition = Event.current.mousePosition;
                _panning = true;
            }

            if (Event.current.type == EventType.MouseUp)
            {
                _panningMousePosition = Vector2.zero;
                _panning = false;
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                if (Event.current.button == 2)
                {
                    DialogueEditorDialogueObject dialogue = _dialogueData.data.dialogues[currentDialogue];
                    //Debug.Log("Dialogue Editor X: " + dialogue.scrollPosition.x);
                    //Debug.Log("Mouse X: " + Event.current.mousePosition.x);
                    dialogue.scrollPosition.x -= (Event.current.mousePosition.x - _panningMousePosition.x) * 1.1f;
                    dialogue.scrollPosition.y -= (Event.current.mousePosition.y - _panningMousePosition.y) * 1.1f;
                }
            }
        }
        #endregion

        #region Node Drawing
        public void DrawDebugNode(DialogueEditorNodeObject node)
        {
            // DrawNodeBase(node, 200, 200);
        }
        // BASE
        private Rect DrawNodeBase(DialogueEditorNodeObject node, int width, int height)
        {
            Rect box = new Rect(node.position.x, node.position.y, width, height);
            GUI.Box(box, string.Empty);

            Rect titleBarRect = new Rect(box.x + 2, box.y + 2, box.width - 4, 20);

            GUI.Box(titleBarRect, string.Empty);
            GUI.Box(titleBarRect, string.Empty);
            
            GUIStyle titleBarStyle = new GUIStyle("label");
            titleBarStyle.alignment = TextAnchor.MiddleCenter;

            Vector2 mousePosition = Event.current.mousePosition;
            if (titleBarRect.Contains(mousePosition))
            {

                GUI.Box(titleBarRect, string.Empty);
                GUI.Box(titleBarRect, string.Empty);
                
            }

            GUI.color = GUI.contentColor;
            GUI.Label(new Rect(titleBarRect.x, titleBarRect.y - 1, titleBarRect.width, titleBarRect.height), DialogueEditorNodeType.GetNodes()[(int)node.type].name, titleBarStyle);


            // Input
            Rect inputButtonRect = new Rect(titleBarRect.x + 2, titleBarRect.y + 2, 16, 16);
            //if(Event.current.type == EventType.MouseUp && inputButtonRect.Contains(mousePosition)){
            if (GUI.Button(inputButtonRect, string.Empty, new GUIStyle(GUI.skin.GetStyle("OL Toggle"))))
            {
                HandleNodeInputClicked(node.id);
                Event.current.Use();
            }
                
            //GUI.Button(inputButtonRect, string.Empty, DialogueEditorGUI.gui.GetStyle("connector_input"));

            // Close
            Rect closeButtonRect = new Rect(titleBarRect.x + titleBarRect.width - 24, titleBarRect.y + 2, 20, 16);
            if (GUI.Button(closeButtonRect, "X"))
            {
                HandleNodeRemoveClick(node.id);
                Event.current.Use();
            }
            if (closeButtonRect.Contains(Event.current.mousePosition))
            {
               SetTooltip("This button deletes the current page.\nThis can NOT be undone.");
            }

            if (Event.current.type == EventType.MouseDown && titleBarRect.Contains(mousePosition))
            {
                _dragSelection = new DialogueEditorDragNodeObject(node.id, new Vector2(titleBarRect.x - mousePosition.x, titleBarRect.y - mousePosition.y));
            }

            return new Rect(box.x, box.y, width, 25);
        }

        // VARIABLE BASE
        private Rect DrawVariableNodeBase(DialogueEditorNodeObject node, int height)
        {
            int width = 300;
            Rect baseRect = DrawNodeBase(node, width, height);

            DialogueEditorVariablesContainer variables;

            if (node.variableScope == VariableEditorScopes.Global)
            {
                if (node.variableType == VariableEditorTypes.Float)
                {
                    variables = _dialogueData.data.globals.floats;
                }
                else if (node.variableType == VariableEditorTypes.String)
                {
                    variables = _dialogueData.data.globals.strings;
                }
                else
                {
                    variables = _dialogueData.data.globals.booleans;
                }
            }
            else
            {
                if (node.variableType == VariableEditorTypes.Float)
                {
                    variables = _dialogueData.data.dialogues[currentDialogue].floats;
                }
                else if (node.variableType == VariableEditorTypes.String)
                {
                    variables = _dialogueData.data.dialogues[currentDialogue].strings;
                }
                else
                {
                    variables = _dialogueData.data.dialogues[currentDialogue].booleans;
                }
            }

            Rect topRowRect = new Rect(baseRect.x + 5, baseRect.yMax, width - 10, 25);
            if (GUI.Toggle(new Rect(topRowRect.x, topRowRect.y, (width - 10) * 0.5f, 25), (node.variableScope == VariableEditorScopes.Global), "Global"))
            {
                node.variableScope = VariableEditorScopes.Global;
                if (node.variableId >= variables.variables.Count) node.variableId = 0;
            }
            if (GUI.Toggle(new Rect(topRowRect.x + (topRowRect.width * 0.5f), topRowRect.y, (width - 10) * 0.5f, 25), (node.variableScope == VariableEditorScopes.Local), "Local"))
            {
                node.variableScope = VariableEditorScopes.Local;
                if (node.variableId >= variables.variables.Count) node.variableId = 0;
            }

            Rect typesRect = new Rect(topRowRect.x, topRowRect.yMax + 5, width - 10, 25);
            Rect typeBooleanToggleRect = new Rect(typesRect.x, typesRect.y, typesRect.width * 0.33333f, typesRect.height);
            Rect typeFloatToggleRect = new Rect(typesRect.x + (typesRect.width * 0.33333f), typesRect.y, typesRect.width * 0.33333f, typesRect.height);
            Rect typeStringToggleRect = new Rect(typesRect.x + ((typesRect.width * 0.33333f) * 2), typesRect.y, typesRect.width * 0.33333f, typesRect.height);

            if (GUI.Toggle(typeBooleanToggleRect, (node.variableType == VariableEditorTypes.Boolean), "Booleans"))
            {
                node.variableType = VariableEditorTypes.Boolean;
                if (node.variableId >= variables.variables.Count) node.variableId = 0;
            }
            if (GUI.Toggle(typeFloatToggleRect, (node.variableType == VariableEditorTypes.Float), "Floats"))
            {
                node.variableType = VariableEditorTypes.Float;
                if (node.variableId >= variables.variables.Count) node.variableId = 0;
            }
            if (GUI.Toggle(typeStringToggleRect, (node.variableType == VariableEditorTypes.String), "Strings"))
            {
                node.variableType = VariableEditorTypes.String;
                if (node.variableId >= variables.variables.Count) node.variableId = 0;
            }

            // ------------------ SCROLL BOX
            // VISUALS
            Rect scrollRect = new Rect(typesRect.x + 2, typesRect.yMax + 7, width - 14, 100);

            GUI.Box(scrollRect,  string.Empty);
            
            // MOUSE HANDLING
            int rowHeight = 20;
            int rowSpacing = (EditorGUIUtility.isProSkin) ? 1 : -1;
            int newScrollHeight = (scrollRect.height > ((rowHeight + rowSpacing) * variables.variables.Count)) ? (int)scrollRect.height : (rowHeight + rowSpacing) * variables.variables.Count;
            Rect scrollContentRect = new Rect(0, 0, scrollRect.width - 15, newScrollHeight);
            Vector2 mouseClickPosition = Vector2.zero;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scrollRect.Contains(Event.current.mousePosition))
            {
                mouseClickPosition = new Vector2(Event.current.mousePosition.x - scrollRect.x - 3, Event.current.mousePosition.y - scrollRect.y - 3 + node.variableScrollPosition.y);
            }
            //START SCROLL VIEW
            node.variableScrollPosition = GUI.BeginScrollView(scrollRect, node.variableScrollPosition, scrollContentRect, false, true);

            //GUI.color = (isPro) ? new Color(1, 1, 1, 0.25f) : new Color(1, 1, 1, 0.1f);
            //GUI.DrawTextureWithTexCoords(
            //    scrollContentRect,
            //    DialogueEditorGUI.scrollboxBgTexture,
            //    new Rect(0, 0, scrollContentRect.width / DialogueEditorGUI.scrollboxBgTexture.width, scrollContentRect.height / DialogueEditorGUI.scrollboxBgTexture.height)
            //);
            //GUI.color = GUI.contentColor;

            for (int i = 0; i < variables.variables.Count; i += 1)
            {
                Rect row = new Rect(0, 0 + ((rowHeight + rowSpacing) * i), scrollRect.width - 15, 20);
                if (mouseClickPosition != Vector2.zero && row.Contains(mouseClickPosition))
                {
                    node.variableId = i;
                }
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.Box(row, string.Empty);
                if (i == node.variableId)
                {
                    GUI.Box(row, string.Empty);
                    GUI.Box(row, string.Empty);
                    
                }

                if (row.Contains(Event.current.mousePosition))
                {
                    GUI.color = Color.white;
                    GUI.Box(row, string.Empty);
                    GUI.Box(row, string.Empty);
                    GUI.Box(row, string.Empty);
                    
                }

                GUI.color = GUI.contentColor;

                Rect labelNumberRow = new Rect(row.x + 2, row.y + 2, row.width - 4, row.height - 4);
                Rect labelNameRow = new Rect(labelNumberRow.x + 25, labelNumberRow.y, labelNumberRow.width - 25, labelNumberRow.height);
                GUI.Label(labelNumberRow, variables.variables[i].id.ToString());
                string labelNameText = (variables.variables[i].variableName != string.Empty) ? variables.variables[i].variableName : string.Empty;
                GUI.Label(labelNameRow, labelNameText);
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.Label(labelNameRow, (variables.variables[i].variable != string.Empty) ? labelNameText + ": " + variables.variables[i].variable : string.Empty);
                GUI.color = GUI.contentColor;
            }
            // END SCROLL VIES
            GUI.EndScrollView();

            Rect outputRect = new Rect(baseRect.x, baseRect.y, width, 195);
            //DialogueEditorGUI.drawShadowedRect(outputRect);
            return outputRect;
        }

        // TEXT BASE
        private Rect DrawTextNodeBase(DialogueEditorNodeObject node, int height)
        {
            int width = 300;
            Rect baseRect = DrawNodeBase(node, width, height);

            return new Rect(baseRect.x, baseRect.y, width, baseRect.height);
        }

        // TEXT ADVANCED
        private Rect DrawTextNodeAdvanced(DialogueEditorNodeObject node, int y, int width)
        {

            Rect baseRect = new Rect(node.position.x + 5, node.position.y + y, width - 10, 0);

            if (node.advanced)
            {
                Rect advancedRect = new Rect(baseRect.x + 5, baseRect.y + baseRect.height, baseRect.width - 10, 26);

                Rect separatorBox = new Rect(baseRect.x, baseRect.y + 2, baseRect.width, 298);

                GUI.Box(separatorBox, string.Empty);
                

                Rect titleBox = new Rect(baseRect.x + 5, baseRect.y + 8, baseRect.width - 10, 24);

                GUI.Box(titleBox, string.Empty);
                
                GUI.Label(new Rect(titleBox.x + 4, titleBox.y + 4, titleBox.width - 10, 20), "Advanced");

                Rect varBox = new Rect(baseRect.x + 5, titleBox.yMax + 5, baseRect.width - 10 - 210 - 5, 26);

                GUI.Box(varBox, string.Empty);
                
                GUI.Label(new Rect(varBox.x + 6, varBox.y + 5, varBox.width - 10, 20), "Variables");
                Rect globalVariableStringsRect = new Rect(varBox.xMax + 5, varBox.y + 3, 210, varBox.height - 6);
                if (GUI.Button(new Rect(globalVariableStringsRect.x + ((globalVariableStringsRect.width / 3) * 0), globalVariableStringsRect.y, globalVariableStringsRect.width / 3, globalVariableStringsRect.height), "Boolean")) { node.text += "<" + NodeVarSubStrings.GLOBAL + NodeVarSubStrings.BOOLEAN + ">" + "0" + "</" + NodeVarSubStrings.GLOBAL + NodeVarSubStrings.BOOLEAN + ">"; }
                if (GUI.Button(new Rect(globalVariableStringsRect.x + ((globalVariableStringsRect.width / 3) * 1), globalVariableStringsRect.y, globalVariableStringsRect.width / 3, globalVariableStringsRect.height), "Float")) { node.text += "<" + NodeVarSubStrings.GLOBAL + NodeVarSubStrings.FLOAT + ">" + "0" + "</" + NodeVarSubStrings.GLOBAL + NodeVarSubStrings.FLOAT + ">"; }
                if (GUI.Button(new Rect(globalVariableStringsRect.x + ((globalVariableStringsRect.width / 3) * 2), globalVariableStringsRect.y, globalVariableStringsRect.width / 3, globalVariableStringsRect.height), "String")) { node.text += "<" + NodeVarSubStrings.GLOBAL + NodeVarSubStrings.STRING + ">" + "0" + "</" + NodeVarSubStrings.GLOBAL + NodeVarSubStrings.STRING + ">"; }

                /*
                Rect themeBox = new Rect(baseRect.x + 5, varBox.yMax + 5, baseRect.width - 10, 26);
                if(isPro){
                    DialogueEditorGUI.drawShadowedRect(themeBox,2);
                }else{
                    GUI.Box(themeBox, string.Empty, DialogueEditorGUI.gui.GetStyle("box_outset"));
                }
                GUI.Label(new Rect(themeBox.x + 6, themeBox.y + 5, themeBox.width - 10, 20), "Theme");
                string[] themes = DialogueEditorDataManager.data.getThemeNames();
                Rect themeBoxPopupRect = new Rect(themeBox.x + 47 + 5, themeBox.y+5, themeBox.width - 10 - 47 - 90, 20);
                phase.theme = EditorGUI.Popup(themeBoxPopupRect, phase.theme, themes);
			
                Rect newWindowToggleRect = new Rect(themeBoxPopupRect.xMax + 5, themeBoxPopupRect.y - 2, themeBoxPopupRect.width, 26);
                phase.newWindow = GUI.Toggle(newWindowToggleRect, phase.newWindow, "New Window");
                */

                Rect themeBox = new Rect(baseRect.x + 5, varBox.yMax + 5, baseRect.width - 10, 26);

                GUI.Box(themeBox, string.Empty);
                
                GUI.Label(new Rect(themeBox.x + 4, themeBox.y + 5, 100, 20), "Theme:");
                Rect themeTextFieldRect = new Rect(themeBox.x + 65, themeBox.y + 5, themeBox.width - 65 - 5, themeBox.height - 10);

                GUI.Box(themeTextFieldRect, string.Empty);
                
                if (node.theme == null) node.theme = string.Empty;
                node.theme = GUI.TextField(themeTextFieldRect, node.theme);

                //Rect newWindowToggleRect = new Rect(themeTextFieldRect.xMax + 5, themeTextFieldRect.y, 90, 26);
                //node.newWindow = GUI.Toggle(newWindowToggleRect, node.newWindow, "New Window");


                Rect nameRect = new Rect(themeBox.x, themeBox.yMax + 5, themeBox.width, 26);

                GUI.Box(nameRect, string.Empty);
                
                //GUI.Label(new Rect(nameRect.x + 4, nameRect.y + 5, 100, 20), "Name:");
                Rect nameTextFieldRect = new Rect(nameRect.x + 4, nameRect.y + 5, nameRect.width - 5, nameRect.height - 10);

                node.character = EditorGUI.ObjectField(nameTextFieldRect, "Character:", node.character, typeof(Character), false) as Character;


                Rect audioRect = new Rect(nameRect.x, nameRect.yMax + 5, nameRect.width, 26);

                GUI.Box(audioRect, string.Empty);
                
                //GUI.Label(new Rect(portraitRect.x + 4, portraitRect.y + 5, 100, 20), "Portrait:");
                Rect audioTextFieldRect = new Rect(audioRect.x + 4, audioRect.y + 5, audioRect.width - 5, audioRect.height - 10);

                node.audio = EditorGUI.ObjectField(audioTextFieldRect, "Audio:", node.audio, typeof(AudioClip), false) as AudioClip;
                //GUI.Box(portraitTextFieldRect, string.Empty);

                //TODO: Display the picture!
                //if (node.character.portrait2D == null) node.character.portrait2D = null;
                //node.character.portrait2D = GUI.b(portraitTextFieldRect, node.character.portrait2D);

                Rect metadataRect = new Rect(audioRect.x, audioRect.yMax + 5, audioRect.width, 26);

                GUI.Box(metadataRect, string.Empty);
                
                GUI.Label(new Rect(metadataRect.x + 4, metadataRect.y + 5, 100, 20), "Metadata:");
                Rect metadataTextFieldRect = new Rect(metadataRect.x + 65, metadataRect.y + 5, metadataRect.width - 65 - 5, metadataRect.height - 10);

                GUI.Box(metadataTextFieldRect, string.Empty);
                
                if (node.metadata == null) node.metadata = string.Empty;
                node.metadata = GUI.TextField(metadataTextFieldRect, node.metadata);

                Rect waitRect = new Rect(metadataRect.x, metadataRect.yMax + 5, metadataRect.width, 48);

                GUI.Box(waitRect, string.Empty);
                
                GUI.Label(new Rect(waitRect.x + 4, waitRect.y + 5, 100, 20), "Wait:");
                Rect waitTextFieldRect = new Rect(waitRect.x + 65, waitRect.y + 5, waitRect.width - 65 - 5, 16);
                GUI.Box(waitTextFieldRect, string.Empty);
                
                // FIX THIS SHIT
                //phase.audio = EditorGUI.ObjectField(audioFileFieldRect, phase.audio, typeof(AudioClip), false) as AudioClip;
                //if (node.audio == null) node.audio = string.Empty;
                //node.audio = GUI.TextField(audioTextFieldRect, node.audio);

                //GUI.Label(new Rect(audioRect.x + 4, audioTextFieldRect.yMax + 5, 100, 20), "Delay:");
                //Rect audioDelayTextFieldRect = new Rect(audioRect.x + 65, audioTextFieldRect.yMax + 5, audioRect.width - 65 - 5, 16);
                //if (isPro)
                //{
                //    DialogueEditorGUI.drawHighlightRect(audioDelayTextFieldRect, 1, 1);
                //}
                //else
                //{
                //    GUI.Box(DialogueEditorGUI.getOutlineRect(audioDelayTextFieldRect, 1), string.Empty, DialogueEditorGUI.gui.GetStyle("box_inset"));
                //}
                //node.audioDelay = EditorGUI.FloatField(audioDelayTextFieldRect, node.audioDelay, GUI.skin.GetStyle("textfield"));


                Rect nodeRect = node.rect;
                Rect rectRect = new Rect(waitRect.x, waitRect.yMax + 5, waitRect.width, 50);

                GUI.Box(rectRect, string.Empty);
                

                Rect xRect = new Rect(rectRect.x + 20, rectRect.y + 7, 100, 16);
                GUI.Label(new Rect(xRect.x - 15, xRect.y, xRect.width, xRect.height), "X");

                Rect yRect = new Rect(rectRect.x + 20, rectRect.y + 7 + 16 + 5, 100, 16);
                GUI.Label(new Rect(yRect.x - 15, yRect.y, yRect.width, yRect.height), "Y");

                Rect wRect = new Rect(rectRect.xMax - 106, rectRect.y + 7, 100, 16);
                GUI.Label(new Rect(wRect.x - 35, wRect.y, wRect.width, wRect.height), "Width");

                Rect hRect = new Rect(rectRect.xMax - 106, rectRect.y + 7 + 16 + 5, 100, 16);
                GUI.Label(new Rect(hRect.x - 40, hRect.y, hRect.width, hRect.height), "Height");



                GUI.Box(xRect, string.Empty);
                GUI.Box(yRect, string.Empty);
                GUI.Box(wRect, string.Empty);
                GUI.Box(hRect, string.Empty);
                

                nodeRect.x = EditorGUI.IntField(xRect, (int)nodeRect.x);
                nodeRect.y = EditorGUI.IntField(yRect, (int)nodeRect.y);
                nodeRect.width = EditorGUI.IntField(wRect, (int)nodeRect.width);
                nodeRect.height = EditorGUI.IntField(hRect, (int)nodeRect.height);
                //DialogueEditorGUI.drawHighlightRect(new Rect(rectRect.x + 40,rectRect.y + 5, 98, 32), 1, 1);
                //DialogueEditorGUI.drawHighlightRect(new Rect(rectRect.x + 178,rectRect.y + 5, 97, 32), 1, 1);
                //if(phase.rect == null) phase.rect = new Rect(0,0,0,0);
                //phase.rect = EditorGUI.RectField(DialogueEditorGUI.getOutlineRect(rectRect, -5), phase.rect);
                node.rect = nodeRect;


                //Rect otherRect = new Rect(themeBox.x + themeBox.width + 5, themeBox.y, 100 - 15, themeBox.height);
                //DialogueEditorGUI.drawShadowedRect(otherRect);

                return new Rect(baseRect.x, baseRect.y, width, advancedRect.height + baseRect.height + 5);
            }
            else
            {
                return new Rect(baseRect.x, baseRect.y, width, baseRect.height);
            }
        }

        // draw TEXT
        private void DrawMessage(DialogueEditorNodeObject node)
        {
            int advancedHeight = (node.advanced) ? 303 : 0;
            Rect baseRect = DrawTextNodeBase(node, 132 + advancedHeight);

            Rect textBoxRect = new Rect(baseRect.x + 5, baseRect.y + baseRect.height, baseRect.width - 10, 102);
            Rect textBoxTitleRect = new Rect(textBoxRect.x + 5, textBoxRect.y + 5, textBoxRect.width - 10 - 100, 20);

            GUI.Box(textBoxRect, string.Empty);
            GUI.Box(textBoxTitleRect, string.Empty);
            
            GUI.Label(new Rect(textBoxTitleRect.x + 3, textBoxTitleRect.y + 2, textBoxTitleRect.width - 2, 20), "Text:");

            //GUI.Box(DialogueEditorGUI.getOutlineRect(textBoxRect,1), string.Empty, DialogueEditorGUI.gui.GetStyle("box_inset"));

            Rect advancedButtonRect = new Rect(textBoxTitleRect.xMax + 5, textBoxRect.y + 6, 70, textBoxTitleRect.height);
            node.advanced = GUI.Toggle(advancedButtonRect, node.advanced, "Advanced");


            Rect textFieldRect = new Rect(textBoxRect.x + 6, textBoxRect.y + 6 + textBoxTitleRect.height + 4, textBoxRect.width - 12, textBoxRect.height - textBoxTitleRect.height - 16);

            GUI.Box(textFieldRect, string.Empty);
            
            node.text = GUI.TextArea(textFieldRect, node.text);

            Rect outputButtonBackRect = new Rect(textBoxTitleRect.x + textBoxRect.width - 30, textBoxTitleRect.y, textBoxTitleRect.height, textBoxTitleRect.height);

            GUI.Box(outputButtonBackRect, string.Empty);
            

            Rect outputButtonRect = new Rect(textBoxTitleRect.x + textBoxRect.width - 28, textBoxTitleRect.y + 2, 16, 16);

           
            DrawOutputConnector(node, new Vector2(outputButtonRect.x, outputButtonRect.y), 0);

            DrawTextNodeAdvanced(node, 130, (int)baseRect.width);
        }

        // draw BRANCHED TEXT
        private void DrawBranchedMessageNode(DialogueEditorNodeObject node)
        {
            int choiceRectHeight = 55;
            int advancedHeight = (node.advanced) ? 303 : 0;
            Rect baseRect = DrawTextNodeBase(node, 132 + advancedHeight + ((choiceRectHeight + 5) * node.outs.Count));

            Rect textBoxRect = new Rect(baseRect.x + 5, baseRect.y + baseRect.height, baseRect.width - 10, 102);
            Rect textBoxTitleRect = new Rect(textBoxRect.x + 5, textBoxRect.y + 5, textBoxRect.width - 10 - 105 - 80, 20);

            GUI.Box(textBoxRect, string.Empty);
            GUI.Box(textBoxTitleRect, string.Empty);
            
            GUI.Label(new Rect(textBoxTitleRect.x + 3, textBoxTitleRect.y + 2, textBoxTitleRect.width - 2, 20), "Body Text");
            Rect textFieldRect = new Rect(textBoxRect.x + 6, textBoxRect.y + 4 + textBoxTitleRect.height + 6, textBoxRect.width - 12, textBoxRect.height - textBoxTitleRect.height - 16);

            GUI.Box(textFieldRect,  string.Empty);
            
            node.text = GUI.TextArea(textFieldRect, node.text);

            Rect advancedButtonRect = new Rect(textBoxTitleRect.xMax + 5, textBoxRect.y + 6, 70, textBoxTitleRect.height);
            node.advanced = GUI.Toggle(advancedButtonRect, node.advanced, "Advanced");

            Rect buttonsRect = new Rect(baseRect.xMax - 110, textBoxTitleRect.y, 100, 20);

            Rect addButtonRect = new Rect(buttonsRect.x, buttonsRect.y, buttonsRect.width * 0.5f, buttonsRect.height);
            if (node.outs.Count < 10)
            {
                if (GUI.Button(addButtonRect, "Add"))
                {
                    node.AddNewChoice();
                }
            }
            else
            {
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.Button(addButtonRect, "Add");
                GUI.color = GUI.contentColor;
            }

            Rect removeButtonRect = new Rect(buttonsRect.x + (buttonsRect.width * 0.5f), buttonsRect.y, buttonsRect.width * 0.5f, buttonsRect.height);
            if (node.outs.Count > 2)
            {
                if (GUI.Button(removeButtonRect, "Remove"))
                {
                    node.RemoveChoice();
                }
            }
            else
            {
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.Button(removeButtonRect, "Remove");
                GUI.color = GUI.contentColor;
            }

            for (int i = 0; i < node.outs.Count; i += 1)
            {
                Rect outerChoiceRect = new Rect(baseRect.x + 5, textBoxRect.yMax + 5 + ((choiceRectHeight + 5) * i), baseRect.width - 10, choiceRectHeight);
                Rect choiceTitleRect = new Rect(outerChoiceRect.x + 5, outerChoiceRect.y + 5, outerChoiceRect.width - 10, 20);

                GUI.Box(outerChoiceRect, string.Empty);
                GUI.Box(choiceTitleRect, string.Empty);
                
                GUI.Label(new Rect(choiceTitleRect.x + 2, choiceTitleRect.y + 2, choiceTitleRect.width, choiceTitleRect.height), "Choice " + (i + 1));

                Rect choiceRect = new Rect(choiceTitleRect.x + 2, choiceTitleRect.yMax + 5 + 2, choiceTitleRect.width - 4, 17);
                GUI.Box(choiceRect, string.Empty);
                
                node.choices[i] = GUI.TextField(choiceRect, node.choices[i]);

                Rect outputButtonRect = new Rect(choiceTitleRect.x + choiceTitleRect.width - 18, choiceTitleRect.y + 2, 16, 16);
                DrawOutputConnector(node, new Vector2(outputButtonRect.x, outputButtonRect.y), i);
            }

            DrawTextNodeAdvanced(node, 130 + (node.outs.Count * 60), (int)baseRect.width);
        }

        // draw SET VARIABLE
        private void DrawSetVariableNode(DialogueEditorNodeObject node)
        {
            Rect baseRect = DrawVariableNodeBase(node, 288);

            Rect setEditorRect = new Rect(baseRect.x + 5, baseRect.yMax, baseRect.width - 10, 87);
            Rect setEditorTitleRect = new Rect(setEditorRect.x + 5, setEditorRect.y + 5, setEditorRect.width - 10, 20);

            GUI.Box(setEditorRect, string.Empty);
            GUI.Box(setEditorTitleRect,string.Empty);
            

            GUI.Label(new Rect(setEditorTitleRect.x + 2, setEditorTitleRect.y + 2, setEditorTitleRect.width, 20), "Output");

            Rect equationTypeRect = new Rect(setEditorTitleRect.x, setEditorTitleRect.yMax + 5, setEditorTitleRect.width, 25);
            Rect inputRect = new Rect(equationTypeRect.x, equationTypeRect.yMax + 5, equationTypeRect.width, 20);
            if (node.variableType == VariableEditorTypes.Boolean)
            {
                //FOR BOOLEANS
                if (node.variableSetEquation != VariableEditorSetEquation.Equals && node.variableSetEquation != VariableEditorSetEquation.Toggle)
                {
                    node.variableSetEquation = VariableEditorSetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 0), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableSetEquation == VariableEditorSetEquation.Equals), "="))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 1), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableSetEquation == VariableEditorSetEquation.Toggle), "Toggle"))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Toggle;
                    node.variableSetValue = "toggle";
                }
            }
            else if (node.variableType == VariableEditorTypes.Float)
            {
                //FOR FLOATS
                if (node.variableSetEquation != VariableEditorSetEquation.Equals && node.variableSetEquation != VariableEditorSetEquation.Add && node.variableSetEquation != VariableEditorSetEquation.Subtract && node.variableSetEquation != VariableEditorSetEquation.Multiply && node.variableSetEquation != VariableEditorSetEquation.Divide)
                {
                    node.variableSetEquation = VariableEditorSetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.2f) * 0), equationTypeRect.y, (equationTypeRect.width) * 0.2f, 25), (node.variableSetEquation == VariableEditorSetEquation.Equals), "="))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.2f) * 1), equationTypeRect.y, (equationTypeRect.width) * 0.2f, 25), (node.variableSetEquation == VariableEditorSetEquation.Add), "+"))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Add;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.2f) * 2), equationTypeRect.y, (equationTypeRect.width) * 0.2f, 25), (node.variableSetEquation == VariableEditorSetEquation.Subtract), "-"))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Subtract;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.2f) * 3), equationTypeRect.y, (equationTypeRect.width) * 0.2f, 25), (node.variableSetEquation == VariableEditorSetEquation.Multiply), "x"))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Multiply;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.2f) * 4), equationTypeRect.y, (equationTypeRect.width) * 0.2f, 25), (node.variableSetEquation == VariableEditorSetEquation.Divide), "/"))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Divide;
                }
            }
            else if (node.variableType == VariableEditorTypes.String)
            {
                // FOR STRINGS
                if (node.variableSetEquation != VariableEditorSetEquation.Equals && node.variableSetEquation != VariableEditorSetEquation.Add)
                {
                    node.variableSetEquation = VariableEditorSetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 0), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableSetEquation == VariableEditorSetEquation.Equals), "="))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 1), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableSetEquation == VariableEditorSetEquation.Add), "+"))
                {
                    node.variableSetEquation = VariableEditorSetEquation.Add;
                }
            }

            if (node.variableType == VariableEditorTypes.Boolean)
            {
                if (GUI.Toggle(new Rect(inputRect.x + ((inputRect.width * 0.5f) * 0), inputRect.y, (inputRect.width) * 0.5f, inputRect.height), (node.variableSetValue == "true"), "True"))
                {
                    node.variableSetValue = "true";
                }
                if (GUI.Toggle(new Rect(inputRect.x + ((inputRect.width * 0.5f) * 1), inputRect.y, (inputRect.width) * 0.5f, inputRect.height), (node.variableSetValue == "false"), "False"))
                {
                    node.variableSetValue = "false";
                }
            }
            else
            {
                Rect newInputRect = new Rect(inputRect.x + 3, inputRect.y + 2, inputRect.width - 6, inputRect.height - 4);

                GUI.Box(newInputRect, string.Empty);
                
                if (node.variableType == VariableEditorTypes.Float)
                {
                    float floatVar;
                    float.TryParse(node.variableSetValue, out floatVar);
                    node.variableSetValue = EditorGUI.FloatField(newInputRect, floatVar).ToString();
                }
                else
                {
                    node.variableSetValue = GUI.TextField(newInputRect, node.variableSetValue);
                }
            }

            Rect outputButtonRect = new Rect(setEditorTitleRect.x + setEditorTitleRect.width - 18, setEditorTitleRect.y + 2, 16, 16);
            DrawOutputConnector(node, new Vector2(outputButtonRect.x, outputButtonRect.y), 0);

        }

        // draw CONDITIONAL
        private void DrawConditionalNode(DialogueEditorNodeObject node)
        {
            Rect baseRect = DrawVariableNodeBase(node, 310);

            Rect ifRect = new Rect(baseRect.x + 5, baseRect.yMax, baseRect.width - 10, 110);
            Rect ifTitleRect = new Rect(ifRect.x + 5, ifRect.y + 5, ifRect.width - 10, 20);

            GUI.Box(ifRect, string.Empty);
            GUI.Box(ifTitleRect, string.Empty);
            

            GUI.Label(new Rect(ifTitleRect.x + 2, ifTitleRect.y + 2, ifTitleRect.width, 20), "If");

            Rect equationTypeRect = new Rect(ifTitleRect.x, ifTitleRect.yMax + 5, ifTitleRect.width, 25);
            Rect inputRect = new Rect(equationTypeRect.x, equationTypeRect.yMax + 5, equationTypeRect.width, 20);
            if (node.variableType == VariableEditorTypes.Boolean)
            {
                //FOR BOOLEANS
                if (node.variableGetEquation != VariableEditorGetEquation.Equals && node.variableGetEquation != VariableEditorGetEquation.NotEquals)
                {
                    node.variableGetEquation = VariableEditorGetEquation.Equals;
                }

                if (node.variableGetValue != "true" && node.variableGetValue != "false")
                {
                    node.variableGetValue = "true";
                }

                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 0), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableGetEquation == VariableEditorGetEquation.Equals), "=="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 1), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableGetEquation == VariableEditorGetEquation.NotEquals), "!="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.NotEquals;
                }
            }
            else if (node.variableType == VariableEditorTypes.Float)
            {
                //FOR FLOATS
                if (node.variableGetEquation != VariableEditorGetEquation.Equals && node.variableGetEquation != VariableEditorGetEquation.NotEquals && node.variableGetEquation != VariableEditorGetEquation.GreaterThan && node.variableGetEquation != VariableEditorGetEquation.LessThan && node.variableGetEquation != VariableEditorGetEquation.EqualOrGreaterThan && node.variableGetEquation != VariableEditorGetEquation.EqualOrLessThan)
                {
                    node.variableGetEquation = VariableEditorGetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width / 6) * 0), equationTypeRect.y, (equationTypeRect.width) / 6, 25), (node.variableGetEquation == VariableEditorGetEquation.Equals), "=="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width / 6) * 1), equationTypeRect.y, (equationTypeRect.width) / 6, 25), (node.variableGetEquation == VariableEditorGetEquation.NotEquals), "!="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.NotEquals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width / 6) * 2), equationTypeRect.y, (equationTypeRect.width) / 6, 25), (node.variableGetEquation == VariableEditorGetEquation.GreaterThan), ">"))
                {
                    node.variableGetEquation = VariableEditorGetEquation.GreaterThan;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width / 6) * 3), equationTypeRect.y, (equationTypeRect.width) / 6, 25), (node.variableGetEquation == VariableEditorGetEquation.LessThan), "<"))
                {
                    node.variableGetEquation = VariableEditorGetEquation.LessThan;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width / 6) * 4), equationTypeRect.y, (equationTypeRect.width) / 6, 25), (node.variableGetEquation == VariableEditorGetEquation.EqualOrGreaterThan), ">="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.EqualOrGreaterThan;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width / 6f) * 5), equationTypeRect.y, (equationTypeRect.width) / 6, 25), (node.variableGetEquation == VariableEditorGetEquation.EqualOrLessThan), "<="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.EqualOrLessThan;
                }
            }
            else if (node.variableType == VariableEditorTypes.String)
            {
                // FOR STRINGS
                if (node.variableGetEquation != VariableEditorGetEquation.Equals && node.variableGetEquation != VariableEditorGetEquation.NotEquals)
                {
                    node.variableGetEquation = VariableEditorGetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 0), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableGetEquation == VariableEditorGetEquation.Equals), "=="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.Equals;
                }
                if (GUI.Toggle(new Rect(equationTypeRect.x + ((equationTypeRect.width * 0.5f) * 1), equationTypeRect.y, (equationTypeRect.width) * 0.5f, 25), (node.variableGetEquation == VariableEditorGetEquation.NotEquals), "!="))
                {
                    node.variableGetEquation = VariableEditorGetEquation.NotEquals;
                }
            }

            if (node.variableType == VariableEditorTypes.Boolean)
            {
                if (GUI.Toggle(new Rect(inputRect.x + ((inputRect.width * 0.5f) * 0), inputRect.y, (inputRect.width) * 0.5f, inputRect.height), (node.variableSetValue == "true"), "True"))
                {
                    node.variableSetValue = "true";
                }
                if (GUI.Toggle(new Rect(inputRect.x + ((inputRect.width * 0.5f) * 1), inputRect.y, (inputRect.width) * 0.5f, inputRect.height), (node.variableSetValue == "false"), "False"))
                {
                    node.variableSetValue = "false";
                }
            }
            else
            {
                Rect newInputRect = new Rect(inputRect.x + 3, inputRect.y + 2, inputRect.width - 6, 18);
                GUI.Box(newInputRect, string.Empty);
                
                if (node.variableType == VariableEditorTypes.Float)
                {
                    float floatVar;
                    float.TryParse(node.variableGetValue, out floatVar);
                    node.variableGetValue = EditorGUI.FloatField(newInputRect, floatVar).ToString();
                }
                else
                {
                    node.variableGetValue = GUI.TextField(newInputRect, node.variableGetValue);
                }
            }

            Rect ifOutputButtonRect = new Rect(ifTitleRect.x + ifTitleRect.width - 18, ifTitleRect.y + 2, 16, 16);
            DrawOutputConnector(node, new Vector2(ifOutputButtonRect.x, ifOutputButtonRect.y), 0);

            Rect elseTitleRect = new Rect(ifTitleRect.x, ifRect.yMax - 25, ifTitleRect.width, ifTitleRect.height);

            GUI.Box(elseTitleRect,string.Empty);
            
            GUI.Label(new Rect(elseTitleRect.x + 2, elseTitleRect.y + 2, elseTitleRect.width, elseTitleRect.height), "Else");
            Rect elseOutputButtonRect = new Rect(elseTitleRect.x + elseTitleRect.width - 18, elseTitleRect.y + 2, 16, 16);
            DrawOutputConnector(node, new Vector2(elseOutputButtonRect.x, elseOutputButtonRect.y), 1);
        }

        // draw SEND MESSAGE
        private void DrawEventNode(DialogueEditorNodeObject node)
        {
            Rect baseRect = DrawNodeBase(node, 200, 135);

            Rect sendMessageRect = new Rect(baseRect.x + 5, baseRect.yMax, baseRect.width - 10, 105);
            Rect sendMessageTitleRect = new Rect(sendMessageRect.x + 5, sendMessageRect.y + 5, sendMessageRect.width - 10, 20);
            Rect messageInputRect = new Rect(sendMessageTitleRect.x + 2, sendMessageTitleRect.yMax + 5, sendMessageTitleRect.width - 4, 18);
            Rect metadataTitleRect = new Rect(sendMessageRect.x + 5, messageInputRect.yMax + 5, sendMessageRect.width - 10, 20);
            Rect metadataInputRect = new Rect(metadataTitleRect.x + 2, metadataTitleRect.yMax + 5, metadataTitleRect.width - 4, 18);

            GUI.Box(sendMessageRect, string.Empty);
            GUI.Box(sendMessageTitleRect, string.Empty);
            GUI.Box(messageInputRect, string.Empty);
            GUI.Box(metadataTitleRect, string.Empty);
            GUI.Box(metadataInputRect, string.Empty);
            

            GUI.Label(new Rect(sendMessageTitleRect.x + 2, sendMessageTitleRect.y + 2, sendMessageTitleRect.width, sendMessageTitleRect.height), "Message Name");
            node.eventName = GUI.TextField(messageInputRect, node.eventName);
            GUI.Label(new Rect(metadataTitleRect.x + 2, metadataTitleRect.y + 2, metadataTitleRect.width, metadataTitleRect.height), "Metadata");
            node.metadata = GUI.TextField(metadataInputRect, node.metadata);

            Rect outputButtonRect = new Rect(sendMessageTitleRect.x + sendMessageTitleRect.width - 18, sendMessageTitleRect.y + 2, 16, 16);
            DrawOutputConnector(node, new Vector2(outputButtonRect.x, outputButtonRect.y), 0);

        }

        // draw END
        private void DrawEndNode(DialogueEditorNodeObject node)
        {
            int width = 100;
            int height = 24;
            DrawNodeBase(node, width, height);
        }
        #endregion

        #region Button Functions

        private void AddDialogue(int numberOfDialoguesToCreate)
        {
            _dialogueData.data.AddDialogue(numberOfDialoguesToCreate, out currentDialogue);
        }

        private void RemoveDialogue(int indexOfDialogue)
        {
            _dialogueData.data.RemoveDialogue(indexOfDialogue, out currentDialogue);
        }

        private void AddNode(DialogueEditorNodeTypes type)
        {
            Vector2 newNodePosition = Vector2.zero;
            newNodePosition.x = _dialogueData.data.dialogues[currentDialogue].scrollPosition.x + 20;
            newNodePosition.y = _dialogueData.data.dialogues[currentDialogue].scrollPosition.y + 20;
            _dialogueData.data.dialogues[currentDialogue].AddNode(type, newNodePosition);
        }
        #endregion
    }

    public class DialogueEditorCurve
    {
        public static void draw(Vector2 startPoint, Vector2 endPoint)
        {

            // Line properties
            float curveThickness = 1.5f;
            Texture2D bezierTexture = Resources.Load("Textures/GUI/bezier_texture") as Texture2D;
            //Texture2D bezierTexture = null;

            // Create shadow start and end points
            Vector2 shadowStartPoint = new Vector2(startPoint.x + 1, startPoint.y + 2);
            Vector2 shadowEndPoint = new Vector2(endPoint.x + 1, endPoint.y + 2);

            /*
            // UNCHANGING
            // Calculate tangents based on distance from startPoint to endPoint, 60 being the max
            float tangent = 60;
            int check = (startPoint.x < endPoint.x) ? 1 : -1 ;
            Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
            Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y);
            Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
            Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y);
            */

            /*
            // FLIPPY
            // Calculate tangents based on distance from startPoint to endPoint, 60 being the max
            float tangent = 60;
            tangent = (Vector2.Distance(startPoint, endPoint) < tangent) ? Vector2.Distance(startPoint, endPoint) : 60 ;
            int check = (startPoint.x < endPoint.x) ? 1 : -1 ;
            Vector2 startTangent = new Vector2(startPoint.x + (tangent * check), startPoint.y);
            Vector2 endTangent = new Vector2(endPoint.x - (tangent * check), endPoint.y);
            Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + (tangent * check), shadowStartPoint.y);
            Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - (tangent * check), shadowEndPoint.y);
            */


            // Easing
            // Calculate tangents based on distance from startPoint to endPoint, 60 being the max
            float tangent = Mathf.Clamp((-1) * (startPoint.x - endPoint.x), -100, 100);
            Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
            Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y);
            Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
            Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y);


            /*
            // Easing 2
            // Calculate tangents based on distance from startPoint to endPoint, 60 being the max
            float tangent = Mathf.Clamp((-1)*((startPoint.x - endPoint.x)), -100, 100);
            Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
            Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y);
            Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
            Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y);
            */

            /*
            // Easing with directional tangents
            // Calculate tangents based on distance from startPoint to endPoint, 100 being the max
            float tangent = Mathf.Clamp(Vector2.Distance(startPoint, endPoint), -100, 100);
            Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
            Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y - (tangent*0.5f));
            Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
            Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y - (tangent*0.5f));
            */

            bool isPro = EditorGUIUtility.isProSkin;

            if (isPro)
            {
                // Draw the shadow first
                Handles.DrawBezier(
                    shadowStartPoint,
                    shadowEndPoint,
                    shadowStartTangent,
                    shadowEndTangent,
                    new Color(0, 0, 0, 0.25f),
                    bezierTexture,
                    curveThickness
                );
            }


            Handles.DrawBezier(
                startPoint,
                endPoint,
                startTangent,
                endTangent,
                //new Color(0.8f,0.6f,0.3f,0.25f),
                (isPro) ? new Color(0.3f, 0.7f, 0.9f, 0.25f) : new Color(0f, 0.1f, 0.4f, 0.6f),
                bezierTexture,
                curveThickness
            );
        }
    }
}