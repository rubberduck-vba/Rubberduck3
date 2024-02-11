namespace Rubberduck.InternalApi.Settings;

public interface IProcessStartInfoArgumentProvider
{
    public MessageTraceLevel ServerTraceLevel { get; }

    string ServerExecutablePath { get; }
    string ToProcessStartInfoArguments(long clientProcessId);
}
