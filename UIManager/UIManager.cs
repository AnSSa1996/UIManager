using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIManager : Singleton<UIManager>
    {
        private ScreenUIController _screenController;
        private PopupUIController _popupController;
        [SerializeField] private WorldUIController _worldUIController;

        private Canvas _mainCanvas;
        private GraphicRaycaster _graphicRaycaster;

        public Canvas MainCanvas
        {
            get
            {
                if (_mainCanvas == null)
                {
                    _mainCanvas = GetComponent<Canvas>();
                }

                return _mainCanvas;
            }
        }

        public Camera UICamera
        {
            get { return MainCanvas.worldCamera; }
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public virtual void Initialize()
        {
            if (_screenController == null)
            {
                _screenController = gameObject.GetComponentInChildren<ScreenUIController>(true);
                _screenController.Initialize();
            }

            if (_popupController == null)
            {
                _popupController = gameObject.GetComponentInChildren<PopupUIController>(true);
                _popupController.Initialize();
            }

            if (_worldUIController.IsUnityNull())
            {
                Debug.LogWarning($"WorldUIController를 추가해주시기 바랍니다.");
                return;
            }

            _worldUIController.Initialize();
            DontDestroyOnLoad(_worldUIController);

            _graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }


        /// <summary>
        /// UI를 열어주는 함수
        /// </summary>
        /// <typeparam name="T">UI 클래스</typeparam>
        /// <param name="priority">UI 우선순위</param>
        public async UniTask<T> OpenUI<T>(UIPriority priority = UIPriority.Default) where T : UIBase
        {
            Type type = typeof(T);
            if (typeof(PopupUI).IsAssignableFrom(type))
            {
                return await OpenPopUI<T>();
            }

            if (typeof(PopupUI).IsAssignableFrom(type) && priority != UIPriority.Default)
            {
                return await OpenPopupUI<T>(priority);
            }

            if (typeof(ScreenUI).IsAssignableFrom(type))
            {
                return await OpenScreenUI<T>();
            }

            if (typeof(ScreenUI).IsAssignableFrom(type) && priority != UIPriority.Default)
            {
                return await OpenScreenUI<T>(priority);
            }

            return null;
        }

        public async UniTask<T> OpenUI<T>(Vector3 position) where T : UIBase
        {
            Type type = typeof(T);
            if (typeof(WorldUI).IsAssignableFrom(type))
            {
                return await OpenWorldUI<T>(position);
            }

            return null;
        }

        public void CloseUI<T>(bool animate = true) where T : UIBase
        {
            Type type = typeof(T);
            if (typeof(PopupUI).IsAssignableFrom(type))
            {
                ClosePopUI<T>();
            }

            if (typeof(ScreenUI).IsAssignableFrom(type))
            {
                CloseScreenUI<T>();
            }
        }

        public void CloseUI(UIBase ui, bool animate = true)
        {
            Type type = ui.GetType();
            if (typeof(PopupUI).IsAssignableFrom(type))
            {
                _popupController.CloseUI(ui);
            }

            if (typeof(ScreenUI).IsAssignableFrom(type))
            {
                _screenController.CloseUI(ui);
            }

            if (typeof(WorldUI).IsAssignableFrom(type))
            {
                _worldUIController.CloseUI(ui);
            }
        }

        public void CloseAllScreenUI(bool animate = true)
        {
            _screenController.CloseAll(animate);
        }

        public void CloseAllPopupUI(bool animate = true)
        {
            _popupController.CloseAll(animate);
        }

        public void CloseCurrentPopUI()
        {
            if (_popupController.CurrentPopupUI != null)
            {
                _popupController.CloseUI(_popupController.CurrentPopupUI);
            }
        }

        public T GetUI<T>() where T : UIBase
        {
            Type type = typeof(T);
            if (typeof(PopupUI).IsAssignableFrom(type))
            {
                return _popupController.GetUI<T>();
            }

            if (typeof(ScreenUI).IsAssignableFrom(type))
            {
                return _screenController.GetUI<T>();
            }

            return null;
        }

        private async UniTask<T> OpenScreenUI<T>() where T : UIBase
        {
            return await _screenController.OpenUIByName<T>();
        }

        private async UniTask<T> OpenScreenUI<T>(UIPriority priority) where T : UIBase
        {
            return await _screenController.OpenUIByName<T>(priority);
        }

        private void CloseScreenUI<T>() where T : UIBase
        {
            _screenController.CloseUI<T>();
        }

        private async UniTask<T> OpenPopUI<T>() where T : UIBase
        {
            return await _popupController.OpenUIByName<T>();
        }

        private async UniTask<T> OpenPopupUI<T>(UIPriority priority = UIPriority.Default) where T : UIBase
        {
            return await _popupController.OpenUIByName<T>(priority);
        }

        private void ClosePopUI<T>() where T : UIBase
        {
            _popupController.CloseUI<T>();
        }

        public async UniTask<T> OpenWorldUI<T>(Vector3 position) where T : UIBase
        {
            var worldUI = await _worldUIController.OpenUIByName<T>();
            var screenPosition = UICamera.WorldToScreenPoint(position);
            var worldCanvas = _worldUIController.WorldCanvas;
            var canvasRectTransform = worldCanvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, UICamera,
                out Vector2 localPosition);
            var rectTransform = worldUI.GetComponent<RectTransform>();
            rectTransform.localPosition = localPosition;
            rectTransform.localRotation = worldCanvas.worldCamera.transform.localRotation;
            return worldUI;
        }

        public void CloseWorldUI(UIBase ui)
        {
            _worldUIController.CloseUI(ui);
        }

        public void CloseAllWorldUI()
        {
            _worldUIController.CloseAll();
        }
    }
}