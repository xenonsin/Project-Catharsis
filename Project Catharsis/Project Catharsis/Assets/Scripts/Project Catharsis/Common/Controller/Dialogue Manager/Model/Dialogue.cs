using System.Collections.Generic;

namespace Catharsis.DialogueEditor.Model
{
    public class Dialogue
    {
        /// <summary>
        /// Name of the Dialogue
        /// </summary>
        public string name;

        /// <summary>
        /// Dialogue ID
        /// </summary>
        public int id;

        /// <summary>
        /// List of all messages the dialogue contains.
        /// </summary>
        public List<DialogueMessage> messages;

        public Dialogue(string name, int ID, List<DialogueMessage> message)
        {
            this.name = name;
            this.id = ID;
            this.messages = message;
        }

    }
}