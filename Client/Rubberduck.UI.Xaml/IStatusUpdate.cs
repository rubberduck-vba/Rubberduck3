namespace Rubberduck.UI.Xaml
{
    public interface IStatusUpdate
    {
        string Status { get; }
        void UpdateStatus(string status);
    }
}
