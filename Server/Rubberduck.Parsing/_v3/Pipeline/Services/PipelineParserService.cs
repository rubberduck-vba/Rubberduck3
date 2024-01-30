using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.Parsing.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.Parsing._v3.Pipeline;

public class PipelineParserService
{
    private readonly RubberduckSettingsProvider _settingsProvider;
    private readonly IParser<string> _parser;
    private readonly IResolverService _resolver;

    public PipelineParserService(RubberduckSettingsProvider settingsProvider, IParser<string> parser, IResolverService resolver)
    {
        _settingsProvider = settingsProvider;
        _parser = parser;
        _resolver = resolver;
    }

    public PipelineParseResult ParseDocument(DocumentState state, CancellationToken token)
    {
        var settings = _settingsProvider.Settings.EditorSettings.CodeFoldingSettings;
        var foldingsListener = new VBFoldingListener(settings);
        
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
