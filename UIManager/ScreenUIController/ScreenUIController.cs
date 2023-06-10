using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    public class ScreenUIController : UIController
    {
        [SerializeField]
        private ScreenPriorityLayerList priorityLayers = null;

        protected override UniTask<T> OpenUI<T>(UIPriority priority)
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var screen = _registeredScreens[uiName] as T;
            ReparentToParaLayer(screen.transform, priority);
            screen.Open();
            return new UniTask<T>(screen);
        }

        protected override UniTask<T> OpenUI<T>()
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var screen = _registeredScreens[uiName] as T;
            ReparentToParaLayer(screen.transform);
            screen.Open();
            return new UniTask<T>(screen);
        }

        public override void CloseUI<T>()
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var screen = _registeredScreens[uiName] as T;;
            screen.Finish();
        }

        public override void OpenUI(UIBase screen)
        {
            ReparentToParaLayer(screen.transform);
            screen.Open();
        }

        public override void OpenUI(UIBase screen, UIPriority priority)
        {
            ReparentToParaLayer(screen.transform, priority);
            screen.Open();
        }

        public override void CloseUI(UIBase screen)
        {
            screen.Finish();
        }

        private void ReparentToParaLayer(Transform screenTransform, UIPriority priority = UIPriority.Default)
        {
            Transform trans;
            if (priorityLayers.ParaLayerLookup.TryGetValue(priority, out trans) == false)
            {
                trans = transform;
            }

            screenTransform.SetParent(trans, false);
        }
    }
}