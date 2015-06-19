using Catharsis.InputEditor.UI;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace Catharsis.UI.Control
{
    public class UIStartCommand : Command
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        public override void Execute()
        {
            IStandaloneInputModule standaloneInputModule = contextView.AddComponent<StandaloneInputModule>();
            injectionBinder.injector.Inject(standaloneInputModule);
        }
    }
}