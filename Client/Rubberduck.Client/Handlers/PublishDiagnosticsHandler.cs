using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace Rubberduck.Client.Handlers
{
    public class PublishDiagnosticsHandler : PublishDiagnosticsHandlerBase
    {
        public override async Task<Unit> Handle(PublishDiagnosticsParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO
            /*
             * Diagnostics notifications are sent from the server to the client to signal results of validation runs.
             * Diagnostics are “owned” by the server so it is the server’s responsibility to clear them if necessary. 
             * The following rule is used for VS Code servers that generate diagnostics:
             * 
             *  > if a language is single file only (for example HTML) then diagnostics are cleared by the server when the file is closed. 
             *    Please note that open / close events don’t necessarily reflect what the user sees in the user interface. 
             *    These events are ownership events. So with the current version of the specification it is possible that problems are not cleared 
             *    although the file is not visible in the user interface since the client has not closed the file yet.
             *    
             *  > if a language has a project system (for example C#) diagnostics are not cleared when a file closes. 
             *    When a project is opened all diagnostics for all files are recomputed (or read from a cache).
             *    When a file changes it is the server’s responsibility to re-compute diagnostics and push them to the client. 
             *    If the computed set is empty it has to push the empty array to clear former diagnostics. 
             *    Newly pushed diagnostics always replace previously pushed diagnostics.
             *    
             *    There is no merging that happens on the client side.
            */

            // request contains the diagnostics for a document identified by its Uri and a version number.
            // if the versions don't match, diagnostics should be considered stale.
            // otherwise, spawn them squiggles!

            return await Task.FromResult(Unit.Value);
        }
    }
}
