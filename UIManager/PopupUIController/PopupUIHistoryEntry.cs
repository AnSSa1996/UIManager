namespace UIFramework
{
    public struct PopupUIHistoryEntry
    {
        public readonly PopupUI Screen;
        public readonly UIPriority Priority;

        public PopupUIHistoryEntry(PopupUI screen, UIPriority priority)
        {
            Screen = screen;
            Priority = priority;
        }

        public void Open()
        {
            Screen.Open(Priority);
        }
    }
}