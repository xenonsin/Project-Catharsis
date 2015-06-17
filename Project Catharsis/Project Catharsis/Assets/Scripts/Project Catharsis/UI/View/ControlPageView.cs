using System;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace Catharsis.UI
{
    public class ControlPageView : View
    {
        public Button BackButton;
        public Button DefaultButton;

        internal Signal BackButtonClickSignal = new Signal();
        internal Signal DefaultButtonClickSignal = new Signal();


        internal void Init()
        {
            BackButton.onClick.AddListener(Back);
            DefaultButton.onClick.AddListener(Default);
        }

        internal void Remove()
        {
            BackButton.onClick.RemoveAllListeners();
            DefaultButton.onClick.RemoveAllListeners();
        }

        private void Back()
        {
            BackButtonClickSignal.Dispatch();
        }

        private void Default()
        {
            DefaultButtonClickSignal.Dispatch();
        }
    }
}