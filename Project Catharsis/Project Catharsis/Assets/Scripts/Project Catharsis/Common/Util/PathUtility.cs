using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using UnityEngine;

namespace Catharsis
{
    [Implements(typeof(IPathUtility), InjectionBindingScope.CROSS_CONTEXT)]

    public class PathUtility : IPathUtility
    {
        public string GetUserInputSaveFolder()
        {
            //C:\Users\<user_name>\AppData\LocalLow\<company_name>\<product_name>
            //Debug.Log(Application.persistentDataPath);

            return string.Format("{0}/InputManager", Application.persistentDataPath);
        }

        public string GetDefaultInputSaveFolder()
        {
            return string.Format("{0}/Resources/InputManager", Application.dataPath);
            
        }
    }
}