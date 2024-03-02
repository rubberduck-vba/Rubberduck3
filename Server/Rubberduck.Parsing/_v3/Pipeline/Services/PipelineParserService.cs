using Rubberduck.Parsing._v3.Pipeline.Services;
using Rubberduck.Parsing.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Parsing._v3.Pipeline;

public class PipelineParserService : ServiceBase
{
    private readonly IParser<string> _parser;
    
    public PipelineParserService(ILogger<PipelineParserService> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance, 
        IParser<string> parser) 
        : base(logger, settingsProvider, performance)
    {
        _parser = parser;
    }

    public PipelineParseResult ParseDocument(DocumentState state, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var parseResult = _parser.Parse(state.Uri, state.Text, token, ParserMode.FallBackSllToLl)
                ?? throw new InvalidOperationException("ParserResult was unexpectedly null.");

        return new()
        {
            ParseResult = parseResult, 
            Uri = state.Uri
        };
    }
}
