using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;


namespace Catharsis.InputEditor.Controller
{
    public static class InputEditorTools
    {
        private static string CUSTOM_PROJECT_SETTINGS = "input_manager_project_settings";

        public static void OverwriteInputSettings()
		{
			TextAsset textAsset = Resources.Load(CUSTOM_PROJECT_SETTINGS) as TextAsset;
			if(textAsset == null)
			{
				EditorUtility.DisplayDialog("Error", "Unable to load input settings from the Resources folder.", "OK");
				return;
			}
			
			int length = Application.dataPath.LastIndexOf('/');
			string projectSettingsFolder = string.Concat(Application.dataPath.Substring(0, length), "/ProjectSettings");
			if(!Directory.Exists(projectSettingsFolder))
			{
				Resources.UnloadAsset(textAsset);
				EditorUtility.DisplayDialog("Error", "Unable to get the correct path to the ProjectSetting folder.", "OK");
				return;
			}
			
			string inputManagerPath = string.Concat(projectSettingsFolder, "/InputManager.asset");
			File.Delete(inputManagerPath);
			using(StreamWriter writer = File.CreateText(inputManagerPath))
			{
				writer.Write(textAsset.text);
			}
			EditorUtility.DisplayDialog("Success", "The input settings have been successfully replaced.\nYou might need to minimize and restore Unity to reimport the new settings.", "OK");

            Resources.UnloadAsset(textAsset);
        }
        
        public static void KeyCodeField(ref string keyString, ref bool isEditing, string label, string controlName, KeyCode currentKey)
        {
            GUI.SetNextControlName(controlName);
            bool hasFocus = (GUI.GetNameOfFocusedControl() == controlName);
            if (!isEditing && hasFocus)
            {
                keyString = currentKey == KeyCode.None ? string.Empty : currentKey.ToString();
            }

            isEditing = hasFocus;
            if (isEditing)
            {
                keyString = EditorGUILayout.TextField(label, keyString);
            }
            else
            {
                EditorGUILayout.TextField(label, currentKey == KeyCode.None ? string.Empty : currentKey.ToString());
            }
        }
    }
}