using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers;

public class VBATokenStreamParser : TokenStreamParserBase<VBAParser>
{
    public VBATokenStreamParser(ISyntaxErrorMessageService errorMessageService, ILogger<ITokenStreamParser> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(errorMessageService, logger, settingsProvider, performance)
    {
    }

    protected override VBAParser GetParser(ITokenStream tokenStream) => new(tokenStream);

    protected override IParseTree Parse(VBAParser parser) => parser.startRule();
}

public class VBAMemberTokenStreamParser : TokenStreamParserBase<VBAMemberParser>
{
    public VBAMemberTokenStreamParser(ISyntaxErrorMessageService errorMessageService, ILogger<ITokenStreamParser> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(errorMessageService, logger, settingsProvider, performance)
    {
    }

    protected override VBAMemberParser GetParser(ITokenStream tokenStream) => new(tokenStream);

    protected override IParseTree Parse(VBAMemberParser parser) => parser.startRule();
}
