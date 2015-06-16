using System;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Catharsis.UI
{
    public class PauseMenuView : View
    {
        public Canvas canvas;
        public GameObject mainPage;
        public GameObject controlsPage;

        private bool isOpen;

        internal void Init()
        {
            isOpen = false;
            canvas.gameObject.SetActive(false);
        }
    }
}