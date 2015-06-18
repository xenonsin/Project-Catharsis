
//This ContextView has two jobs:
//1. Provide the Cross-Context dependencies (see MainContext)
//2. Load the other Contexts (see MainStartupCommand)

using Catharsis.Main.Config;
using strange.extensions.context.impl;

namespace Catharsis.Main
{
    public class MainBootstrap : ContextView
    {
        void Start()
        {
            context = new MainContext(this);
        }
    }
}