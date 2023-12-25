using UnityEngine;
using System.Collections.Generic;

namespace UIFramework
{
    public class PopupUIParaLayer : MonoBehaviour
    {
        [SerializeField] private GameObject darkenBgObject = null;

        private List<PopupUI> containedScreens = new List<PopupUI>();

        public void AddScreen(PopupUI screenRectTransform)
        {
            screenRectTransform.transform.SetParent(transform, false);
            if (containedScreens.Contains(screenRectTransform)) return;
            containedScreens.Add(screenRectTransform);
        }

        public void RefreshDarken()
        {
            for (int i = 0; i < containedScreens.Count; i++)
            {
                if (containedScreens[i] != null)
                {
                    if (containedScreens[i].gameObject.activeInHierarchy && containedScreens[i].useDarkenBG)
                    {
                        darkenBgObject.transform.SetSiblingIndex(containedScreens[i].transform.GetSiblingIndex() - 1);
                        darkenBgObject.SetActive(true);
                        return;
                    }
                }
            }

            darkenBgObject.SetActive(false);
        }
    }
}