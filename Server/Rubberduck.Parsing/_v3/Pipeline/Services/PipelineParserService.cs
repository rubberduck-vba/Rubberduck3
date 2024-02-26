using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.Parsing.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.Parsing.Exceptions;

namespace Rubberduck.Parsing._v3.Pipeline;

public class PipelineParserService
{
    private readonly RubberduckSettingsProvider _settingsProvider;
    private readonly IParser<string> _parser;
    private readonly IResolverService _resolver;
    private readonly ISyntaxErrorMessageService _messageService;

    public PipelineParserService(RubberduckSettingsProvider settingsProvider, IParser<string> parser, ISyntaxErrorMessageService messageService, IResolverService resolver)
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

        var result = _parser.Parse(state.Uri, state.Text, token, parseListeners: [foldingsListener])
            ?? throw new InvalidOperationException("ParserResult was unexpectedly null.");

        return new()
        {
            Foldings = foldingsListener.Foldings,
            ParseResult = result,
            Uri = state.Uri
        };
    }
}
