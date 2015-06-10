using System.Security.AccessControl;
using UnityEditor.Callbacks;

namespace Catharsis.InputEditor.Service
{
    public class ScriptReloadService
    {
        [Inject]
        public ScriptReloadSignal signal { get; set; }

        private static ScriptReloadService instance;

        public ScriptReloadService()
        {
            if (instance == null)
                instance = this;
        }

        private void dispatch()
        {
            signal.Dispatch();
        }

        [DidReloadScripts]
        static void DidReloadScripts()
        {
            instance.dispatch();
        }
    }
}