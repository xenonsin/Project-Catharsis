using System.Collections.Generic;
using Catharsis.DialogueEditor.Model.Nodes;

namespace Catharsis.DialogueEditor.Model
{
    public class Dialogue
    {
        /// <summary>
        /// Name of the Dialogue
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Dialogue ID
        /// </summary>
        public readonly int startNodeid;

        /// <summary>
        /// List of all messages the dialogue contains.
        /// </summary>
        public readonly List<DialogueNode> nodes;

        private readonly DialogueVariables _originalLocalVariables;
        public DialogueVariables LocalVariables;

        public Dialogue(string name, int startNodeid, DialogueVariables localVariables, List<DialogueNode> nodes)
        {
            this.name = name;
            this.startNodeid = startNodeid;
            this._originalLocalVariables = localVariables;
            this.nodes = nodes;
        }

        public void Reset()
        {
            LocalVariables = _originalLocalVariables.Clone();
        }

    }
}