namespace Rubberduck.UI.Abstract
{
    public interface IStatusUpdate
    {
        string CurrentStatus { get; }
        void UpdateStatus(string status);
    }
}
