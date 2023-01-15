using NLog;
using Rubberduck.RPC.Platform;
using Rubberduck.UI.Command;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class PauseTraceCommand : CommandBase
    {
        private readonly IJsonRpcConsole _console;

        public PauseTraceCommand(IJsonRpcConsole console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(LogLevel.Info, $"Executing {nameof(PauseTraceCommand)}...");
            _console.IsEnabled = false;
        }
    }
}
