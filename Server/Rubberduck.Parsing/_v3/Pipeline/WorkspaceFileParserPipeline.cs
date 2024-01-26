using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Listeners;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A pipeline that produces and broadcasts all symbols in a given <c>ParserResult</c>.
/// </summary>
public class SymbolsPipeline : ParserPipeline<ParserResult, DocumentState>
{
    private readonly DocumentContentStore _contentStore;

    public SymbolsPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
    }

    private TransformBlock<ParserResult, IParseTree> AcquireParseTreeBlock { get; set; }
    private TransformBlock<IParseTree, Symbol[]> AcquireSymbolsBlock { get; set; }

    protected override (ITargetBlock<ParserResult> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireParseTreeBlock = new(AcquireParseTree, ExecutionOptions);
        AcquireSymbolsBlock = new(AcquireSymbols, ExecutionOptions);

        return (AcquireParseTreeBlock, AcquireParseTreeBlock.Completion);
    }

    protected override void SetInitialState(ParserResult input) 
    {
        var uri = input.Uri;
        State = _contentStore.GetContent(uri);
    }

    private IParseTree AcquireParseTree(ParserResult input)
    {
        IParseTree? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = input.ParseResult.Tree;

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(AcquireParseTreeBlock!, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private Symbol[] AcquireSymbols(IParseTree syntaxTree)
    {
        List<Symbol> result = [];
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(AcquireSymbolsBlock!, exception);
        }

        return result.ToArray();
    }
}


/// <summary>
/// A pipeline that broadcasts parse results for a given <c>DocumentState</c>.
/// </summary>
public class WorkspaceFileParserPipeline : ParserPipeline<WorkspaceFileUri, DocumentState>
{
    private readonly DocumentContentStore _contentStore;
    private readonly IParser<string> _parser;

    public WorkspaceFileParserPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        IParser<string> parser) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _parser = parser;
    }

    private TransformBlock<WorkspaceUri, DocumentState>? AcquireDocumentStateBlock { get; set; }
    private TransformBlock<DocumentState, ParserResult>? ParseDocumentTextBlock { get; set; }
    private BroadcastBlock<ParserResult>? BroadcastParseResultsBlock { get; set; }
    private ActionBlock<ParserResult>? UpdateDocumentStateBlock { get; set; }

    protected override (ITargetBlock<WorkspaceFileUri> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireDocumentStateBlock = new(AcquireDocumentState, ExecutionOptions);
        ParseDocumentTextBlock = new(ParseDocumentText, ExecutionOptions);
        UpdateDocumentStateBlock = new(UpdateDocumentState, ExecutionOptions);
        BroadcastParseResultsBlock = new(BroadcastParserResult, ExecutionOptions);

        Link(AcquireDocumentStateBlock, ParseDocumentTextBlock, WithCompletionPropagation);
        Link(ParseDocumentTextBlock, UpdateDocumentStateBlock, WithCompletionPropagation);
        Link(BroadcastParseResultsBlock, UpdateDocumentStateBlock, WithCompletionPropagation);
        
        return (AcquireDocumentStateBlock, UpdateDocumentStateBlock.Completion);
    }

    protected override void SetInitialState(WorkspaceFileUri input) { }

    private DocumentState AcquireDocumentState(WorkspaceUri uri)
    {
        uri = uri as WorkspaceFileUri 
            ?? throw new ArgumentException($"Expected a {nameof(WorkspaceFileUri)}.");

        DocumentState? result = null;
        if (!TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = _contentStore.GetContent(uri)
                ?? throw new InvalidOperationException("Document state was not found in the content store.");

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(AcquireDocumentStateBlock!, exception);
        }

        State = result;
        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private ParserResult ParseDocumentText(DocumentState documentState)
    {
        ParserResult? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            // FIXME abstraction level feels wrong

            var foldingsListener = new VBFoldingListener(null!); // TODO implement code folding settings

            var parserResult = _parser.Parse(documentState.Uri, documentState.Text, Token, parseListeners: [foldingsListener])
                ?? throw new InvalidOperationException("ParserResult was unexpectedly null.");

            result = new()
            {
                Foldings = foldingsListener.Foldings,
                ParseResult = parserResult,
                Uri = documentState.Uri
            };

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(ParseDocumentTextBlock!, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private void UpdateDocumentState(ParserResult parserResult)
    {
        DocumentState? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = State.WithFoldings(parserResult.Foldings) 
                ?? throw new InvalidOperationException("Document state was unexpectedly null.");

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(UpdateDocumentStateBlock!, exception);
        }

        State = result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private ParserResult BroadcastParserResult(ParserResult parserResult)
    {
        ParserResult? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = parserResult;

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(BroadcastParseResultsBlock!, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }
}
