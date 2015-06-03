using UnityEngine;
using System.Collections;
using strange.extensions.context.api;
using strange.extensions.injector.api;
using TeamUtility.IO;

namespace Catharsis
{
    [Implements(typeof(IInputManager), InjectionBindingScope.CROSS_CONTEXT)]
    public class InputManagerRunner : IInputManager
    {
         [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        private InputManager mb;

        [PostConstruct]
        public void PostConstruct()
        {
            mb = contextView.AddComponent<InputManager>();
        }

    }
}