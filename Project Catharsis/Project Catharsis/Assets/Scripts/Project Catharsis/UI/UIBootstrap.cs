using strange.extensions.context.impl;

namespace Catharsis.UI
{
    public class UIBootstrap : ContextView
    {
        void Start()
        {
            context = new UIContext(this);
        }
    }
}