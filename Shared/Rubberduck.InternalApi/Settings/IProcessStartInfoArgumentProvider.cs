namespace Rubberduck.InternalApi.Settings
{
    public interface IProcessStartInfoArgumentProvider
    {
        public ServerTraceLevel ServerTraceLevel { get; }

        string ServerExecutablePath { get; }
        string ToProcessStartInfoArguments(long clientProcessId);
    }
}
