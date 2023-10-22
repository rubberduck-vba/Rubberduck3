namespace Rubberduck.UI
{
    public interface IWindowFactory<TView, TViewModel>
    {
        TView Create(TViewModel model);
    }
}
