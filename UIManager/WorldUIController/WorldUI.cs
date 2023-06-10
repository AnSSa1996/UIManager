using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

namespace UIFramework
{
    public class WorldUI : UIBase
    {
        public override void Close()
        {
            UIManager.Instance.CloseUI(this);
        }
        
        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
    }
}
