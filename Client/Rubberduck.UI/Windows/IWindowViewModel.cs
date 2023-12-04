namespace Rubberduck.UI.Windows
{
    public interface IToolWindowViewModel : IWindowViewModel
    {

    }

    public interface IWindowViewModel
    {
        string Title { get; }
    }
}
