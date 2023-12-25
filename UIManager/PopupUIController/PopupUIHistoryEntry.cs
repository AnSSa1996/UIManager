using Cysharp.Threading.Tasks;

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

        public async UniTask Open()
        {
            await Screen.Open(Priority);
        }
    }
}