namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface ISafeEventedComWrapper : ISafeComWrapper
    {
        void AttachEvents();
        void DetachEvents();
    }
}
