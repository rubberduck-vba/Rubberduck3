namespace Rubberduck.InternalApi.Settings
{
    public interface IProcessStartInfoArgumentProvider
    {
        public ServerTraceLevel TraceLevel { get; init; }

        string Path { get; }
        string ToProcessStartInfoArguments(long clientProcessId);
    }
}
