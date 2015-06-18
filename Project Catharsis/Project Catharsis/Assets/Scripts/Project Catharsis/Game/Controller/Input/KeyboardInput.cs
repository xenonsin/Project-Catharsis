using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using System.Collections;
using Catharsis.InputEditor;

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
                if (inputManager.GetKeyDown(KeyCode.W))//inputManager.GetButtonDown("Forward"))
                {
                    input |= GameInputEvent.MOVE_FORWARD;
                    Debug.Log("W");
                }
                if (inputManager.GetKeyDown(KeyCode.A))//inputManager.GetButtonDown("Left"))
                {
                    input |= GameInputEvent.MOVE_LEFT;
                    Debug.Log("A");

                }
                if (inputManager.GetKeyDown(KeyCode.S))//inputManager.GetButtonDown("Back"))
                {
                    input |= GameInputEvent.MOVE_BACKWARD;
                    Debug.Log("S");

                }
                if (inputManager.GetKeyDown(KeyCode.D))//inputManager.GetButtonDown("Right"))
                {
                    input |= GameInputEvent.MOVE_RIGHT;
                    Debug.Log("D");

                }
             
                gameInputSignal.Dispatch(input);
                yield return null;
            }
        }
    }

}