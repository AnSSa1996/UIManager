using UnityEngine.Serialization;

namespace UIFramework
{
    public abstract class PopupUI : UIBase
    {
        public bool useDarkenBG = true;
        
        public override void Close()
        {
            UIManager.Instance.CloseUI(this);
            base.Close();
        }
        
        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
    }
}