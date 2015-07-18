using strange.extensions.command.impl;
using UnityEngine;

namespace Catharsis.DialogueEditor
{
    public class DialogueManagerStartCommand : Command
    {
        public override void Execute()
        {
            Debug.Log("Dialogue Manager started.");
        }
    }
}