using NLog;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.UI.Command;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class ResumeTraceCommand : CommandBase
    {
        private readonly IServerConsoleProxy<SharedServerCapabilities> _console;

        public ResumeTraceCommand(IServerConsoleProxy<SharedServerCapabilities> console) 
            : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            //_console.LogTraceAsync(ServerLogLevel.Info, $"Executing {nameof(ResumeTraceCommand)}..."); // normally not logged
            var config = _console.GetConfigurationAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            config.IsEnabled = true;
        }
    }
}
