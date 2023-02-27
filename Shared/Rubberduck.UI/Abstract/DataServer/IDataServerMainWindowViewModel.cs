namespace Rubberduck.UI.Abstract
{
    public interface IDataServerMainWindowViewModel
    {
        IConsoleViewModel Console { get; }
        IServerStatusViewModel Status { get; }
    }
}
