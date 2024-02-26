using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers;

public class VBAPreprocessorParser : TokenStreamParserBase<VBAConditionalCompilationParser>
{
    public VBAPreprocessorParser(ISyntaxErrorMessageService errorMessageService, ILogger<ITokenStreamParser> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(errorMessageService, logger, settingsProvider, performance)
    {
    }

    protected override VBAConditionalCompilationParser GetParser(ITokenStream tokenStream) => new(tokenStream);

    protected override IParseTree Parse(VBAConditionalCompilationParser parser) => parser.compilationUnit();
}
