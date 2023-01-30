using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using static Nerdbank.Streams.MultiplexingStream;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    public interface IBuilder<T> where T : class, new()
    {
        /// <summary>
        /// Returns the built <see cref="T"/> instance.
        /// </summary>
        T Build();
    }

    public interface IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> : IBuilder<TServerCommands>
        where TServerCommands : class, new()
        where TServerProxy : class, IServerProxyService<TOptions>
        where TOptions : class, new()
    {
        IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> WithServerInfoCommand(TServerProxy server);
        IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> WithInitializeCommand(TServerProxy server);
        IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> WithSetConfigurationOptionsCommand(IServerProxyClient server);
        IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> WithConnectClientCommand(TServerProxy server);
        IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> WithDisconnectClientCommand(TServerProxy server);
        IServerCommandsBuilder<TServerCommands, TServerProxy, TOptions> WithExitCommand(TServerProxy server);
    }
}