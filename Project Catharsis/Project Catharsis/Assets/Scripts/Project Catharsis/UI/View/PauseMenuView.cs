using System;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
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
           // isOpen = false;
            //canvas.gameObject.SetActive(false);
            Show();
           
        }

        internal void Show()
        {
            if (!isOpen)
            {
                isOpen = true;
                canvas.gameObject.SetActive(true);
                ChangeToMainPage();
            }
            //pause?
        }

        internal void Close()
        {
            if (isOpen)
            {
                isOpen = false;
                canvas.gameObject.SetActive(false);
                //unpause?
            }
        }

        internal void ChangeToMainPage()
        {
            controlsPage.SetActive(false);
            mainPage.SetActive(true);
        }

        internal void ChangeToControlsPage()
        {
            mainPage.SetActive(false);
            controlsPage.SetActive(true);
        }


    }
}