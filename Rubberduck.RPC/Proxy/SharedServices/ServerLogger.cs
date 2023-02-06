using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices
{
    public class ServerLogger<TOptions> : IServerLogger, IInternalServerLogger 
        where TOptions : SharedServerCapabilities
    {
        private readonly IServerConsoleProxy<TOptions> _service;

        public ServerLogger(IServerConsoleProxy<TOptions> service)
        {
            _service = service;
        }

        public void OnError(Exception exception) => Log(ServerLogLevel.Error, exception.Message, exception.StackTrace);
        public void OnWarning(string message, string verbose = null) => Log(ServerLogLevel.Warn, message, verbose);
        public void OnInfo(string message, string verbose = null) => Log(ServerLogLevel.Info, message, verbose);
        public void OnDebug(string message, string verbose = null) => Log(ServerLogLevel.Debug, message, verbose);
        public void OnTrace(string message, string verbose = null) => Log(ServerLogLevel.Trace, message, verbose);

        public void Log(ServerLogLevel level, string message, string verbose = null) => _service.LogTraceAsync(level, message, verbose);
    }
}