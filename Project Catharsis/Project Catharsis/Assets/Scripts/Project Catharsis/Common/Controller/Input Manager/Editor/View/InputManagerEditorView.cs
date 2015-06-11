using strange.extensions.editor.impl;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Catharsis.InputEditor.View
{
    public sealed class InputManagerEditorView : EditorView
    {
        #region Menu Options
        public enum FileMenuOptions
        {
            NewInputConfiguration = 0, NewAxisConfiguration, Export, Import,
        }
        #endregion


        #region Fields
        [SerializeField]
        private List<int> _selectionPath;
        [SerializeField]
        private Vector2 _hierarchyScrollPos = Vector2.zero;
        [SerializeField]
        private Vector2 _mainPanelScrollPos = Vector2.zero;
        [SerializeField]
        private float _hierarchyPanelWidth = _menuWidth * 2;
        [SerializeField]
        private Texture2D _highlightTexture;
        [SerializeField]
        private string _searchString = "";
        [SerializeField]
        private string _keyString = string.Empty;
        private float _toolbarHeight = 20.0f;
        private float _minCursorRectWidth = 10.0f;
        private float _maxCursorRectWidth = 50.0f;

        //Window Dimensions
        private const float _minWindowRectWidth = 400.0f;
        private const float _minWindowRectHeight = 200.0f;

        private const float _menuWidth = 100.0f;
        private const float _minHierarchyPanelWidth = 150.0f;
        #endregion


        [MenuItem("Catharsis/InputManager/Input Editor", false, 0)]
        public static void OpenWindow()
        {
            InputManagerEditorView window = EditorWindow.GetWindow<InputManagerEditorView>("Input Manager");
            window.minSize = new Vector2(_minWindowRectWidth, _minWindowRectHeight);
            //window.title
            window.Show();
        }

        void OnGUI()
        {
            DisplayHierarchyPanel();
            DisplayToolbar();
        }

        private void DisplayHierarchyPanel()
        {
            Rect screenRect = new Rect(0.0f, _toolbarHeight - 5.0f, _hierarchyPanelWidth, position.height - _toolbarHeight + 10.0f);
            Rect scrollView = new Rect(screenRect.x, screenRect.y, screenRect.width, position.height - screenRect.y);

            GUI.Box(screenRect, "");
            GUILayout.BeginArea(scrollView);
            _hierarchyScrollPos = EditorGUILayout.BeginScrollView(_hierarchyScrollPos);
            //GUILayout.Space(5.0f);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Space(5.0f);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DisplayToolbar()
        {
            //The rects constructed here determine the look of all the components hence forth.
            Rect screenRect = new Rect(0.0f,0.0f, position.width, _toolbarHeight); //This rect encompasses the toolbars
            Rect fileMenuRect = new Rect(0.0f, 0.0f, _menuWidth, screenRect.height); //This rect is the dimensions of the FileMenu drop down
            Rect editMenuRect = new Rect(fileMenuRect.xMax, 0.0f, _menuWidth, screenRect.height); //this rect is the dimensiosn of the EditMenu drop down
            Rect paddingLabelRect = new Rect(editMenuRect.xMax, 0.0f, screenRect.width - _menuWidth * 2, screenRect.height); //This rect is the dimensions of the padding after the drop down menus

            GUI.BeginGroup(screenRect);
            DisplayFileMenu(fileMenuRect);
            DisplayEditMenu(editMenuRect);
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
                CreateFileMenu(new Rect(rect.x, rect.yMax, 0.0f,0.0f));
            }
        }

        private void CreateFileMenu(Rect rect)
        {
            GenericMenu fileMenu = new GenericMenu();
            fileMenu.AddItem(new GUIContent("New Input Configuration") , false, HandleFileMenuOption, FileMenuOptions.NewInputConfiguration);
            fileMenu.AddItem(new GUIContent("New Axis Configuration"), false, HandleFileMenuOption, FileMenuOptions.NewAxisConfiguration);

            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("Import"), false, HandleFileMenuOption, FileMenuOptions.Import);
            fileMenu.AddItem(new GUIContent("Export"), false, HandleFileMenuOption, FileMenuOptions.Export);

            fileMenu.DropDown(rect);
        }

        private void HandleFileMenuOption(object arg)
        {
            FileMenuOptions option = (FileMenuOptions) arg;
            switch (option)
            {
                    case FileMenuOptions.NewInputConfiguration:
                        CreateNewInputConfiguration();
                        break;
                    case FileMenuOptions.NewAxisConfiguration:
                        CreateNewAxisConfiguration();
                        break;
                    case FileMenuOptions.Import:
                        ImportInputConfigurations();
                        break;
                    case FileMenuOptions.Export:
                        ExportInputConfigurations();
                        break;
            }
        }

        public void CreateNewInputConfiguration()
        {
            
        }

        public void CreateNewAxisConfiguration()
        {
            
        }

        public void ExportInputConfigurations()
        {
            
        }

        public void ImportInputConfigurations()
        {
            
        }

        private void DisplayEditMenu(Rect rect)
        {
            EditorGUI.LabelField(rect, "Edit", EditorStyles.toolbarDropDown);
        }


    }
}