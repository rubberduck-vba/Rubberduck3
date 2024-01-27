using System.Threading.Tasks;

namespace Rubberduck.InternalApi.ServerPlatform.UpdateServer;

public interface IUpdateClient
{
    Task CheckForUpdatesAsync();
}
