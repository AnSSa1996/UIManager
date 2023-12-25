namespace UIFramework
{
    public class ScreenUI : UIBase
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
    }
}