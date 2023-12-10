namespace Rubberduck.UI.Windows
{
    public interface IToolWindowViewModel : ITabViewModel
    {
        DockingLocation DockingLocation { get; set; }
    }
}
