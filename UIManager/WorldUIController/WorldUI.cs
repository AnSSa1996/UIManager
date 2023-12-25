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
            base.Close();
        }
        
        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
        
        protected override void OnTransitionOutFinished()
        {
            base.OnTransitionOutFinished();
            gameObject.Release();
        }
    }
}
