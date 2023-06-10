namespace UIFramework
{
    public abstract class PopupUI : UIBase
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