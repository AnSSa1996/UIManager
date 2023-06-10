namespace UIFramework
{
    public class ScreenUI : UIBase
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