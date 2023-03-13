namespace Rubberduck.ServerPlatform
{
    public interface IServerApplication
    {
        Task StartAsync(CancellationToken token);
    }
}