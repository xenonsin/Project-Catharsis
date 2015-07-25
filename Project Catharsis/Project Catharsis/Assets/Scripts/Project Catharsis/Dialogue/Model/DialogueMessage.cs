using System.Collections.Generic;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model
{
    public class DialogueMessage
    {
        /// <summary>
        /// The message's text
        /// </summary>
        public string text;

        /// <summary>
        /// the message speaker;
        /// </summary>
        public Character speaker;

        /// <summary>
        /// If the message has audio
        /// </summary>
        public AudioClip audioClip;

        /// <summary>
        /// The message's branching choices
        /// </summary>
        public string[] choices;

        public DialogueMessage(string txt, Character speaker, AudioClip audio, List<string> choice)
        {
            this.text = txt;
            this.speaker = speaker;
            this.audioClip = audio;

            if (choice != null)
            {
                string[] temp = choice.ToArray();
                this.choices = temp.Clone() as string[];
            }
        }
        //Probably make a consutrctor that uses resource load.
    }
}