using strange.extensions.mediation.impl;
using UnityEngine;

namespace Catharsis.UI
{
    public class DialoguePageView : View
    {
        public Canvas canvas;
        public GameObject dialoguePanel;

        private bool _isOpen;

        internal void Init()
        {
            _isOpen = false;
            canvas.gameObject.SetActive(false);
        }

        internal void Show()
        {
            if (!_isOpen)
            {
                _isOpen = true;
                canvas.gameObject.SetActive(true);
            }
            //pause?
        }

        internal void Close()
        {
            if (_isOpen)
            {
                _isOpen = false;
                canvas.gameObject.SetActive(false);
                //unpause?
            }
        }

        internal void ShowDialoguePanel()
        {
            dialoguePanel.SetActive(true);
        }
    }
}