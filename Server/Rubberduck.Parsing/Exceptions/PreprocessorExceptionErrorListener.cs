﻿using Antlr4.Runtime;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class PreprocessorExceptionErrorListener : ParsePassExceptionErrorListener
    {
        public PreprocessorExceptionErrorListener(string moduleName, CodeKind codeKind)
        :base(moduleName, codeKind)
        { }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            // adding 1 to line, because line is 0-based, but it's 1-based in the VBE
            throw new PreprocessorSyntaxErrorException(msg, e, offendingSymbol, line, charPositionInLine + 1, ModuleName, CodeKind);
        }
    }
}
