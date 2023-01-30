using NLog;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.UI.Command;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class ResumeTraceCommand : CommandBase
    {
        private readonly IServerConsoleService<ServerConsoleOptions> _console;

        public ResumeTraceCommand(IServerConsoleService<ServerConsoleOptions> console) 
            : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(ServerLogLevel.Info, $"Executing {nameof(ResumeTraceCommand)}..."); // normally not logged
            _console.Configuration.IsEnabled = true;
        }
    }
}
