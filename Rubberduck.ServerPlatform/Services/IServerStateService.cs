using Rubberduck.ServerPlatform.Model;

namespace Rubberduck.ServerPlatform.Services
{
    public interface IServerStateService
    {
        ServerProcessInfo GetServerProcessInfo();

        bool AddClient(ClientProcessInfo client);
        bool RemoveClient(ClientProcessInfo client);
    }
}
