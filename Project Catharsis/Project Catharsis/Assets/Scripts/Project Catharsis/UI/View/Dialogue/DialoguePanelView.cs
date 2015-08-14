using Catharsis.DialogueEditor.Model.Data;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Catharsis.UI
{
    public class DialoguePanelView : View
    {
        public Button ContinueButton;
        internal Signal ContinueButtonClickSignal = new Signal();

        public Scrollbar Scrollbar;

        public RectTransform ScrollRect;
        private float _scrollRectHieght;

        internal void Init()
        {
            ContinueButton.onClick.AddListener(Continue);
            _scrollRectHieght = ScrollRect.sizeDelta.y;
            ResetScrollBar();
        }

        internal void Remove()
        {
            ContinueButton.onClick.RemoveListener(Continue);
        }

        internal void DisplayMessage(TextData textData)
        {
            
        }

        internal void Continue()
        {
            ContinueButtonClickSignal.Dispatch();
            
        }

        void ResetScrollBar()
        {
            Scrollbar.value = 0;
            Canvas.ForceUpdateCanvases();

        }

        void DisableContinueButton()
        {
            ContinueButton.gameObject.SetActive(false);
        }

        void EnableContinueButton()
        {
            ContinueButton.gameObject.SetActive(true);
        }
    }
}