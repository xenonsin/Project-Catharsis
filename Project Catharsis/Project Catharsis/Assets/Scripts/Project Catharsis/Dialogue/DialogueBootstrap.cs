/*
 * This is an adaptation of the Dialoguer Asset from http://www.dialoguer.info/
 */

using Catharsis.DialogueEditor.Config;
using strange.extensions.context.impl;

namespace Assets.Scripts.Project_Catharsis.Dialogue
{
    public class DialogueBootstrap : ContextView
    {
        void Start()
        {
            context = new DialogueContext(this);
        }
    }
}