using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIFramework
{
    public class PopupUIController : UIController
    {
        [SerializeField] private PopupUIParaLayer priorityParaLayer = null;

        public PopupUI CurrentPopupUI { get; private set; }

        private List<PopupUIHistoryEntry> _windowHistoryList;

        public override void Initialize()
        {
            base.Initialize();
            _registeredScreens = new Dictionary<string, UIBase>();
            _windowHistoryList = new List<PopupUIHistoryEntry>();
        }

        protected override void ProcessUIRegister(string screenId, UIBase controllerController)
        {
            base.ProcessUIRegister(screenId, controllerController);
            controllerController.OpenFinishAction += OnInAnimationFinished;
            controllerController.CloseFinishAction += OnOutAnimationFinished;
        }

        protected override UniTask<T> OpenUI<T>()
        {
            return OpenUI<T>(UIPriority.Default);
        }

        protected override UniTask<T> OpenUI<T>(UIPriority priority = UIPriority.Default)
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var screen = _registeredScreens[uiName] as T;
            var popupUI = screen as PopupUI;

            if (popupUI.IsUnityNull()) return new UniTask<T>(null);
            DoOpen(popupUI, priority, popupUI.useDarkenBG);
            return new UniTask<T>(screen);
        }

        public override void CloseUI<T>()
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var screen = _registeredScreens[uiName] as T;
            if (screen != CurrentPopupUI) return;
            if (_windowHistoryList.Count == 0) return;
            _windowHistoryList.Remove(_windowHistoryList.Last());
            screen.Finish();
            CurrentPopupUI = null;
            if (_windowHistoryList.Count > 0) ChangeCurrentPopupUI();
        }

        public override void OpenUI(UIBase screen)
        {
            var popupUI = screen as PopupUI;

            if (popupUI.IsUnityNull()) return;
            DoOpen(popupUI, useDarkenBG: popupUI.useDarkenBG);
        }

        public override void OpenUI(UIBase screen, UIPriority priority)
        {
            var popupUI = screen as PopupUI;
            if (popupUI.IsUnityNull()) return;
            DoOpen(popupUI, priority, popupUI.useDarkenBG);
        }

        public override void CloseUI(UIBase screen)
        {
            if (screen != CurrentPopupUI) return;
            _windowHistoryList.Remove(_windowHistoryList.Last());
            screen.Finish();
            CurrentPopupUI = null;
            if (_windowHistoryList.Count > 0) ChangeCurrentPopupUI();
        }

        public override void CloseAll(bool shouldAnimateWhenHiding = true)
        {
            base.CloseAll(shouldAnimateWhenHiding);
            CurrentPopupUI = null;
            priorityParaLayer.RefreshDarken();
            _windowHistoryList.Clear();
        }

        private void ChangeCurrentPopupUI()
        {
            if (_windowHistoryList.Count > 0 == false) return;
            PopupUIHistoryEntry popupUI = _windowHistoryList.Last();
            CurrentPopupUI = popupUI.Screen;
        }

        private void DoOpen(PopupUI screen, UIPriority priority = UIPriority.Default, bool useDarkenBG = true)
        {
            DoOpen(new PopupUIHistoryEntry(screen, priority));
        }

        private async void DoOpen(PopupUIHistoryEntry popupUIEntry)
        {
            if (popupUIEntry.IsUnityNull()) return;
            if (popupUIEntry.Screen.IsUnityNull()) return;
            if (CurrentPopupUI == popupUIEntry.Screen)
            {
                Debug.LogWarning($"열려있는 PopupUI를 다시 열려고 시도했습니다. {popupUIEntry.Screen.name}");
                return;
            }
            
            CurrentPopupUI = popupUIEntry.Screen;
            _windowHistoryList.RemoveAll(x => x.Screen == CurrentPopupUI);
            _windowHistoryList.Add(popupUIEntry);
            await popupUIEntry.Open();
            priorityParaLayer.AddScreen(popupUIEntry.Screen);
            priorityParaLayer.RefreshDarken();
        }

        private void OnInAnimationFinished(UIBase screen)
        {
        }

        private void OnOutAnimationFinished(UIBase screen)
        {
            priorityParaLayer.RefreshDarken();
        }
    }
}