using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.Parsing.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.InternalApi.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace Rubberduck.Parsing._v3.Pipeline;

public class PipelineParserService : ServiceBase
{
    private readonly RubberduckSettingsProvider _settingsProvider;
    private readonly IParser<string> _parser;
    private readonly IResolverService _resolver;
    private readonly ISyntaxErrorMessageService _messageService;
    
    public PipelineParserService(ILogger<PipelineParserService> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance, 
        IParser<string> parser, 
        ISyntaxErrorMessageService messageService, 
        IResolverService resolver) 
        : base(logger, settingsProvider, performance)
    {
        _settingsProvider = settingsProvider;
        _parser = parser;
        _resolver = resolver;
        _messageService = messageService;
    }

    public PipelineParseResult ParseDocument(DocumentState state, CancellationToken token)
    {
        var settings = _settingsProvider.Settings.EditorSettings.CodeFoldingSettings;
        var foldingsListener = new VBFoldingListener(settings);
        var sllErrorListener = new ReportingSyntaxErrorListener(state.Uri, _messageService);
        
        token.ThrowIfCancellationRequested();
        var parseResult = _parser.Parse(state.Uri, state.Text, token, parseListeners: [foldingsListener])
                ?? throw new InvalidOperationException("ParserResult was unexpectedly null.");

        var foldings = foldingsListener.Foldings.ToArray();
        LogDebug($"VBFoldingListener: found {foldings.Length} foldings");

        return new()
        {
            Foldings = foldings,
            ParseResult = parseResult,
            Uri = state.Uri
        };
    }
}
