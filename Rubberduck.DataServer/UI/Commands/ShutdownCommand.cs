using NLog;
using Rubberduck.InternalApi.RPC;
using Rubberduck.RPC.Proxy;
using Rubberduck.UI.Command;
using System;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class ShutdownCommandParameter
    {
        public int ExitCode { get; set; }
    }

    public class ShutdownCommand : CommandBase
    {
        private ILocalDbServer _proxy;
        private IEnvironmentService _environment;

        public ShutdownCommand(ILocalDbServer proxy, IEnvironmentService environment) : base(LogManager.GetCurrentClassLogger()) 
        {
            _proxy = proxy;
            _environment = environment;
        }

        protected override void OnExecute(object parameter)
        {
            _proxy.Console.Log(LogLevel.Info, $"Executing {nameof(ShutdownCommand)}...");

            var exitCode = 0;
            try
            {
                _proxy.Shutdown();

                if (parameter is ShutdownCommandParameter shutdownParam)
                {
                    exitCode = shutdownParam.ExitCode;
                }
            }
            catch (Exception exception)
            {
                _proxy.Console.Log(exception, LogLevel.Error, exception.Message, exception.ToString());
                exitCode = 1;
            }

            _proxy.Console.Log(LogLevel.Info, "Exiting...", verbose: $"Exit code: {exitCode}.");
            _environment.Exit(exitCode);
        }
    }
}
