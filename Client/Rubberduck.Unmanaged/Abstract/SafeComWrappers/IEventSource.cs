namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface IEventSource<out TEventSource>
    {
        TEventSource EventSource { get; }
    }
}
