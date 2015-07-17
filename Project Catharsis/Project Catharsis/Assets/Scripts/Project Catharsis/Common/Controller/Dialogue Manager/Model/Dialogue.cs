using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Nodes;

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
        public List<DialogueNode> nodes;

        public Dialogue(string name, int id, List<DialogueNode> nodes)
        {
            this.name = name;
            this.id = id;
            this.nodes = nodes;
        }

    }
}