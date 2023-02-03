using NLog;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.UI.Command;
using System;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class SetTraceCommand : CommandBase
    {
        private readonly IServerConsoleService<SharedServerCapabilities> _console;

        public SetTraceCommand(IServerConsoleService<SharedServerCapabilities> console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(ServerLogLevel.Info, $"Executing {nameof(SetTraceCommand)}...", verbose: $"Parameter: '{parameter}'");

            if (parameter is string traceValue)
            {
                switch (traceValue)
                {
                    case Constants.TraceValue.Off:
                    case Constants.TraceValue.Messages:
                    case Constants.TraceValue.Verbose:
                        if (Enum.TryParse<Constants.Console.VerbosityOptions.AsStringEnum>(traceValue, true, out var verbosity))
                        {
                            _console.Configuration.ConsoleOptions.Trace = verbosity;
                        }
                        break;
                    default:
                        _console.Log(ServerLogLevel.Debug, $"Parameter value '{traceValue}' is not valid.", verbose: $"Trace will not be changed (value: '{_console.Configuration.ConsoleOptions.Trace}').");
                        break;
                }
            }
        }
    }
}
