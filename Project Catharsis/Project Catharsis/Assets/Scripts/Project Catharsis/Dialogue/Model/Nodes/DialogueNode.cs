using System.Collections.Generic;
using Catharsis.DialogueEditor.Config;
using Catharsis.DialogueEditor.Model.Data;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model.Nodes
{
    public abstract class DialogueNode
    {
        [Inject]
        public DialogueNodeCompleteSignal NodeCompleteSignal { get; set; }

        public readonly int?[] outs;
        protected int nextPhaseId;
        protected DialogueVariables _localVariables;

        public DialogueNode(List<int?> outs)
        {
			if(outs != null){
				int?[] outsClone = outs.ToArray();
				this.outs = outsClone.Clone() as int?[];
			}
		}

        private NodeState _state;
        public NodeState state
        {
            get
            {
                return _state;
            }
            protected set
            {
                _state = value;
                switch (_state)
                {
                    case NodeState.Inactive:
                        // Do Nothing
                        break;

                    case NodeState.Start:
                        OnStart();
                        break;

                    case NodeState.Action:
                        OnAction();
                        break;

                    case NodeState.Complete:
                        OnComplete();
                        break;
                }
            }
        }

        public void Start(DialogueVariables localVars)
        {
            Reset();
            _localVariables = localVars;
            state = NodeState.Start;
        }

        virtual protected void Reset()
        {
            nextPhaseId = (outs != null && outs[0].HasValue) ? outs[0].Value : 0;
            _localVariables = null;
        }

        virtual public void Continue(int outId)
        {
            int nextId = 0;

            if (outs != null && outs[outId].HasValue)
            {
                nextId = outs[outId].Value;
            }
            else
            {
                Debug.LogWarning("Invalid Out Id");
            }

            nextPhaseId = nextId;
        }

        virtual protected void OnStart()
        {
            state = NodeState.Action;
        }

        virtual protected void OnAction()
        {
            state = NodeState.Complete;
        }

        virtual protected void OnComplete()
        {
            NodeCompleteSignal.Dispatch(nextPhaseId);
            state = NodeState.Inactive;
            Reset();
        }
    }
}