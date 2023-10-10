namespace Rubberduck.InternalApi.Model.Abstract
{
    public interface IStatusUpdate
    {
        string Status { get; }
        void UpdateStatus(string status);
    }
}
