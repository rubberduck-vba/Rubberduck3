using Antlr4.Runtime;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class SyntaxErrorInfo
{
    public SyntaxErrorInfo(string message, RecognitionException innerException, IToken offendingSymbol, int line, int position, string moduleName, CodeKind codeKind)
    {
        Message = message;
        Exception = innerException;
        OffendingSymbol = offendingSymbol;
        LineNumber = line;
        Position = position;
        ModuleName = moduleName;
        CodeKind = codeKind;
    }

    public string Message { get; }
    public RecognitionException Exception { get; }
    public IToken OffendingSymbol { get; }
    public int LineNumber { get; }
    public int Position { get; }

    public string ModuleName { get; }
    public CodeKind CodeKind { get; }
}