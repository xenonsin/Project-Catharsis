using Catharsis.InputEditor.Service;
using strange.extensions.command.impl;
using UnityEngine;

namespace Catharsis.InputEditor.Controller
{
    public class InitInputManagerCommand : Command
    {
        [Inject] 
        public ScriptReloadService scriptReloadService { get; set; } //Use this for instantiation
        public override void Execute()
        {
            //Debug.Log("Input Manager Editor Loaded");
        }
    }
}