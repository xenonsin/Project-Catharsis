using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using System.Collections;
using Catharsis.InputEditor;

namespace Catharsis
{
    public class KeyboardInput : IInput
    {
        [Inject]
        public IRoutineRunner routineRunner { get; set; }

        [Inject]
        public GameInputSignal gameInputSignal { get; set; }

        [Inject]
        public IInputManager InputManager { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            //For Now Run automatically. Would create a command that runs this after gamestart.
            routineRunner.StartCoroutine(Update());
        }

        //Is called after the InputManager is loaded.
        private void StartKeyboardInput()
        {
            routineRunner.StartCoroutine(Update());
            
        }

        protected IEnumerator Update()
        {
            //HOW TO START  ?
            //NO WAY TO END ?????

            while (true)
            {
                //Debug.Log(InputManager.mousePosition);
                int input = GameInputEvent.NONE;
                //if (InputManager.GetButtonDown("Forward"))
                //{
                //    input |= GameInputEvent.MOVE_FORWARD;
                //    Debug.Log("W");
                //}
                //if (InputManager.GetButtonDown("Left"))
                //{
                //    input |= GameInputEvent.MOVE_LEFT;
                //    Debug.Log("A");

                //}
                //if (InputManager.GetButtonDown("Back"))
                //{
                //    input |= GameInputEvent.MOVE_BACKWARD;
                //    Debug.Log("S");

                //}
                //if (InputManager.GetButtonDown("Right"))
                //{
                //    input |= GameInputEvent.MOVE_RIGHT;
                //    Debug.Log("D");

                //}
             
                gameInputSignal.Dispatch(input);
                yield return null;
            }
        }
    }

}