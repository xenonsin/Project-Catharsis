using UnityEngine;
using System;
using strange.extensions.context.impl;

namespace Catharsis.UI
{
    public class UIBootstrap : ContextView
    {
        public void Start()
        {
            context = new UIContext(this);
        }
    }
}