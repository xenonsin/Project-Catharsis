using System;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace Catharsis.UI
{
    public class PausePanelMenuView : View
    {
        public Button ResumeButton;
        public Button OptionsButton;
        public Button QuitButton;
        internal Signal ResumeButtonClickSignal = new Signal();
        internal Signal OptionsButtonClickSignal = new Signal();
        internal Signal QuitButtonClickSignal = new Signal();

        internal void Init()
        {
            ResumeButton.onClick.AddListener(Resume);
            OptionsButton.onClick.AddListener(ShowControlPage);
            QuitButton.onClick.AddListener(Quit);
        }

        internal void Remove()
        {
            ResumeButton.onClick.RemoveAllListeners();
            OptionsButton.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();
        }

        private void Resume()
        {
            ResumeButtonClickSignal.Dispatch();
        }

        private void ShowControlPage()
        {
            OptionsButtonClickSignal.Dispatch();
        }

        private void Quit()
        {
            QuitButtonClickSignal.Dispatch();
        }
    }
}