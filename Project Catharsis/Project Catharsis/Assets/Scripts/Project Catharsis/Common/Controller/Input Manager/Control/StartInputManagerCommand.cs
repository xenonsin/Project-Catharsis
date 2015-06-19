using strange.extensions.command.impl;

namespace Catharsis.InputEditor
{
    public class StartInputManagerCommand : Command
    {

        [Inject]
        public IInputManager InputManager { get; set; }

        public override void Execute()
        {
            InputManager.StartAfterConfigLoaded();
        }
    }
}