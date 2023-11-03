namespace Rubberduck.UI
{
    public interface IDialogService<TViewModel>
        where TViewModel : IDialogWindowViewModel
    {
        TViewModel ShowDialog();
    }
}
