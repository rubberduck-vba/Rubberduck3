using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace
{
    /// <summary>
    /// A notification sent from the Rubberduck Editor to the VBIDE-hosted addin client,
    /// to have the VBE enter a debug session (or resume a paused one).
    /// </summary>
    /// <remarks>
    /// Server should expect a subsequent <c>DidBeginDebugSession</c> notification.
    /// </remarks>
    public record class BeginDebugSessionParams : INotification
    {
        public const string RpcNotificationName = "vbe/beginDebug";
    }

    /// <summary>
    /// A notification sent from the VBIDE-hosted addin client to the Rubberduck editor,
    /// to signal that the VBE host has entered 'run' state (from either 'edit' or 'break' states).
    /// </summary>
    /// <remarks>
    /// This notification may be sent without a prior <c>BeginDebugSession</c> having been issued.
    /// </remarks>
    public record class DidBeginDebugSessionParams : INotification
    {
        public const string RpcNotificationName = "vbe/didBeginDebug";
    }

    /// <summary>
    /// A notification sent from the Rubberduck editor to the VBIDE-hosted addin client,
    /// to signal that the editor process wants to resume editing.
    /// </summary>
    /// <remarks>
    /// Server/editor should still await <c>DidEndDebugSession</c> to allow editing.
    /// </remarks>
    public record class EndDebugSessionParams : INotification
    {
        public const string RpcNotificationName = "vbe/endDebug";
    }

    /// <summary>
    /// A notification sent from the Rubberduck editor to the VBIDE-hosted addin client,
    /// to signal that the editor process wants to resume editing.
    /// </summary>
    /// <remarks>
    /// Server should expect a subsequent <c>DidBreakDebugSessionParams</c> notification.
    /// </remarks>
    public record class PauseDebugSessionParams : INotification
    {
        public const string RpcNotificationName = "vbe/pauseDebug";
    }

    /// <summary>
    /// A notification sent from the VBIDE-hosted addin client to the Rubberduck editor,
    /// to signal that the VBE host has entered 'break' state; debug session is still active, but edits could be made in the VBE.
    /// </summary>
    public record class DidPauseDebugSessionParams : INotification
    {
        public const string RpcNotificationName = "vbe/didPauseDebug";
    }

    /// <summary>
    /// A notification sent from the VBIDE-hosted addin client to the Rubberduck editor,
    /// to signal that the VBE host has resumed 'edit' state and the editor can safely take control of the source code.
    /// </summary>
    /// <remarks>
    /// Server should re-initialize the workspace from the current VBE content, and prompt to load or discard any external changes (requires filewatcher capabilities enabled).
    /// </remarks>
    public record class DidEndDebugSessionParams : INotification
    {
        public const string RpcNotificationName = "vbe/didEndDebug";
    }

    /// <summary>
    /// A notification sent from the Rubberduck Editor to the VBIDE-hosted addin client,
    /// to synchronize the workspace contents into the VBE for a specific project.
    /// </summary>
    /// <remarks>
    /// Server should expect a subsequent <c>DidSynchronizeWorkspaceNotification</c> notification from the client.
    /// </remarks>
    public record class SynchronizeWorkspaceParams : INotification
    {
        public const string RpcNotificationName = "vbe/synchronize";
    }

    public record class DidSynchronizeWorkspaceResult : INotification
    {
        /// <summary>
        /// The files that were successfully synchronized/loaded into the VBE.
        /// </summary>
        public Uri[] FilesLoaded { get; init; }
        /// <summary>
        /// Files that failed to update or otherwise could not be loaded into the VBE.
        /// </summary>
        public Uri[] Errors { get; init; }
    }

    public class AddInClientService
    {
        private readonly ILanguageClient _addin;

        public AddInClientService(ILanguageClient addin)
        {
            _addin = addin;
        }

        public void SynchronizeWorkspace()
        {
            var param = new SynchronizeWorkspaceParams();
            _addin.SendNotification(SynchronizeWorkspaceParams.RpcNotificationName, param);
        }

        public void BeginDebugSession()
        {
            var param = new BeginDebugSessionParams();
            _addin.SendNotification(BeginDebugSessionParams.RpcNotificationName, param);
        }

        public void EndDebugSession()
        {
            var param = new EndDebugSessionParams();
            _addin.SendNotification(EndDebugSessionParams.RpcNotificationName, param);
        }

        public void PauseDebugSession()
        {
            var param = new PauseDebugSessionParams();
            _addin.SendNotification(PauseDebugSessionParams.RpcNotificationName, param);
        }
    }

    /* 
    * The workspace/executeCommand request is sent from the client to the server to trigger command execution on the server. 
    * In most cases the server creates a WorkspaceEdit structure and applies the changes to the workspace 
    * using the request workspace/applyEdit which is sent from the server to the client.
    * https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#workspace_executeCommand
    */

    public class ExecuteCommandHandler : ExecuteCommandHandlerBase
    {
        public async override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(Unit.Value);
        }

        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities)
        {
            return new ExecuteCommandRegistrationOptions
            {
                WorkDoneProgress = true,
                Commands = new Container<string>(/*TODO*/)
            };
        }
    }
}