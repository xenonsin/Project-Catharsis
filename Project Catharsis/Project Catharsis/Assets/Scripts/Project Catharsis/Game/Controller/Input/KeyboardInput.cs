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
                if (inputManager.GetButtonDown("Left"))
                {
                    input |= GameInputEvent.MOVE_LEFT;
                }
                if (inputManager.GetButtonDown("Forward"))
                {
                    input |= GameInputEvent.MOVE_FORWARD;
                }
                if (inputManager.GetButtonDown("Right"))
                {
                    input |= GameInputEvent.MOVE_RIGHT;
                }
                if (inputManager.GetButtonDown("Back"))
                {
                    input |= GameInputEvent.MOVE_BACKWARD;
                }
                gameInputSignal.Dispatch(input);
                yield return null;
            }
        }
    }

}