using strange.extensions.signal.impl;

namespace Catharsis.InputEditor
{
    public class InputManagerConfigurationChangedSignal : Signal<string> { }
    public class InputManagerConfigurationDirtySignal : Signal<string> { }
    public class InputManagerLoadedSignal : Signal { }
    public class InputManagerSavedSignal : Signal { }
    public class InputManagerRemoteUpdateSignal : Signal { }

    public class InputManagerLoadUserInputSignal : Signal{ }
    public class InputManagerLoadDefaultInputSignal : Signal { }
    public class InputManagerSaveSignal : Signal{ }


}