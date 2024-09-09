using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIManager : Singleton<UIManager>
    {
        private const float DEFAULT_RESOLUTION_WIDTH = 1920f;
        private const float DEFAULT_RESOLUTION_HEIGHT = 1080f;   
        private const float DEFAULT_RESOLUTION_RATIO = 1.777777777777778f;
        private const float DEFAULT_FILED_OF_VIEW = 38.5f;
        
        private ScreenUIController _screenController;
        private PopupUIController _popupController;
        [SerializeField] private WorldUIController _worldUIController;

        private Canvas _mainCanvas;
        private GraphicRaycaster _graphicRaycaster;
        
        
        private float _currentResolutionWidth = 0f;
        private float _currentResolutionHeight = 0f;
        private float _currentResolutionRatio = 0f;

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
        
        public Camera WorldCamera
        {
            get { return _worldUIController.WorldCamera; }
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
            InitResoultion();
        }

        private void InitResoultion()
        {
            _currentResolutionWidth = Screen.width;
            _currentResolutionHeight = Screen.height;
            _currentResolutionRatio = _currentResolutionWidth / _currentResolutionHeight;

            if (DEFAULT_RESOLUTION_RATIO <= _currentResolutionRatio) return;
            float fieldOfView = DEFAULT_FILED_OF_VIEW * (DEFAULT_RESOLUTION_RATIO / _currentResolutionRatio);
            UICamera.fieldOfView = fieldOfView;
            
            var canvasScaler = GetComponent<CanvasScaler>();
            canvasScaler.matchWidthOrHeight = 0f;
        }


        /// <summary>
        /// UI를 열어주는 함수
        /// </summary>
        /// <typeparam name="T">UI 클래스</typeparam>
        /// <param name="priority">UI 우선순위</param>
        public async UniTask<T> OpenUI<T>(UIPriority priority = UIPriority.Default) where T : UIBase
        {
            Type type = typeof(T);

            if (typeof(PopupUI).IsAssignableFrom(type) && priority != UIPriority.Default)
            {
                return await OpenPopupUI<T>(priority);
            }

            if (typeof(PopupUI).IsAssignableFrom(type))
            {
                return await OpenPopUI<T>();
            }


            if (typeof(ScreenUI).IsAssignableFrom(type) && priority != UIPriority.Default)
            {
                return await OpenScreenUI<T>(priority);
            }

            if (typeof(ScreenUI).IsAssignableFrom(type))
            {
                return await OpenScreenUI<T>();
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

        public void CloseUI(UIBase ui)
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

        public void CloseAllUI()
        {
            CloseAllScreenUI(false);
            CloseAllPopupUI(false);
            CloseAllWorldUI();
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

            if (typeof(WorldUI).IsAssignableFrom(type))
            {
                return _worldUIController.GetUI<T>();
            }
            
            return null;
        }

        public UIBase GetUI(string uiName)
        {
            UIBase result;
            result = _popupController.GetUI(uiName);
            if (result.IsUnityNull() == false) return result;
            result = _screenController.GetUI(uiName);
            if (result.IsUnityNull() == false) return result;
            result = _worldUIController.GetUI(uiName);
            if (result.IsUnityNull() == false) return result;
            return result;
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
            var rectTransform = worldUI.GetComponent<Transform>();
            rectTransform.InitLocalTransform();
            rectTransform.position = position;
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