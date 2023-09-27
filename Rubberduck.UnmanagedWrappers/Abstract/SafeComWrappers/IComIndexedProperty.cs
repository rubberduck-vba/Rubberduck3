namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface IComIndexedProperty<out TItem>
    {
        TItem this[object index] { get; }
    }
}