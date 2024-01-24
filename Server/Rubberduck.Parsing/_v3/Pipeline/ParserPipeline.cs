using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Listeners;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

public interface IParserPipeline
{
    /// <summary>
    /// Starts executing the parser pipeline with the provided state.
    /// </summary>
    Task StartAsync(ParserPipelineState state);
    /// <summary>
    /// Gets a task that completes when the first pass is completed.
    /// </summary>
    /// <remarks>
    /// Await in LSP handler to ensure availability of resources.
    /// </remarks>
    Task FirstPassParserResultTask { get; }
}

public record class ParserPipelineState
{
    public ParserPipelineState(WorkspaceUri workspaceRootUri, IOrderedEnumerable<DocumentState> prioritizedState)
    {
        WorkspaceRootUri = workspaceRootUri;
        Documents = new(prioritizedState.ToImmutableSortedDictionary(document => document.Uri, document => document));
    }

    public WorkspaceUri WorkspaceRootUri { get; init; }

    public ConcurrentDictionary<WorkspaceFileUri, DocumentState> Documents { get; init; }
}

public class FirstPassParserResult
{
    public WorkspaceFileUri Uri { get; init; } = null!;
    public ParseResult ParseResult { get; init; } = null!;
    public IEnumerable<FoldingRange> Foldings { get; init; } = [];
}

internal class ParserPipeline : ServiceBase, IParserPipeline, IDisposable
{
    private readonly IWorkspaceStateManager _workspaceManager;
    private readonly DocumentContentStore _contentStore;
    private readonly IParser<string> _parser;

    private readonly CancellationTokenSource _tokenSource;
    private readonly IEnumerable<IDisposable> _disposables;

    public ParserPipeline(ILogger<ParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        IWorkspaceStateManager workspaceManager,
        DocumentContentStore contentStore,
        IParser<string> parser)
        : base(logger, settingsProvider, performance)
    {
        _workspaceManager = workspaceManager;
        _contentStore = contentStore;
        _parser = parser;

        _tokenSource = new();

        var options = new ExecutionDataflowBlockOptions { CancellationToken = _tokenSource.Token, MaxDegreeOfParallelism = 4 };

        AcquireWorkspaceBlock = new(AcquireWorkspaceState, options);
        PrioritizeFilesBlock = new(PrioritizeFiles, options);
        AcquireDocumentBlock = new(AcquireDocumentState, options);
        ParseDocumentBlock = new(ParseDocumentText, options);
        BroadcastFirstPassParseResultBlock = new(result => result);
        UpdateInitialPassDocumentStateBlock = new(UpdateDocumentState, options);
        UpdateWorkspaceTypeSymbolsBlock = new(UpdateWorkspaceTypeSymbols, options);

        _disposables = 
        [
            AcquireWorkspaceBlock.LinkTo(PrioritizeFilesBlock, _withCompletionPropagation),
            PrioritizeFilesBlock.LinkTo(AcquireDocumentBlock, _withCompletionPropagation),
            AcquireDocumentBlock.LinkTo(ParseDocumentBlock, _withCompletionPropagation),
            ParseDocumentBlock.LinkTo(BroadcastFirstPassParseResultBlock, _withCompletionPropagation),
            BroadcastFirstPassParseResultBlock.LinkTo(UpdateInitialPassDocumentStateBlock, _withCompletionPropagation),
            BroadcastFirstPassParseResultBlock.LinkTo(UpdateWorkspaceTypeSymbolsBlock, _withCompletionPropagation),
        ];
    }

    public ParserPipelineState? State { get; private set; }

    public Task StartAsync(ParserPipelineState state)
    {
        if (State != null)
        {
            throw new InvalidOperationException("BUG: This pipeline instance is already in use; a new one should be created every time.");
        }
        State = state ?? throw new ArgumentNullException(nameof(state));

        AcquireWorkspaceBlock.Post(state.WorkspaceRootUri);
        AcquireWorkspaceBlock.Complete();

        return UpdateWorkspaceTypeSymbolsBlock.Completion;
    }

    private static readonly DataflowLinkOptions _withCompletionPropagation = new(){ PropagateCompletion = true };
    
    private TransformBlock<WorkspaceUri, IWorkspaceState> AcquireWorkspaceBlock { get; }
    private TransformManyBlock<IWorkspaceState, WorkspaceFileUri> PrioritizeFilesBlock { get; }
    private TransformBlock<WorkspaceFileUri, DocumentState> AcquireDocumentBlock { get; }
    private TransformBlock<DocumentState, FirstPassParserResult> ParseDocumentBlock { get; }
    private BroadcastBlock<FirstPassParserResult> BroadcastFirstPassParseResultBlock { get; }
    private ActionBlock<FirstPassParserResult> UpdateInitialPassDocumentStateBlock { get; }
    public Task FirstPassParserResultTask => UpdateInitialPassDocumentStateBlock.Completion;

    private TransformBlock<FirstPassParserResult, Symbol[]> UpdateWorkspaceTypeSymbolsBlock { get; }
    

    private void ThrowIfCancellationRequested() => _tokenSource.Token.ThrowIfCancellationRequested();
    private void FaultDataflowBlock(IDataflowBlock block, Exception exception)
    {
        block.Fault(exception);
        _tokenSource?.Cancel();
    }

    private IWorkspaceState AcquireWorkspaceState(WorkspaceUri uri)
    {
        IWorkspaceState? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            
            result = _workspaceManager.GetWorkspace(uri) 
                ?? throw new InvalidOperationException($"Could not find workspace state for URI '{uri}'.");

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(AcquireWorkspaceBlock, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private WorkspaceFileUri[] PrioritizeFiles(IWorkspaceState state)
    {
        WorkspaceFileUri[]? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = state.WorkspaceFiles
                .OrderByDescending(file => file.IsOpened)
                .ThenBy(file => file.Name)
                .Select(file => file.Uri).ToArray();

            if (result.Length == 0)
            {
                throw new InvalidOperationException($"Workspace state has no files to process.");
            }

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(PrioritizeFilesBlock, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private DocumentState AcquireDocumentState(Uri uri)
    {
        DocumentState? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = _contentStore.GetContent(uri)
                ?? throw new InvalidOperationException("Document state was not found in the content store.");

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(AcquireDocumentBlock, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private FirstPassParserResult ParseDocumentText(DocumentState documentState)
    {
        FirstPassParserResult? result = null;
        var foldingsListener = new VBFoldingListener(null!); // TODO implement code folding settings

        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            var parserResult = _parser.Parse(documentState.Uri, documentState.Text, _tokenSource.Token, parseListeners: [foldingsListener])
                ?? throw new InvalidOperationException("Parser returned an unexpected null reference.");

            result = new()
            {
                Foldings = foldingsListener.Foldings,
                ParseResult = parserResult,
                Uri = documentState.Uri
            };

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(ParseDocumentBlock, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    private void UpdateDocumentState(FirstPassParserResult result)
    {
        if (State != null && !TryRunAction(()=>
        {
            ThrowIfCancellationRequested();

            var state = State.Documents[result.Uri].WithFoldings(result.Foldings)
                ?? throw new InvalidOperationException("Document state was unexpectedly null.");

            State.Documents.AddOrUpdate(result.Uri, state, (uri, old) => state);

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(UpdateInitialPassDocumentStateBlock, exception);
        }
    }

    private Symbol[] UpdateWorkspaceTypeSymbols(FirstPassParserResult result)
    {
        Symbol[]? symbols = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            var tree = result.ParseResult.Tree;
            // TODO walk the tree, spawn first pass symbols

        }, out var exception) && exception != null)
        {
            FaultDataflowBlock(UpdateWorkspaceTypeSymbolsBlock, exception);
        }

        return symbols ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    public void Dispose()
    {
        _tokenSource?.Dispose();
        foreach(var disposable in _disposables) 
        { 
            disposable.Dispose(); 
        }
    }
}
