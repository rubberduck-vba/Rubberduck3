using Rubberduck.RPC.Platform.Protocol;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;

namespace Rubberduck.InternalApi.RPC
{
    public class ProgressNotification : ParameterizedNotificationMessage
    {
        public ProgressNotification(string progressToken, int value)
        {
            Method = "$/progress";
            Params = new ProgressParams { Token = progressToken, Value = value };
        }
    }
}
