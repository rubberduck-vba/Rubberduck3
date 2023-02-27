using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.RPC.Platform.Model
{
    public class RpcClientInfo
    {
        public RpcClientInfo(ClientInfo rpcClient, int processId)
        {
            Name = rpcClient.Name;
            Version = rpcClient.Version;

            ProcessId = processId;
        }

        public string Name { get; set; }
        public string Version { get; set; }

        public int ProcessId { get; set; }
    }
}
