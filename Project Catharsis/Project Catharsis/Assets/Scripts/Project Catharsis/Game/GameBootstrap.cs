//Every context starts by attaching a contextview to a gameobject
//The main job of this is to instantiate the Context
//requires you to name a context in config/GameContext

using strange.extensions.context.impl;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
    public class GameBootstrap : ContextView
    {
        void Start()
        {
            context = new GameContext(this);
        }
    }

}