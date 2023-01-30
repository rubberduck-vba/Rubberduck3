using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices
{
    public class ServerLogger : IServerLogger, IInternalServerLogger 
    {
        private readonly Action<Exception> _onErrorAction;
        private readonly Action<ServerLogLevel, string, string> _onLogMessageAction;

        public ServerLogger(Action<Exception> onErrorAction, Action<ServerLogLevel, string, string> onLogMessageAction)
        {
            _onErrorAction = onErrorAction;
            _onLogMessageAction = onLogMessageAction;
        }

        public void OnError(Exception exception) => _onErrorAction?.Invoke(exception);
        public void OnWarning(string message, string verbose = null) => _onLogMessageAction.Invoke(ServerLogLevel.Warn, message, verbose);
        public void OnInfo(string message, string verbose = null) => _onLogMessageAction.Invoke(ServerLogLevel.Info, message, verbose);
        public void OnDebug(string message, string verbose = null) => _onLogMessageAction.Invoke(ServerLogLevel.Debug, message, verbose);
        public void OnTrace(string message, string verbose = null) => _onLogMessageAction.Invoke(ServerLogLevel.Trace, message, verbose);

        public void Log(ServerLogLevel level, string message, string verbose = null) => _onLogMessageAction(level, message, verbose);
    }
}