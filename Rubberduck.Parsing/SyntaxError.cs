using Antlr4.Runtime;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing
{
    public class SyntaxError
    {
        public SyntaxError(string moduleName, string message, IToken token, VBABaseParserRuleContext context)
        {
            ModuleName = moduleName;
            Message = message;
            OffendingToken = token;
            Context = context;
        }

        public string Message { get; }
        public string ModuleName { get; }
        public IToken OffendingToken { get; }
        public VBABaseParserRuleContext Context { get; }
    }
}
