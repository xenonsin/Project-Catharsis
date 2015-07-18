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

        private DialogueEditorData _dialogueData;

        [SerializeField]
        private Vector2 _hierarchyScrollPos = Vector2.zero;
        [SerializeField]
        private float _hierarchyPanelWidth = _menuWidth * 2;
        [SerializeField] 
        private List<int> _selectionIndex; 

        //Window Dimensions
        private float _toolbarHeight = 18.0f;
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

            if (_selectionIndex == null)
                _selectionIndex = new List<int>();

            if (_dialogueData == null)
                _dialogueData = new DialogueEditorData();
        }

        void OnDisable()
        {
            base.OnDisable();
        }

        void OnGUI()
        {
            DisplayNodePanel();
            DisplayHierarchyPanel();
            DisplayToolBar();
        }

        void DisplayNodePanel()
        {
            
        }

        void DisplayHierarchyPanel()
        {
            Rect screenRect = new Rect(0.0f, _toolbarHeight - 5.0f, _hierarchyPanelWidth, position.height - _toolbarHeight + 10.0f);
            Rect scrollView = new Rect(screenRect.x, screenRect.y, screenRect.width, position.height - screenRect.y);

            GUI.Box(screenRect, "");
            GUILayout.BeginArea(scrollView);
            _hierarchyScrollPos = EditorGUILayout.BeginScrollView(_hierarchyScrollPos);
            GUILayout.Space(5.0f);

            //for (int i = 0; i < _inputManager.GetInputConfigurationCount(); i++)
            //{
            //    DisplayHierarchyInputConfigItem(screenRect, i, _inputManager.GetInputConfiguration(i).name);
            //    if (_inputManager.GetInputConfiguration(i).isExpanded)
            //    {
            //        for (int j = 0; j < _inputManager.GetInputConfiguration(i).axes.Count; j++)
            //        {
            //            DisplayHierarchiAxisConfigItem(screenRect, i, j, _inputManager.GetInputConfiguration(i).axes[j].name);
            //        }
            //    }
            //}

            GUILayout.Space(5.0f);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

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

            if (_selectionIndex.Count >= 1)
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
            if (_selectionIndex.Count > 0)
                editMenu.AddItem(new GUIContent("Duplicate          Shift+D"), false, HandleEditMenuOption, EditMenuOptions.Duplicate);
            else
                editMenu.AddDisabledItem(new GUIContent("Duplicate          Shift+D"));

            if (_selectionIndex.Count > 0)
                editMenu.AddItem(new GUIContent("Delete                Del"), false, HandleEditMenuOption, EditMenuOptions.Delete);
            else
                editMenu.AddDisabledItem(new GUIContent("Delete                Del"));

            //if (_inputManager.GetInputConfigurationCount() > 0)
            //    editMenu.AddItem(new GUIContent("Delete All"), false, HandleEditMenuOption, EditMenuOptions.DeleteAll);
            //else
            //    editMenu.AddDisabledItem(new GUIContent("Delete All"));

            if (_selectionIndex.Count >= 2)
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