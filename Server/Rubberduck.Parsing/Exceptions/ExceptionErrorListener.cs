﻿using Antlr4.Runtime;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class ExceptionErrorListener : RubberduckParseErrorListenerBase
    {
        public ExceptionErrorListener(string moduleName, CodeKind codeKind)
        :base(moduleName, codeKind)
        { }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            // adding 1 to line, because line is 0-based, but it's 1-based in the VBE
            throw new SyntaxErrorException(new SyntaxErrorInfo(msg, e, offendingSymbol, line, charPositionInLine + 1, ModuleName, CodeKind));
        }
    }
}
