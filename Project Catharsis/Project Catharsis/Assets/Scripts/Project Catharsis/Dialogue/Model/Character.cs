
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;


namespace Catharsis.DialogueEditor.Model
{
    //Currently having warning that this must be instantated using the ScriptioableObject.CreateInstance instead of new Character
    //The new Character is being called by the serializer.. it creates a new Character every time it loads.. but it cant.
    
    //TODO: Add editor extension so that you can add characters
    //TODO: Allow the Dialogue Editor to choose the animation when using the gameobject
    [System.Serializable]
    public class Character : ScriptableObject
    {
        public string characterName;
        private Sprite portrait2D;

        public Sprite Portrait2D
        {
            get { return portrait2D;}
            set
            {
                portrait2D = value;

            }
        }

        private GameObject portrait3D;

        public GameObject Portrait3D
        {
            get { return portrait3D; }
            set
            {
                portrait3D = value;
                GetAnimNames();
            }
        }

        public Color nameColor;
        public int[] animIndex;
        public string[] animationNames;

        public Character()
        {
            characterName = "< Name >";
            portrait2D = Resources.Load<Sprite>("defaultPortrait");
            portrait3D = Resources.Load("defaultGameObject") as GameObject;
            nameColor = Color.yellow;

            
        }

        public Character(string name, Sprite portrait2D, GameObject portrait3D)
        {
           this.characterName = name;
           Portrait2D = portrait2D;
           Portrait3D = portrait3D;
        }

        public void GetAnimNames()
        {

            List<string> animNames = new List<string>();
            List<int> animId = new List<int>();
            if (portrait3D)
            {
                Animator anim = portrait3D.gameObject.GetComponent<Animator>();

                int i = 0;
                foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
                {
                    animNames.Add(clip.name);
                    animId.Add(i);
                    i++;
                }
            }

            animIndex = animId.ToArray();
            animationNames = animNames.ToArray();
        }

       

        //TODO: Add a list of animations and be able to play them.?
    }
}