namespace Rubberduck.UI.Windows
{
    public interface IWindowFactory<TView, TViewModel>
    {
        TView Create(TViewModel model);
    }
}
