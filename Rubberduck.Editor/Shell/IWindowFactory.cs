namespace Rubberduck.Editor.Shell
{
    public interface IWindowFactory<TView, TViewModel>
    {
        TView Create(TViewModel model);
    }
}
