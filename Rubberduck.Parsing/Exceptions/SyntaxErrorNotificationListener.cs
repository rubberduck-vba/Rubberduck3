using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class SyntaxErrorNotificationListener : RubberduckParseErrorListenerBase
    {
        private readonly IList<SyntaxErrorInfo> _errors = new List<SyntaxErrorInfo>();

        public SyntaxErrorNotificationListener(string moduleName, CodeKind codeKind) 
        :base(moduleName, codeKind) { }

        public IEnumerable<SyntaxErrorInfo> Errors => _errors;

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            _errors.Add(new SyntaxErrorInfo(msg, e, offendingSymbol, line, charPositionInLine, ModuleName, CodeKind));
        }
    }
}