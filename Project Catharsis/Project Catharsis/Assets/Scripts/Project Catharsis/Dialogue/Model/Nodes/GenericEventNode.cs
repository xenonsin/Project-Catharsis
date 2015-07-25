using System.Collections.Generic;
using System.Reflection.Emit;
using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Model.Data;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public class GenericEventNode : DialogueNode
    {
        [Inject]
        public DialogueGenericEventSignal EventSignal { get; set; }

        public readonly string message;
		public readonly string metadata;

        public GenericEventNode(string message, string metadata, List<int?> outs)
            : base(outs)
        {
			this.message = message;
			this.metadata = metadata;
		}
		
		protected override void OnStart(){
            EventSignal.Dispatch(message, metadata);
			state = NodeState.Complete;
		}
    }
}