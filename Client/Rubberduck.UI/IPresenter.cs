using Rubberduck.UI.Services;

namespace Rubberduck.UI
{
    public interface IPresenter
    {
        void Show(WindowSize windowSize = WindowSize.MediumDialog);
        void Hide();
        void Close();
    }
}
