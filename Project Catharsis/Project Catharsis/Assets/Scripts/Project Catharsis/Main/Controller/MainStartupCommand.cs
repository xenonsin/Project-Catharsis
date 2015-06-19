using System.Security.Policy;
using Catharsis.InputEditor;
using Catharsis.InputEditor.UI;
using strange.extensions.command.impl;
using UnityEngine;

namespace Catharsis.Main.Controller
{
    public class MainStartupCommand : Command
    {
        [Inject]
        public IRoutineRunner RoutineRunner { get; set; }
       [Inject]
       public IInputManager InputManager { get; set; }


        public override void Execute()
        {
            Application.LoadLevelAdditive("Game");
            Application.LoadLevelAdditive("UI");

        }
    }
}