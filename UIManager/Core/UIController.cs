using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace UIFramework
{
    public abstract class UIController : MonoBehaviour
    {
        protected Dictionary<string, UIBase> _registeredScreens;

        /// <summary>
        /// UI를 불러오는 함수
        /// </summary>
        /// <typeparam name="T">UI 클래스 이름</typeparam>
        protected abstract UniTask<T> OpenUI<T>() where T : UIBase;

        /// <summary>
        /// UI를 불러오는 함수 (UIPriority를 사용)
        /// </summary>
        /// <typeparam name="T">UI 클래스 이름</typeparam>
        /// <param name="priority">UI우선 순위</param>
        protected abstract UniTask<T> OpenUI<T>(UIPriority priority) where T : UIBase;

        /// <summary>
        /// UI를 숨기는 함수
        /// </summary>
        /// <typeparam name="T">UI 클래스 이름</typeparam>
        public abstract void CloseUI<T>() where T : UIBase;
        public abstract void OpenUI(UIBase screen);
        public abstract void OpenUI(UIBase screen, UIPriority priority);
        public abstract void CloseUI(UIBase screen);

        public virtual void Initialize()
        {
            _registeredScreens = new Dictionary<string, UIBase>();
        }

        protected void RegisterUI(string screenId, UIBase controller)
        {
            if (_registeredScreens.ContainsKey(screenId) == false)
            {
                ProcessUIRegister(screenId, controller);
            }
            else
            {
                Debug.LogError("이미 등록되어 있습니다 " + screenId);
            }
        }
        
        public async UniTask<T> OpenUIByName<T>() where T : UIBase
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            if (_registeredScreens.ContainsKey(uiName))
            {
                return await OpenUI<T>();
            }

            var ui = await ResourceManager.Instance.LoadUIPrefabAsync<T>();
            RegisterUI(uiName, ui);
            return await OpenUI<T>();
        }

        public async UniTask<T> OpenUIByName<T>(UIPriority priority) where T : UIBase
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            if (_registeredScreens.ContainsKey(uiName))
            {
                return await OpenUI<T>(priority);
            }

            var ui = await ResourceManager.Instance.LoadUIPrefabAsync<T>();
            RegisterUI(uiName, ui);
            return await OpenUI<T>(priority);
        }
        
        public virtual void CloseAll(bool animated = true)
        {
            foreach (var screen in _registeredScreens)
            {
                screen.Value.Finish(animated);
            }
        }
        
        public T GetUI<T>() where T : UIBase
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            if (_registeredScreens.ContainsKey(uiName))
            {
                return _registeredScreens[uiName] as T;
            }
            
            return null;
        }

        protected virtual void ProcessUIRegister(string screenId, UIBase controller)
        {
            _registeredScreens.Add(screenId, controller);
        }
    }
}