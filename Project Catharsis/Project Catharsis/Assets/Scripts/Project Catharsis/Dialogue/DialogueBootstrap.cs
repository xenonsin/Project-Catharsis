/*
 * This is an adaptation of the Dialoguer Asset from http://www.dialoguer.info/
 */

using Catharsis.DialogueEditor.Config;
using strange.extensions.context.impl;

namespace Catharsis.DialogueEditor
{
    public class DialogueBootstrap : ContextView
    {
        void Start()
        {
            context = new DialogueContext(this);
        }
    }
}