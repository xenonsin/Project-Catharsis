using strange.extensions.signal.impl;

namespace Catharsis.GlobalVariablesManager.Config
{
    public class GlobalVariableManagerLoaded : Signal {}

    //Todo: make command
    public class GlobalVariableManagerLoad : Signal<string> {}

    public class GlobalVariableManagerLoadDefault : Signal { }
}