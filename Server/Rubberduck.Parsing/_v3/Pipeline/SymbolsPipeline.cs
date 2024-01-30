using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using MSXML;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline;

/// <summary>
/// A pipeline that produces and broadcasts all symbols in a given <c>ParserResult</c>.
/// </summary>
public class SymbolsPipeline : ParserPipeline<PipelineParseResult, DocumentParserState>
{
    private readonly DocumentContentStore _contentStore;
    private readonly PipelineParseTreeSymbolsService _service;

    public SymbolsPipeline(ILogger<WorkspaceParserPipeline> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance,
        DocumentContentStore contentStore,
        PipelineParseTreeSymbolsService service) 
        : base(logger, settingsProvider, performance)
    {
        _contentStore = contentStore;
        _service = service;
    }

    private TransformBlock<PipelineParseResult, IParseTree> AcquireSyntaxTreeBlock { get; set; } = null!;
    private TransformBlock<IParseTree, Symbol> AcquireMemberSymbolsBlock { get; set; } = null!;
    private BroadcastBlock<Symbol> BroadcastMemberSymbolsBlock { get; set; } = null!;
    private ActionBlock<Symbol> UpdateDocumentStateSymbolsBlock { get; set; } = null!;
    private TransformBlock<IParseTree, Symbol> AcquireSymbolsBlock { get; set; } = null!;

    private BroadcastBlock<Symbol> BroadcastDocumentSymbolsBlock { get; set; } = null!;

    protected override (ITargetBlock<PipelineParseResult> inputBlock, Task completion) DefinePipelineBlocks()
    {
        AcquireSyntaxTreeBlock = new(AcquireSyntaxTree, ExecutionOptions);
        AcquireMemberSymbolsBlock = new(AcquireMemberSymbols, ExecutionOptions);
        BroadcastMemberSymbolsBlock = new(BroadcastMemberSymbols, ExecutionOptions);
        AcquireSymbolsBlock = new(AcquireDeclarationSymbols, ExecutionOptions);
        UpdateDocumentStateSymbolsBlock = new(UpdateDocumentStateSymbols, ExecutionOptions);

        Link(AcquireSyntaxTreeBlock, AcquireMemberSymbolsBlock, WithCompletionPropagation);
        Link(AcquireMemberSymbolsBlock, BroadcastMemberSymbolsBlock, WithCompletionPropagation);
        Link(BroadcastMemberSymbolsBlock, UpdateDocumentStateSymbolsBlock, WithCompletionPropagation);

        return (AcquireSyntaxTreeBlock, BroadcastDocumentSymbolsBlock.Completion);
    }

    protected override void SetInitialState(PipelineParseResult input) 
    {
        var uri = input.Uri;
        State = new DocumentParserState(_contentStore.GetContent(uri));
    }

    private IParseTree AcquireSyntaxTree(PipelineParseResult input) => 
        RunTransformBlock(AcquireSyntaxTreeBlock, input, e =>
        {
            State = State.WithParseTree(e.ParseResult.Tree);
            return e.ParseResult.Tree;
        });

    private Symbol AcquireMemberSymbols(IParseTree syntaxTree) =>
        RunTransformBlock(AcquireMemberSymbolsBlock, syntaxTree, e => _service.DiscoverMemberSymbols(e, State.Uri));

    private Symbol BroadcastMemberSymbols(Symbol symbol) =>
        RunTransformBlock(BroadcastMemberSymbolsBlock, symbol, e => e);

    private Symbol AcquireDeclarationSymbols(IParseTree syntaxTree) => 
        RunTransformBlock(AcquireSymbolsBlock, syntaxTree, e => _service.DiscoverDeclarationSymbols(e, State.Uri));

    private void UpdateDocumentStateSymbols(Symbol symbol) =>
        RunActionBlock(UpdateDocumentStateSymbolsBlock, symbol, e => State = State.WithHierarchicalSymbols(e));
}
