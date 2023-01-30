using NLog;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.UI.Command;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class PauseTraceCommand : CommandBase
    {
        private readonly IServerConsoleService<ServerConsoleOptions> _console;

        public PauseTraceCommand(IServerConsoleService<ServerConsoleOptions> console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(ServerLogLevel.Info, $"Executing {nameof(PauseTraceCommand)}...");
            _console.Configuration.IsEnabled = false;
        }
    }
}
