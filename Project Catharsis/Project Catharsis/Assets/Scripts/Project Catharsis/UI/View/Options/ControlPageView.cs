using System;
using Catharsis.InputEditor;
using Catharsis.InputEditor.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace Catharsis.UI
{
    public class ControlPageView : View
    {
        public Button BackButton;
        public Button DefaultButton;

        public RebindInput ForwardInput;
        public RebindInput BackwardInput;
        public RebindInput RightInput;
        public RebindInput LeftInput;

        internal Signal BackButtonClickSignal = new Signal();
        internal Signal DefaultButtonClickSignal = new Signal();


        internal void Init(IInputManager input)
        {
            BackButton.onClick.AddListener(Back);
            DefaultButton.onClick.AddListener(Default);

            ForwardInput.Init(input);
            BackwardInput.Init(input);
            RightInput.Init(input);
            LeftInput.Init(input);
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

        internal void Loaded()
        {
            ForwardInput.InitializeAxisConfig();
            BackwardInput.InitializeAxisConfig();
            RightInput.InitializeAxisConfig();
            LeftInput.InitializeAxisConfig();

        }
        internal void HandleDirty(string name)
        {
            ForwardInput.HandleConfigurationDirty(name);
            BackwardInput.HandleConfigurationDirty(name);
            RightInput.HandleConfigurationDirty(name);
            LeftInput.HandleConfigurationDirty(name);
        }
    }
}