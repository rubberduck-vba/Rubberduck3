namespace Rubberduck.UI.Windows.ToolWindows
{

    public interface IToolWindowService<TViewModel>
        where TViewModel : IWindowViewModel
    {
        void Float(TViewModel viewModel);
        void Dock(TViewModel viewModel, ToolDockLocation location);
    }
}
