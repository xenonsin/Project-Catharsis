using strange.extensions.editor.impl;
using UnityEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor
{
    public class DialogueEditorView : EditorView
    {

        //Window Dimensions
        private const float _minWindowRectWidth = 800.0f;
        private const float _minWindowRectHeight = 400.0f;
         
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
        }

        void OnDisable()
        {
            base.OnDisable();
        }

        void OnGUI()
        {
            DisplayNodePanel();
        }

        void DisplayNodePanel()
        {
            
        }

        void DisplayHierarchyPanel()
        {
            
        }
    }
}