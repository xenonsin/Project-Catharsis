using UnityEngine;
using strange.extensions.mediation.impl;
using System;

namespace Catharsis.UI
{
    public class PauseMenuMediator : Mediator
    {

        [Inject]
        public PauseMenuView view { get; set; }

        [Inject]
        public ShowPauseMenuSignal ShowPauseMenuSignal { get; set; }

        [Inject]
        public ClosePauseMenuSignal ClosePauseMenuSignal { get; set; }

        [Inject]
        public ShowMainPageSignal ShowMainPageSignal { get; set; }
        
        [Inject]
        public ShowControlPageSignal ShowControlPageSignal { get; set; }
        public override void OnRegister()
        {
            ShowControlPageSignal.AddListener(Show);
            ClosePauseMenuSignal.AddListener(Close);
            ShowMainPageSignal.AddListener(ChangeToMainPage);
            ShowControlPageSignal.AddListener(ChangeToControlPage);
            view.Init();
            base.OnRegister();
        }

        public override void OnRemove()
        {

            ShowControlPageSignal.RemoveListener(Show);
            ClosePauseMenuSignal.RemoveListener(Close);
            ShowMainPageSignal.RemoveListener(ChangeToMainPage);
            ShowControlPageSignal.RemoveListener(ChangeToControlPage);
            base.OnRemove();
        }

        //[ListensTo(typeof(ShowPauseMenuSignal))] //coming in v1.0!
        private void Show()
        {
            view.Show();           
        }

        private void Close()
        {
            view.Close();
        }

        private void ChangeToMainPage()
        {
            view.ChangeToMainPage();
        }

        private void ChangeToControlPage()
        {
            view.ChangeToControlsPage();
        }


    }
}