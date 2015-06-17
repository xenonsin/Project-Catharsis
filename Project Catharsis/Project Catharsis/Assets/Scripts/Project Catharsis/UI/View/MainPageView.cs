using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;
using UnityEngine;

namespace Catharsis.UI
{
    public class MainPageView : View
    {
        public Button ResumeButton;
        public Button ControlsButton;
        public Button QuitButton;

        internal Signal ResumeButtonClickSignal = new Signal ();
        internal Signal ControlButtonClickSignal = new Signal();
        internal Signal QuitButtonClickSignal = new Signal();

        internal void Init()
        {
            ResumeButton.onClick.AddListener(Resume);
            ControlsButton.onClick.AddListener(ShowControlPage);
            QuitButton.onClick.AddListener(QuitButtonClickSignal.Dispatch);
        }

        internal void Remove()
        {
            ResumeButton.onClick.RemoveAllListeners();
            ControlsButton.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();
        }

        private void Resume()
        {
            ResumeButtonClickSignal.Dispatch();
        }

        private void ShowControlPage()
        {
            ControlButtonClickSignal.Dispatch();
        }
    }
}