using NLog;
using Rubberduck.RPC.Platform;
using Rubberduck.UI.Command;

namespace Rubberduck.DataServer.UI.Commands
{
    public class ResumeTraceCommand : CommandBase
    {
        private readonly IJsonRpcConsole _console;

        public ResumeTraceCommand(IJsonRpcConsole console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(LogLevel.Info, $"Executing {nameof(ResumeTraceCommand)}..."); // normally not logged
            _console.IsEnabled = true;
        }
    }
}
