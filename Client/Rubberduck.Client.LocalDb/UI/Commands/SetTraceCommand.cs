﻿using NLog;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.UI.Command;
using System;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class SetTraceCommand : CommandBase
    {
        private readonly IServerConsoleProxyClient _console;

        public SetTraceCommand(IServerConsoleProxyClient console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            //_console.LogTraceAsync(ServerLogLevel.Info, $"Executing {nameof(SetTraceCommand)}...", verbose: $"Parameter: '{parameter}'");

            if (parameter is string traceValue)
            {
                switch (traceValue)
                {
                    case Constants.TraceValue.Off:
                    case Constants.TraceValue.Messages:
                    case Constants.TraceValue.Verbose:
                        if (Enum.TryParse<Constants.Console.VerbosityOptions.AsStringEnum>(traceValue, true, out var verbosity))
                        {
                            _console.OnSetTraceAsync(new SetTraceParams { Value = verbosity });
                        }
                        break;
                    default:
                        //_console.LogTraceAsync(ServerLogLevel.Debug, $"Parameter value '{traceValue}' is not valid.", verbose: $"Trace will not be changed (value: '{_console.Configuration.ConsoleOptions.Trace}').");
                        break;
                }
            }
        }
    }
}
