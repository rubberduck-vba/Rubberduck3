using NLog;
using Rubberduck.InternalApi.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.UI.Command;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class ShutdownCommandParameter
    {
        public int ExitCode { get; set; }
    }

    public class ShutdownCommand : CommandBase
    {
        private IJsonRpcServer _server;
        private IEnvironmentService _environment;

        public ShutdownCommand(IJsonRpcServer server, IEnvironmentService environment) : base(LogManager.GetCurrentClassLogger()) 
        {
            _server = server;
            _environment = environment;
        }

        protected override void OnExecute(object parameter)
        {
            _server.Console.Log(LogLevel.Info, $"Executing {nameof(ShutdownCommand)}...");

            var exitCode = 0;
            try
            {
                _server.Stop();

                if (parameter is ShutdownCommandParameter shutdownParam)
                {
                    exitCode = shutdownParam.ExitCode;
                }
            }
            catch (Exception exception)
            {
                _server.Console.Log(exception, LogLevel.Error, exception.Message, exception.ToString());
                exitCode = 1;
            }

            if (_server.IsInteractive)
            {
                _server.Console.Log(LogLevel.Info, "Exiting...", verbose: $"Exit code: {exitCode}. Interactive mode: process terminating in {_server.ExitDelay.TotalMilliseconds:N0} milliseconds.");
                Task.Delay(_server.ExitDelay).ContinueWith(t => _environment.Exit(exitCode));
            }
            else
            {
                _server.Console.Log(LogLevel.Info, "Exiting...", verbose: $"Exit code: {exitCode}.");
                _environment.Exit(exitCode);
            }
        }
    }
}
