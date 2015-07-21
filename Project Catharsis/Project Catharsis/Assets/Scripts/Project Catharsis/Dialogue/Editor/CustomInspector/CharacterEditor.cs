using Catharsis.DialogueEditor.Model;
using UnityEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor.CustomInspector
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            //For Debug Purposes
            //
            Character myCharacter = (Character) target;
            //Debug.Log(myCharacter.Portrait2D);
            //DrawDefaultInspector();
            
            myCharacter.characterName = EditorGUILayout.TextField("Name", myCharacter.characterName);
            myCharacter.Portrait2D = EditorGUILayout.ObjectField("Portrait 2D", myCharacter.Portrait2D, typeof(Sprite), true) as Sprite;
            myCharacter.Portrait3D = EditorGUILayout.ObjectField("Portrait 3D", myCharacter.Portrait3D, typeof(GameObject), true) as GameObject;

            if (GUI.changed)
            {

                
                EditorUtility.SetDirty(myCharacter);
                AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

            }

        }
    }
}