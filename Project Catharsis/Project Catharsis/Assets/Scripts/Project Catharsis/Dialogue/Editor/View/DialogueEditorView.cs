using System.Collections.Generic;
using Catharsis.DialogueEditor.Model;
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
            SaveScenario = 0, LoadScenario, NewDialogue, AddMessageNode, AddBranchingMessageNode, AddConditionalNode, AddSetVariableNode, AddGenericEventNode, AddEndNode,
        }

        public enum EditMenuOptions
        {
            Duplicate, Delete, DeleteAll, Copy, Paste,
        }

        #endregion

        //probably put this in the mediator?
        private DialogueEditorData _dialogueData;

        [SerializeField]
        private Vector2 _hierarchyScrollPos = Vector2.zero;
        [SerializeField]
        private float _hierarchyPanelWidth = _menuWidth * 2;

        //[0] - Current Dialogue
        //[1] - Current Node
        [SerializeField] 
        private int currentDialogue;
        [SerializeField]
        private int currentNode;
        [SerializeField]
        private Texture2D _highlightTexture;

        private bool loaded = false;

        //Window Dimensions
        private float _toolbarHeight = 18.0f;
        private float _hierarchyItemHeight = 20.0f;
        private const float _minWindowRectWidth = 800.0f;
        private const float _minWindowRectHeight = 400.0f;
        private const float _menuWidth = 100.0f;
        private const float _minHierarchyPanelWidth = 150.0f;

         
        [MenuItem("Catharsis/Dialogue Manager/Dialogue Editor", false, 0)]
        public static void OpenWindow()
        {
            DialogueEditorView window = EditorWindow.GetWindow<DialogueEditorView>("Dialogue Editor");
            window.minSize = new Vector2(_minWindowRectWidth, _minWindowRectHeight);
            //window.title
            window.Show();
        }

        void OnEnable()
        {
            base.OnEnable();

            if (currentDialogue == null)
                currentDialogue = 0;
            if (currentNode == null)
                currentNode = 0;

            if (_dialogueData == null)
                _dialogueData = new DialogueEditorData();

            //Debug
            if (_dialogueData.DialogueCount < 5)
                for (int i = 0; i < 5 ; i ++)
                    _dialogueData.AddDialogue(1);
            
            if (_highlightTexture == null)
                CreateHighlightTexture();

            loaded = true;
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
                DisplayNodePanel();
                DisplayHierarchyPanel();
                
            }

            DisplayToolBar();
        }

        private void CreateHighlightTexture()
        {
            _highlightTexture = new Texture2D(1, 1);
            _highlightTexture.SetPixel(0, 0, new Color32(50, 125, 255, 255));
            _highlightTexture.Apply();
        }

        void DisplayNodePanel()
        {
            
        }

        #region HierarchyPanel
        void DisplayHierarchyPanel()
        {
            Rect screenRect = new Rect(0.0f, _toolbarHeight - 5.0f, _hierarchyPanelWidth, position.height - _toolbarHeight + 10.0f);
            Rect scrollView = new Rect(screenRect.x, screenRect.y, screenRect.width, position.height - screenRect.y);

            GUI.Box(screenRect, "");
            GUILayout.BeginArea(scrollView);


            _hierarchyScrollPos = EditorGUILayout.BeginScrollView(_hierarchyScrollPos);
            GUILayout.Space(5.0f);
            for (int i = 0; i < _dialogueData.DialogueCount; i++)
            {
                DisplayHierarchyDialogueItem(screenRect, i, _dialogueData.dialogues[i].name);

            }
            
            GUILayout.Space(5.0f);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void DisplayHierarchyDialogueItem(Rect rect, int index, string name)
        {
            Vector2 mouseClickPosition = Vector2.zero;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                mouseClickPosition = new Vector2(Event.current.mousePosition.x - rect.x - 3, Event.current.mousePosition.y - rect.y - 3 + _hierarchyItemHeight);
            }

            Rect configPos = GUILayoutUtility.GetRect(new GUIContent(name), EditorStyles.label, GUILayout.Height(_hierarchyItemHeight));

            if (mouseClickPosition != Vector2.zero && configPos.Contains(mouseClickPosition))
            {
                currentDialogue = index;
            }

            //Display Highlight texture
            //if (configPos.Contains(Event.current.mousePosition))
            //{
            //    GUI.color = Color.grey;
            //    GUI.Box(configPos, string.Empty);
            //}

            //GUI.color = GUI.contentColor;

            //Debug.Log(_selectionIndex.Count);

            if (currentDialogue == index) //_selectionIndex[0] == index
            {
                if (_highlightTexture == null)
                {
                    CreateHighlightTexture();
                }
                GUI.DrawTexture(configPos, _highlightTexture, ScaleMode.StretchToFill);

            }
            

            Rect labelNumberRow = new Rect(configPos.x + 2, configPos.y + 2, configPos.width - 4, configPos.height - 4);
            Rect labelNameRow = new Rect(labelNumberRow.x + 25, labelNumberRow.y, labelNumberRow.width - 25, labelNumberRow.height);

            GUI.Label(labelNumberRow, _dialogueData.dialogues[index].id.ToString());
            GUI.Label(labelNameRow, (_dialogueData.dialogues[index].name == string.Empty) ? "-" : _dialogueData.dialogues[index].name);

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
            fileMenu.AddItem(new GUIContent("Save Scenario"), false, HandleFileMenuOption, FileMenuOptions.SaveScenario);
            fileMenu.AddItem(new GUIContent("Load Scenario"), false, HandleFileMenuOption, FileMenuOptions.LoadScenario);

            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("New Dialogue"), false, HandleFileMenuOption, FileMenuOptions.NewDialogue);
            fileMenu.AddSeparator("");

            if (currentNode >= 1)
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
                case FileMenuOptions.SaveScenario:

                    break;
                case FileMenuOptions.LoadScenario:

                    break;
                case FileMenuOptions.NewDialogue:

                    break;
                case FileMenuOptions.AddMessageNode:
 
                    break;
                case FileMenuOptions.AddBranchingMessageNode:

                    break;
                case FileMenuOptions.AddConditionalNode:

                    break;
                case FileMenuOptions.AddSetVariableNode:

                    break;
                case FileMenuOptions.AddGenericEventNode:

                    break;
                case FileMenuOptions.AddEndNode:

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

            if (currentNode >= 1)
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
                   // Delete();
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

        private void Save()
        {
            
        }
        #endregion
    }
}