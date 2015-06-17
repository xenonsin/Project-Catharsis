using strange.extensions.command.impl;

namespace Catharsis.UI.Control
{
    public class QuitGameCommand : Command
    {
        public override void Execute()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }
    }
}