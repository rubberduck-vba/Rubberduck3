namespace Rubberduck.UI.Windows
{
    public interface IToolWindowViewModel : IWindowViewModel
    {
        DockingLocation DockingLocation { get; set; }
    }

    public interface IWindowViewModel
    {
        string Title { get; }
    }
}
