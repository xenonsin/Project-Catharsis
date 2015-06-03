using strange.extensions.command.impl;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
    public class GameStartCommand : Command
    {
        //This injection instantates the game input, this instantiates the routine behavior!!!
        [Inject] 
        public IInput input { get; set; }
        public override void Execute()
        {
           Debug.Log("Game Start Command has been executed.");
        }
    }

}