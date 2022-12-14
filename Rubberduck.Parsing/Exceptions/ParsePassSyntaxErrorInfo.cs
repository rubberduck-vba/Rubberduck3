using Antlr4.Runtime;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class ParsePassSyntaxErrorInfo : SyntaxErrorInfo
    {
        public ParsePassSyntaxErrorInfo(string message, RecognitionException innerException, IToken offendingSymbol, int line, int position, string moduleName, CodeKind codeKind)
        :base(message, innerException, offendingSymbol, line, position, codeKind){
            ModuleName = moduleName;
        }

        public string ModuleName { get; }
    }
}