using strange.extensions.editor.impl;
using UnityEngine;
using UnityEditor;

namespace Catharsis.InputEditor.View
{
    public class InputManagerEditorView : EditorView 
    {
        [MenuItem("Catharsis/InputManager/Input Editor", false, 0)]
        public static void OpenWindow()
        {
            InputManagerEditorView window = EditorWindow.GetWindow<InputManagerEditorView>("Input Manager");
            window.minSize = new Vector2(400, 300);
            //window.title
            window.Show();
        }
    }
}