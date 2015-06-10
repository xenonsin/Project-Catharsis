using strange.extensions.editor.impl;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Catharsis.InputEditor.View
{
    public class InputManagerEditorView : EditorView
    {
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

        private const float _menuWidth = 100.0f;
        private const float _minHierarchyPanelWidth = 150.0f;
        #endregion


        [MenuItem("Catharsis/InputManager/Input Editor", false, 0)]
        public static void OpenWindow()
        {
            InputManagerEditorView window = EditorWindow.GetWindow<InputManagerEditorView>("Input Manager");
            window.minSize = new Vector2(400, 300);
            //window.title
            window.Show();
        }

        void OnGUI()
        {
            DisplayHierarchyPanel();
        }

        private void DisplayHierarchyPanel()
        {
            Rect screenRect = new Rect(0.0f, _toolbarHeight - 5.0f, _hierarchyPanelWidth, position.height - _toolbarHeight + 10.0f);
            Rect scrollView = new Rect(screenRect.x, screenRect.y + 5.0f, screenRect.width, position.height - screenRect.y);

            GUI.Box(screenRect, "");
            //GUILayout.BeginArea(scrollView);
            //_hierarchyScrollPos = EditorGUILayout.BeginScrollView(_hierarchyScrollPos);

            //EditorGUILayout.EndScrollView();
            //GUILayout.EndArea();
        }


    }
}