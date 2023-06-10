using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField] private ATransitionComponent animIn;

        [SerializeField] private ATransitionComponent animOut;

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

        public Action<UIBase> OpenFinishAction { get; set; }
        public Action<UIBase> CloseFinishAction { get; set; }
        public bool IsVisible { get; private set; }

        public UIPriority Priority { get; private set; }

        protected Vector2 _originPos = Vector2.one;
        protected Quaternion _originRot = Quaternion.identity;
        protected RectTransform _rectTransform = null;

        protected virtual void Awake()
        {
            AwakeInit();
            BindingUI();
            BindingEvent();
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform.IsUnityNull()) return;
            _originPos = _rectTransform.anchoredPosition;
            _originRot = _rectTransform.rotation;
        }

        protected virtual void AwakeInit()
        {
            if (AnimIn.IsUnityNull()) AnimIn = transform.GetOrAddComponent<SimpleTransition>();
            if (AnimOut.IsUnityNull()) AnimOut = transform.GetOrAddComponent<SimpleTransition>();
        }

        protected virtual void BindingUI()
        {
            string[] enumStrings = System.Enum.GetNames(GetType().GetNestedType("EComponents"));
            var allTransform = transform.GetComponentsInChildren<RectTransform>(true);
            foreach (var currentTransform in allTransform)
            {
                var currentName = currentTransform.name;
                foreach (var enumString in enumStrings)
                {
                    if (currentName.Contains(enumString))
                    {
                        _bindingWidgetTransformDictionary.Add(enumString, currentTransform);
                    }
                }
            }
        }

        protected virtual void BindingEvent()
        {
        }

        protected T GetControl<T>(Enum enumType) where T : Component
        {
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out var componentTransform) == false) return null;
            return componentTransform.GetComponent<T>();
        }

        protected void BindingEvent<T>(Enum enumType, UnityAction action) where T : Component
        {
            Transform componentTransform = null;
            var enumName = enumType.ToString();
            if (_bindingWidgetTransformDictionary.TryGetValue(enumName, out componentTransform) == false) return;
            var type = typeof(T);
            if (typeof(UIButton).IsAssignableFrom(type))
            {
                var uiButton = componentTransform.GetComponent<UIButton>();
                uiButton?.onClick.AddListener(() => action());
            }
        }

        protected virtual void HierarchyFixOnShow()
        {
        }

        public void Finish(bool animate = true)
        {
            DoAnimation(animate ? animOut : null, false, OnTransitionOutFinished, false);
        }

        public void Open(UIPriority priority = default)
        {
            transform.gameObject.SetActive(true);
            Priority = priority;
            HierarchyFixOnShow();
            _rectTransform.anchoredPosition = _originPos;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.rotation = _originRot;
            OpenInit();
            DoAnimation(animIn, true, OnTransitionInFinished, true);
        }

        protected virtual void OpenInit()
        {
        }

        public virtual void Close()
        {
        }

        private void DoAnimation(ATransitionComponent caller, bool fadeIn, UnityAction callWhenFinished, bool isVisible)
        {
            if (caller == null)
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

        private void OnTransitionInFinished()
        {
            IsVisible = true;

            if (OpenFinishAction != null)
            {
                OpenFinishAction(this);
            }
        }

        private void OnTransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            if (CloseFinishAction != null)
            {
                CloseFinishAction(this);
            }
        }
    }
}