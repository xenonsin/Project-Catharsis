using strange.extensions.command.impl;
using UnityEngine;

namespace Catharsis.UI.Control
{
    public class PauseGameCommand : Command
    {
        public override void Execute()
        {
            //Time.timeScale = 0.0f;

            base.Execute();
        }
    }
}