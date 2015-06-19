using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace Catharsis.UI
{
    public class OptionsPageView : View
    {
        public Button ControlsButton;
        public Button BackButton;
        internal Signal ControlsButtonClickSignal = new Signal();
        internal Signal BackButtonClickSignal = new Signal();

        internal void Init()
        {
            ControlsButton.onClick.AddListener(ShowControlPage);
            BackButton.onClick.AddListener(ShowPauseMenu);
        }
        internal void Remove()
        {
            ControlsButton.onClick.RemoveAllListeners();
            BackButton.onClick.RemoveAllListeners();

        }

        internal void ShowControlPage()
        {
            ControlsButtonClickSignal.Dispatch();
        }

        internal void ShowPauseMenu()
        {
            BackButtonClickSignal.Dispatch();
        }
    }
}