using strange.extensions.context.api;
using strange.extensions.injector.api;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
   [Implements(typeof (IRoutineRunner), InjectionBindingScope.CROSS_CONTEXT)]
    public class RoutineRunner : IRoutineRunner
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        private RoutineRunnerBehavior mb;

        [PostConstruct]
        public void PostConstruct()
        {
#if UNITY_EDITOR
            Debug.Log("RoutineRunner is working.");
#endif
            mb = contextView.AddComponent<RoutineRunnerBehavior>();
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return mb.StartCoroutine(routine);
        }
    }

    public class RoutineRunnerBehavior : MonoBehaviour{ }
}