using System;
using strange.framework.api;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
    public class ResourceInstanceProvider : IInstanceProvider
    {
        //The GameObject instantiated from the prefab
        private GameObject prototype;

        //The name of the resource in Unity's resources folder
        private string resourceName;
        //The render layer to which the GAmeObjects will be assigned;
        private int layer;
        //THe id tacked on to the name to make it easier to track individual instances
        private int id = 0;

        public ResourceInstanceProvider(string name, int layer)
        {
            resourceName = name;
            this.layer = layer;
        }

        public T GetInstance<T>()
        {
            object instance = GetInstance(typeof (T));
            T retv = (T) instance;
            return retv;
        }

        public object GetInstance(Type key)
        {
            if (prototype == null)
            {
                //Get the resource from Unity
                prototype = Resources.Load<GameObject>("Prefabs/" + resourceName);
                prototype.transform.localScale = Vector3.one;
            }

            //Copy the prototype
            GameObject go = GameObject.Instantiate(prototype) as GameObject;
            go.layer = layer;
            go.name = resourceName + "_" + id++;
            return go;
        }
    }

}