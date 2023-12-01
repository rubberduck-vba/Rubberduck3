namespace Rubberduck.UI.Windows.ToolWindows
{

    public interface IToolWindowService<TViewModel>
        where TViewModel : IWindowViewModel
    {
        void ShowFloating(TViewModel viewModel);
        void ShowDocked(TViewModel viewModel, ToolDockLocation location);
    }
}
