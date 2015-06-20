using Catharsis.InputEditor;
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

        [Inject]
        public IInputManager InputManager { get; set; }

        public override void Execute()
        {
            //Debug.Log("hi");
            //It's a monobehavior so this needs to be done!
            //https://groups.google.com/forum/#!topic/strangeioc/7h_-w90GtQw
            IStandaloneInputModule standaloneInputModule = contextView.AddComponent<StandaloneInputModule>();
            injectionBinder.injector.Inject(standaloneInputModule);
        }
    }
}