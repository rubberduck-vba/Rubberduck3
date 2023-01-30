using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Model;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using StreamJsonRpc;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Abstract
{
    public interface ITextDocumentEditorProxy
    {
        /// <summary>
        /// The <strong>Hover</strong> request is sent from the client to the server to request hover information at a given text document position.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.Hover), LspCompliant]
        Task<Hover> HoverAsync(HoverParams parameters);

        /// <summary>
        /// The <strong>Formatting</strong> request is sent from the client to the server to format a whole document.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.Formatting), LspCompliant]
        Task<TextEdit[]> FormattingAsync(DocumentFormattingParams parameters);

        /// <summary>
        /// The <strong>Completion</strong> request is sent from the client to the server to compute completion items at a given cursor position.
        /// </summary>
        /// <remarks>
        /// Completion items are presented in the IntelliSense user interface. 
        /// If computing full completion items is expensive, servers can additionally provide a handler for a subsequent ‘completionItem/resolve’ request.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.Completion), LspCompliant]
        Task<CompletionList> CompletionAsync(CompletionParams parameters);

        /// <summary>
        /// The <strong>SignatureHelp</strong> request is sent from the client to the server to request signature information at a given cursor position.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.SignatureHelp), LspCompliant]
        Task<SignatureHelp> SignatureHelpAsync(SignatureHelpParams parameters);

        /// <summary>
        /// The <strong>ColorPresentation</strong> request is sent from the client to the server to obtain a list of presentations for a color value at a given location.
        /// </summary>
        /// <remarks>
        /// Clients can use the result to
        /// <list type="bullet">
        /// <item>modify a color reference.</item>
        /// <item>show in a color picker and let users pick one of the presentations</item>
        /// </list>
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.ColorPresentation), LspCompliant]
        Task<ColorPresentation[]> ColorPresentationAsync(ColorPresentationParams parameters);

        /// <summary>
        /// The <strong>DocumentColor</strong> request is sent from the client to the server to list all color references found in a given text document. 
        /// Along with the range, a color value in RGB is returned.
        /// </summary>
        /// <remarks>
        /// Clients can use the result to decorate color references in an editor. For example:
        /// <list type="bullet">
        /// <item>Color boxes showing the actual color next to the reference</item>
        /// <item>Show a color picker when a color reference is edited</item>
        /// </list>
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.Color), LspCompliant]
        Task<ColorInformation[]> DocumentColorAsync(DocumentColorParams parameters);

        /// <summary>
        /// The <strong>DocumentHighlieht</strong> request is sent from the client to the server to resolve a document highlights for a given text document position.
        /// </summary>
        /// <remarks>
        /// For programming languages this usually highlights all references to the symbol scoped to this file.
        /// Use <c>textDocument/references</c> instead for identifier references.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.Highlights), LspCompliant]
        Task<DocumentHighlight[]> DocumentHighlightsAsync(DocumentHighlightParams parameters);

        /// <summary>
        /// The <strong>FoldingRanges</strong> request is sent from the client to the server to return all folding ranges found in a given text document.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.FoldingRanges), LspCompliant]
        Task<FoldingRange[]> FoldingRangesAsync(FoldingRangeParams parameters);

        /// <summary>
        /// The <strong>InlayHints</strong> request is sent from the client to the server to compute inlay hints for a given <c>(TextDocument,Range)</c> tuple that may be rendered in the editor in place with other text.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.InlayHints), LspCompliant]
        Task<InlayHint[]> InlayHintAsync(InlayHintParams parameters);

        /// <summary>
        /// The <strong>LinkedEditingRanges</strong> request is sent from the client to the server to return for a given position in a document the range of the symbol at the position and all ranges that have the same content.
        /// </summary>
        /// <remarks>
        /// Optionally a word pattern can be returned to describe valid contents. 
        /// A rename to one of the ranges can be applied to all other ranges if the new content is valid. 
        /// If no result-specific word pattern is provided, the word pattern from the client’s language configuration is used.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.LinkedEditingRanges), LspCompliant]
        Task<LinkedEditingRanges> LinkedEditingRangesAsync(LinkedEditingRangeParams parameters);

        /// <summary>
        /// The document <strong>OnTypeFormatting</strong> request is sent from the client to the server to format parts of the document during typing.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.OnTypeFormatting), LspCompliant]
        Task<TextEdit[]> OnTypeFormattingAsync(DocumentOnTypeFormattingParams parameters);

        /// <summary>
        /// The <strong>RangeFormatting</strong> request is sent from the client to the server to format a given range in a document.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.RangeFormatting), LspCompliant]
        Task<TextEdit[]> RangeFormattingAsync(RangeFormattingParams parameters);

        /// <summary>
        /// The <strong>SelectionRange</strong> request is sent from the client to the server to return suggested <em>selection ranges</em> at an array of given positions.
        /// </summary>
        /// <remarks>
        /// A <em>selection range</em> is a range around the cursor position which the user might be interested in selecting.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Editor.SelectionRange), LspCompliant]
        Task<SelectionRange[]> SelectionRangeAsync(SelectionRangeParams parameters);
    }
}