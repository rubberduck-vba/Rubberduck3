using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Client;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;

namespace Rubberduck.RPC.Proxy.LocalDbServer
{
    [JsonRpcSource]
    public class LocalDbServerConsoleProxyClient : JsonRpcClientSideServerProxyService<IServerConsoleProxy>, IServerConsoleProxyClient
    {
        public LocalDbServerConsoleProxyClient(IRpcStreamFactory<NamedPipeClientStream> rpcStreamFactory) 
            : base(rpcStreamFactory)
        {
        }

        public event EventHandler<SetTraceParams> SetTrace;
        public event EventHandler StopTrace;
        public event EventHandler ResumeTrace;

        public async Task LogMessageAsync(LogMessageParams parameter)
        {
            System.Diagnostics.Debug.WriteLine(parameter);
            await Task.Yield();
        }

        public async Task OnResumeTraceAsync() => ResumeTrace?.Invoke(this, EventArgs.Empty);

        public async Task OnSetTraceAsync(SetTraceParams parameter) => SetTrace?.Invoke(this, parameter);

        public async Task OnStopTraceAsync() => StopTrace?.Invoke(this, EventArgs.Empty);
    }
}
