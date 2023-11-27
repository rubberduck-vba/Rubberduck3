namespace Rubberduck.UI.Windows
{
    public interface IDialogService<TViewModel>
        where TViewModel : IDialogWindowViewModel
    {
        TViewModel ShowDialog();
    }
}
