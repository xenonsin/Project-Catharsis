using strange.extensions.command.impl;

namespace Catharsis.InputEditor
{
    public class SaveInputCommand : Command
    {

        [Inject]
        public IInputManager InputManager { get; set; }

        [Inject]
        public IPathUtility PathUtility { get; set; }


        public override void Execute()
        {
          string saveFolder = PathUtility.GetUserInputSaveFolder();
          if (!System.IO.Directory.Exists(saveFolder))
              System.IO.Directory.CreateDirectory(saveFolder);

          InputSaverXML saver = new InputSaverXML(saveFolder + "/input_config.xml");
			InputManager.Save(saver);
		
        }
    }
}