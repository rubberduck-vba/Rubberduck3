using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.DataServer.Response;

namespace Rubberduck.RPC.Proxy.Controllers
{
    public interface IConsoleController
    {
        /// <summary>
        /// Gets the status of this server.
        /// </summary>
        ConsoleStatusResult Status();

        /// <summary>
        /// Sets the console configuration options for this server.
        /// </summary>
        ConsoleStatusResult SetOptions(ServerConsoleOptions parameters);

        /// <summary>
        /// Notifies the client that console options have changed.
        /// </summary>
        ConsoleStatusResult OptionsChanged();
    }
}
