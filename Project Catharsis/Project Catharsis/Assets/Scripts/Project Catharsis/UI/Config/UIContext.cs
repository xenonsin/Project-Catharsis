using UnityEngine;

namespace Catharsis.UI
{
    public class UIContext : SignalContext
    {
        public UIContext(MonoBehaviour contextView) : base(contextView)
        {
        }

        protected override void mapBindings()
        {
            base.mapBindings();
        }

        protected override void postBindings()
        {
            base.postBindings();
        }
    }

}