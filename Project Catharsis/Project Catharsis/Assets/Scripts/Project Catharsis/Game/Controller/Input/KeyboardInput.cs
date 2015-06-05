using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
    public class KeyboardInput : IInput
    {
        [Inject(ContextKeys.CONTEXT_DISPATCHER)]
        public IEventDispatcher dispatcher { get; set; }

        [Inject]
        public IRoutineRunner routineRunner { get; set; }

        [Inject]
        public GameInputSignal gameInputSignal { get; set; }

        [Inject]
        public IInputManager inputManager { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            //Debug.Log("hi input");
            routineRunner.StartCoroutine(Update());
        }

        protected IEnumerator Update()
        {
            while (true)
            {
                int input = GameInputEvent.NONE;
                if (inputManager.GetKeyDown(KeyCode.Space))
                {
                    input |= GameInputEvent.NONE;
                }
                if (inputManager.GetKeyDown(KeyCode.A))
                {
                    input |= GameInputEvent.MOVE_LEFT;
                }
                if (inputManager.GetKeyDown(KeyCode.W))
                {
                    input |= GameInputEvent.MOVE_FORWARD;
                }
                if (inputManager.GetKeyDown(KeyCode.D))
                {
                    input |= GameInputEvent.MOVE_RIGHT;
                }
                if (inputManager.GetKeyDown(KeyCode.S))
                {
                    input |= GameInputEvent.MOVE_BACKWARD;
                }
                gameInputSignal.Dispatch(input);
                yield return null;
            }
        }
    }

}