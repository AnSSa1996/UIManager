using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIFramework
{
    public class PopupUIController : UIController
    {
        [SerializeField]
        private PopupUIParaLayer priorityParaLayer = null;

        public PopupUI CurrentPopupUI { get; private set; }

        private Queue<PopupUIHistoryEntry> _windowQueue;
        private Stack<PopupUIHistoryEntry> _windowHistory;

        public override void Initialize()
        {
            base.Initialize();
            _registeredScreens = new Dictionary<string, UIBase>();
            _windowQueue = new Queue<PopupUIHistoryEntry>();
            _windowHistory = new Stack<PopupUIHistoryEntry>();
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

            if (ShouldEnqueue(popupUI))
            {
                EnqueueWindow(popupUI, priority);
            }
            else
            {
                DoOpen(popupUI, priority);
            }

            return new UniTask<T>(screen);
        }

        public override void CloseUI<T>()
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var screen = _registeredScreens[uiName] as T;
            if (screen == CurrentPopupUI)
            {
                _windowHistory.Pop();
                screen.Finish();

                CurrentPopupUI = null;

                if (_windowQueue.Count > 0)
                {
                    OpenNextInQueue();
                }
                else if (_windowHistory.Count > 0)
                {
                    OpenPreviousInHistory();
                }
            }
        }

        public override void OpenUI(UIBase screen)
        {
            var popupUI = screen as PopupUI;

            if (popupUI.IsUnityNull()) return;
            
            if (ShouldEnqueue(popupUI))
            {
                EnqueueWindow(popupUI);
            }
            else
            {
                DoOpen(popupUI);
            }
        }

        public override void OpenUI(UIBase screen, UIPriority priority)
        {
            var popupUI = screen as PopupUI;

            if (popupUI.IsUnityNull()) return;
            
            if (ShouldEnqueue(popupUI))
            {
                EnqueueWindow(popupUI, priority);
            }
            else
            {
                DoOpen(popupUI, priority);
            }
        }

        public override void CloseUI(UIBase screen)
        {
            if (screen == CurrentPopupUI)
            {
                _windowHistory.Pop();
                screen.Finish();

                CurrentPopupUI = null;

                if (_windowQueue.Count > 0)
                {
                    OpenNextInQueue();
                }
                else if (_windowHistory.Count > 0)
                {
                    OpenPreviousInHistory();
                }
            }
        }

        public override void CloseAll(bool shouldAnimateWhenHiding = true)
        {
            base.CloseAll(shouldAnimateWhenHiding);
            CurrentPopupUI = null;
            priorityParaLayer.RefreshDarken();
            _windowHistory.Clear();
        }

        private void EnqueueWindow(PopupUI screen, UIPriority priority = UIPriority.Default)
        {
            _windowQueue.Enqueue(new PopupUIHistoryEntry(screen, priority));
        }

        private bool ShouldEnqueue(PopupUI controllerController)
        {
            if (CurrentPopupUI == null && _windowQueue.Count == 0)
            {
                return false;
            }
            
            if (controllerController.Priority != UIPriority.Default)
            {
                return true;
            }

            return false;
        }

        private void OpenPreviousInHistory()
        {
            if (_windowHistory.Count > 0)
            {
                PopupUIHistoryEntry popupUI = _windowHistory.Pop();
                DoOpen(popupUI);
            }
        }

        private void OpenNextInQueue()
        {
            if (_windowQueue.Count > 0)
            {
                PopupUIHistoryEntry popupUI = _windowQueue.Dequeue();
                DoOpen(popupUI);
            }
        }

        private void DoOpen(PopupUI screen, UIPriority priority = UIPriority.Default)
        {
            DoOpen(new PopupUIHistoryEntry(screen, priority));
        }

        private void DoOpen(PopupUIHistoryEntry popupUIEntry)
        {
            if (CurrentPopupUI == popupUIEntry.Screen)
            {
                Debug.LogWarning($"열려있는 PopupUI를 다시 열려고 시도했습니다. {popupUIEntry.Screen.name}");
                return;
            }
            
            if (CurrentPopupUI != null)
            {
                CurrentPopupUI.Finish();
            }

            _windowHistory.Push(popupUIEntry);
            priorityParaLayer.DarkenBG();

            CurrentPopupUI = popupUIEntry.Screen;
            popupUIEntry.Screen.transform.SetParent(priorityParaLayer.transform);
            popupUIEntry.Open();
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