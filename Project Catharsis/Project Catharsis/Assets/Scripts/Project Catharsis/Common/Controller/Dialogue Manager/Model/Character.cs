using UnityEngine;


namespace Catharsis.DialogueEditor.Model
{
    //TODO: Add editor extension so that you can add characters
    //TODO: Allow the Dialogue Editor to choose the animation when using the gameobject
    public class Character
    {
        public string name;

        /// <summary>
        /// The Character Icon, or main portrait if only using one image.
        /// </summary>
        public Texture2D portrait2D;

        public GameObject portrait3D;

        public Color nameColor;

        public Character(string name, Texture2D portrait2D, GameObject portrait3D)
        {
            this.name = name;
            this.portrait2D = portrait2D;
            this.portrait3D = portrait3D;
        }

        //TODO: Add a list of animations and be able to play them.?
    }
}