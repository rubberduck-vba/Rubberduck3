using NLog;
using Rubberduck.InternalApi.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.UI.Command;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class SetTraceCommand : CommandBase
    {
        private readonly IJsonRpcConsole _console;

        public SetTraceCommand(IJsonRpcConsole console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(LogLevel.Info, $"Executing {nameof(SetTraceCommand)}...", verbose: $"Parameter: '{parameter}'");

            if (parameter is string traceValue)
            {
                switch (traceValue)
                {
                    case Constants.TraceValue.Off:
                    case Constants.TraceValue.Messages:
                    case Constants.TraceValue.Verbose:
                        _console.Trace = traceValue;
                        break;
                    default:
                        _console.Log(LogLevel.Debug, $"Parameter value '{traceValue}' is not valid.", verbose: $"Trace will not be changed (value: '{_console.Trace}').");
                        break;
                }
            }
        }
    }
}
