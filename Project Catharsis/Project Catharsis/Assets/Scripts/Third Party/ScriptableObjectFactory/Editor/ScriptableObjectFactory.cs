/*
 * From https://github.com/liortal53/ScriptableObjectFactory 
 */

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectFactory
{

    /// <summary>
    /// Helper class for instantiating ScriptableObjects.
    /// </summary>
    public class ScriptableObjectFactory
    {
        [MenuItem("Project/Create/ScriptableObject")]
        [MenuItem("Assets/Create/ScriptableObject")]
        public static void Create()
        {
            var assembly = GetAssembly();

            // Get all classes derived from ScriptableObject
            var allScriptableObjects = (from t in assembly.GetTypes()
                                        where t.IsSubclassOf(typeof(ScriptableObject))
                                        select t).ToList();

            var editorAssembly = GetEditorAssembly();
            var moreScriptableObjects = (from t in editorAssembly.GetTypes()
                                         where t.IsSubclassOf(typeof(ScriptableObject))
                                         select t).ToList();

            allScriptableObjects.AddRange(moreScriptableObjects);

            // Show the selection window.
            var window = EditorWindow.GetWindow<ScriptableObjectWindow>(true, "Create a new ScriptableObject", true);
            window.ShowPopup();

            window.Types = allScriptableObjects;
        }

        /// <summary>
        /// Returns the assembly that contains the script code for this project (currently hard coded)
        /// </summary>
        private static Assembly GetAssembly()
        {
            return Assembly.Load(new AssemblyName("Assembly-CSharp"));
        }

        private static Assembly GetEditorAssembly()
        {
            return Assembly.Load(new AssemblyName("Assembly-CSharp-Editor"));
        }
    }
}