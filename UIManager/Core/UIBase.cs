using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace UIFramework
{
    public enum UIPriority
    {
        Default,
        Low,
        Mid,
        High,
    }

    public class UIBase : MonoBehaviour
    {
        protected enum EComponents
        {
        }

        protected Dictionary<string, Transform> _bindingWidgetTransformDictionary = new Dictionary<string, Transform>();


        [SerializeField] private bool useAnim = true;
        [SerializeField] private ATransitionComponent animIn;
        [SerializeField] private ATransitionComponent animOut;
        [SerializeField] private ATransitionComponent animShow;
        [SerializeField] private ATransitionComponent animHide;

        public ATransitionComponent AnimIn
        {
            get { return animIn; }
            set { animIn = value; }
        }

        public ATransitionComponent AnimOut
        {
            get { return animOut; }
            set { animOut = value; }
        }
        
        public ATransitionComponent AnimShow
        {
            get { return animShow; }
            set { animShow = value; }
        }
        
        public ATransitionComponent AnimHide
        {
            get { return animHide; }
            set { animHide = value; }
        }

        public Action<UIBase> OpenFinishAction { get; set; }
        public Action<UIBase> CloseFinishAction { get; set; }
        public Action<UIBase> ShowFinishAction { get; set; }
        public Action<UIBase> HideFinishAction { get; set; }
        
        public bool IsVisible { get; private set; }
        public bool IsActive { get; private set; }

        public UIPriority Priority { get; private set; }

        protected Vector2 _originPos = Vector2.one;
        protected Quaternion _originRot = Quaternion.identity;
        protected RectTransform _rectTransform = null;

        protected virtual void Awake()
        {
            BindingUI();
            BindingEvent();
            AwakeInit();
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform.IsUnityNull()) return;
            _originPos = _rectTransform.anchoredPosition;
            _originRot = _rectTransform.rotation;
        }

        protected virtual void AwakeInit()
        {
            if (AnimIn.IsUnityNull()) AnimIn = transform.GetOrAddComponent<SimpleTransition>();
            if (AnimOut.IsUnityNull()) AnimOut = transform.GetOrAddComponent<SimpleTransition>();
            if (AnimShow.IsUnityNull()) AnimShow = transform.GetOrAddComponent<SimpleTransition>();
            if (AnimHide.IsUnityNull()) AnimHide = transform.GetOrAddComponent<SimpleTransition>();
        }

        protected virtual void BindingUI()
        {
            var currentType = GetType();
            string[] enumStrings = System.Enum.GetNames(currentType.GetNestedType("EComponents"));
            var allTransform = transform.GetComponentsInChildren<RectTransform>(true);
            foreach (var currentTransform in allTransform)
            {
                var currentName = currentTransform.name;
                currentName = currentName.Replace("(Clone)", "");
                foreach (var enumString in enumStrings)
                {
                    if (currentName.Equals(enumString))
                    {
                        _bindingWidgetTransformDictionary.Add(enumString, currentTransform);
                    }
                }
            }
        }

        protected virtual void BindingEvent()
        {
        }

        public T GetControl<T>(Enum enumType) where T : Component
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out var componentTransform) == false) return null;
            return componentTransform.GetComponent<T>();
        }
        
        public T GetControl<T>(string targetName) where T : Component
        {
            if (_bindingWidgetTransformDictionary.TryGetValue(targetName, out var componentTransform) == false) return null;
            return componentTransform.GetComponent<T>();
        }

        protected void BindingEvent<T>(Enum enumType, UnityAction action, bool isForce = false) where T : Component
        {
            Transform componentTransform = null;
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out componentTransform) == false) return;
            var type = typeof(T);
            UnityAction wrappedAction = () => 
            {
                if (IsActive == false && isForce == false) return;
                action.Invoke();
            };
            
            if (typeof(UIButton).IsAssignableFrom(type))
            {
                var uiButton = componentTransform.GetComponent<UIButton>();
                if (uiButton.IsUnityNull()) return;
                uiButton.onClick.AddListener(wrappedAction);
            }

            if (typeof(UIToggleSwitch).IsAssignableFrom(type))
            {
                var uiToggleSwitch = componentTransform.GetComponent<UIToggleSwitch>();
                if (uiToggleSwitch.IsUnityNull()) return;
                uiToggleSwitch.onClick.AddListener(wrappedAction);
            }
        }

        protected void BindingEvent<T>(Enum enumType, UnityAction<bool> action) where T : Component
        {
            Transform componentTransform = null;
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out componentTransform) == false) return;
            var type = typeof(T);
            if (typeof(UIToggleCheck).IsAssignableFrom(type))
            {
                var uiToggleCheck = componentTransform.GetComponent<UIToggleCheck>();
                if (uiToggleCheck.IsUnityNull()) return;
                uiToggleCheck.onValueChanged.AddListener(action);
            }
        }

        protected void BindingEvent<T>(Enum enumType, UnityAction<float> action) where T : Component
        {
            Transform componentTransform = null;
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out componentTransform) == false) return;
            var type = typeof(T);
            if (typeof(UISlider).IsAssignableFrom(type))
            {
                var uiSlider = componentTransform.GetComponent<UISlider>();
                if (uiSlider.IsUnityNull()) return;
                uiSlider.onValueChanged.AddListener(action);
            }
        }

        protected void RemoveBindingEvent<T>(Enum enumType, UnityAction action) where T : Component
        {
            Transform componentTransform = null;
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out componentTransform) == false) return;
            var type = typeof(T);
            if (typeof(UIButton).IsAssignableFrom(type))
            {
                var uiButton = componentTransform.GetComponent<UIButton>();
                if (uiButton.IsUnityNull()) return;
                uiButton.onClick.RemoveListener(action);
            }

            if (typeof(UIToggleSwitch).IsAssignableFrom(type))
            {
                var uiToggleSwitch = componentTransform.GetComponent<UIToggleSwitch>();
                if (uiToggleSwitch.IsUnityNull()) return;
                uiToggleSwitch.onClick.RemoveListener(action);
            }
        }

        protected virtual void HierarchyFixOnShow()
        {
        }

        public void Finish(bool animate = true)
        {
            DoAnimation(animate ? animOut : null, false, OnTransitionOutFinished, false);
        }

        public async UniTask Open(UIPriority priority = default)
        {
            IsActive = true;
            Priority = priority;
            HierarchyFixOnShow();
            InitRectTransform();
            await AsyncBeforeOpenInit();
            DoAnimation(animIn, true, OnTransitionInFinished, true);
            await AsyncAfterOpenInit();
        }

        public virtual void Close()
        {
            IsActive = false;
        }

        public virtual void Show(bool animate = true)
        {
            IsActive = true;
            DoAnimation(animate ? animShow : null, true, OnTransitionShowFinished, true);
        }
        
        public virtual void Hide(bool animate = true)
        {
            IsActive = false;
            DoAnimation(animate ? animHide : null, false, OnTransitionHideFinished, false);
        }

        protected virtual async UniTask AsyncBeforeOpenInit()
        {
        }
        
        protected virtual async UniTask AsyncAfterOpenInit()
        {
            
        }

        private void InitRectTransform()
        {
            if (_rectTransform.IsUnityNull()) return;
            _rectTransform.anchoredPosition = _originPos;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.rotation = _originRot;
        }

        private void DoAnimation(ATransitionComponent caller, bool fadeIn, UnityAction callWhenFinished, bool isVisible)
        {
            if (caller == null || useAnim == false)
            {
                gameObject.SetActive(isVisible);
                if (callWhenFinished != null)
                {
                    callWhenFinished();
                }
            }
            else
            {
                if (isVisible && gameObject.activeSelf == false)
                {
                    gameObject.SetActive(true);
                }

                caller.Animate(transform, fadeIn, callWhenFinished);
            }
        }

        protected virtual void OnTransitionInFinished()
        {
            IsVisible = true;

            if (OpenFinishAction != null)
            {
                OpenFinishAction(this);
            }
        }

        protected virtual void OnTransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            if (CloseFinishAction != null)
            {
                CloseFinishAction(this);
            }
        }
        
        protected virtual void OnTransitionShowFinished()
        {
            IsVisible = true;

            if (ShowFinishAction != null)
            {
                ShowFinishAction(this);
            }
        }
        
        protected virtual void OnTransitionHideFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            if (HideFinishAction != null)
            {
                HideFinishAction(this);
            }
        }

        protected void SetButtonEnable(Enum enumType, bool isActive)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform value) == false) return;
            if (value.IsUnityNull()) return;

            var uiButton = value.GetComponent<UIButton>();
            if (uiButton.IsUnityNull()) return;
            uiButton.interactable = isActive;
        }

        protected void SetActive(Enum enumType, bool isActive)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform value) == false) return;
            if (value.IsUnityNull()) return;
            value.gameObject.SetActive(isActive);
        }

        protected void SetText(Enum enumType, string text)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform value) == false) return;
            if (value.IsUnityNull()) return;
            var textMeshProUGUI = value.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = text;
        }

        protected void SetInputFieldText(Enum enumType, string text)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform value) == false) return;
            if (value.IsUnityNull()) return;
            var textMeshProUGUI = value.GetComponent<TMP_InputField>();
            textMeshProUGUI.text = text;
        }

        protected string GetInputFieldText(Enum enumType)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform value) == false) return null;
            if (value.IsUnityNull()) return null;
            var textMeshProUGUI = value.GetComponent<TMP_InputField>();
            return textMeshProUGUI.text;
        }
        
        protected void SetProgressBar(Enum enumType, float value)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform valueTransform) == false) return;
            if (valueTransform.IsUnityNull()) return;
            var progressBar = valueTransform.GetComponent<UIProgressBar>();
            progressBar.SetValue(value);
        }

        protected void SetImage(Enum enumType, Sprite spriteImage)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform valueTransform) == false) return;
            if (valueTransform.IsUnityNull()) return;
            var image = valueTransform.GetComponent<Image>();
            image.sprite = spriteImage;
        }

        protected async void SetImage(Enum enumType, string spriteImage)
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out Transform valueTransform) == false) return;
            if (valueTransform.IsUnityNull()) return;
            var image = valueTransform.GetComponent<Image>();
            var texture2D = await ResourceManager.Instance.LoadAssetAsync<Texture2D>(spriteImage);
            image.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }
    }
}