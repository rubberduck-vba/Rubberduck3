using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Window.Commands.Parameters;
using System;

namespace Rubberduck.Server.LSP.Proxies
{
    public interface IWindowClientProxy
    {
        [LspCompliant(JsonRpcMethods.ClientProxyRequests.LSP.CancelWorkDoneProgress)]
        event EventHandler<WorkDoneProgressCancelParams> CancelWorkDoneProgress;

        [LspCompliant(JsonRpcMethods.ClientProxyRequests.LSP.CreateWorkDoneProgress)]
        event EventHandler<WorkDoneProgressCreateParams> CreateWorkDoneProgress;

        [LspCompliant(JsonRpcMethods.ClientProxyRequests.LSP.LogMessage)]
        event EventHandler<LogMessageParams> LogMessage;

        [LspCompliant(JsonRpcMethods.ClientProxyRequests.LSP.ShowDocument)]
        event EventHandler<ShowDocumentParams> ShowDocument;

        [LspCompliant(JsonRpcMethods.ClientProxyRequests.LSP.ShowMessage)]
        event EventHandler<ShowMessageParams> ShowMessage;
    }
}