using Rubberduck.Editor.Message;

namespace Rubberduck.Editor.Shell
{
    public interface IDialogService<TViewModel>
        where TViewModel : IDialogWindowViewModel
    {
        TViewModel ShowDialog();
    }
}
