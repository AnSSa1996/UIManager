using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIFramework
{
    public class WorldUIController : MonoBehaviour
    {
        public Canvas WorldCanvas = null;
        public GraphicRaycaster WorldGraphicRaycaster = null;
        private List<UIBase> _allWorldList = new List<UIBase>();

        public Camera WorldCamera
        {
            get
            {
                if (WorldCanvas.worldCamera.IsUnityNull())
                {
                    WorldCanvas.worldCamera = Camera.main;
                }
                return WorldCanvas.worldCamera;
            }
        }

        public virtual void Initialize()
        {
            _allWorldList = new List<UIBase>();

            if (WorldGraphicRaycaster == null)
            {
                WorldGraphicRaycaster = WorldCanvas.GetComponent<GraphicRaycaster>();
            }
        }

        public async UniTask<T> OpenUIByName<T>() where T : UIBase
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var ui = await ResourceManager.Instance.LoadUIPrefabAsync<T>();
            ReparentToParaLayer(ui.transform);
            _allWorldList.Add(ui);
            ui.Open();
            return ui;
        }

        public async UniTask<T> OpenUIByName<T>(UIPriority priority) where T : UIBase
        {
            var uiName = PublicStaticMethod.GetTypeName<T>();
            var ui = await ResourceManager.Instance.LoadUIPrefabAsync<T>();
            _allWorldList.Add(ui);
            ReparentToParaLayer(ui.transform);
            ui.Open();
            return ui;
        }

        public virtual void CloseAll(bool animated = true)
        {
            foreach (var worldUI in _allWorldList)
            {
                worldUI.CloseFinishAction = (screen) =>
                {
                    screen.gameObject.Release();
                };
                worldUI.Finish();
            }

            _allWorldList.Clear();
        }

        public void OpenUI(UIBase screen)
        {
            ReparentToParaLayer(screen.transform);
            screen.Open();
        }

        public void OpenUI(UIBase screen, UIPriority priority)
        {
            ReparentToParaLayer(screen.transform, priority);
            screen.Open();
        }

        public void CloseUI(UIBase screen)
        {
            screen.CloseFinishAction = (screen) =>
            {
                _allWorldList.Remove(screen);
                screen.gameObject.Release();
            };
            screen.Finish();
        }

        private void ReparentToParaLayer(Transform screenTransform, UIPriority priority = UIPriority.Default)
        {
            if (WorldCanvas.IsUnityNull()) return;
            screenTransform.SetParent(WorldCanvas.transform, false);
        }
    }
}