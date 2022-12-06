using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using Rubberduck.Parsing.Grammar;
using System;
using System.Collections.Generic;

namespace Rubberduck.Parsing
{
    public class SyntaxErrorListener : IParserErrorListener
    {
        private readonly IList<SyntaxError> _syntaxErrors = new List<SyntaxError>();
        private readonly string _moduleName;

        public IEnumerable<SyntaxError> SyntaxErrors => _syntaxErrors;

        public SyntaxErrorListener(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void ReportAmbiguity([NotNull] Parser recognizer, [NotNull] DFA dfa, int startIndex, int stopIndex, bool exact, [Nullable] BitSet ambigAlts, [NotNull] ATNConfigSet configs)
        {
            //throw new NotImplementedException();
        }

        public void ReportAttemptingFullContext([NotNull] Parser recognizer, [NotNull] DFA dfa, int startIndex, int stopIndex, [Nullable] BitSet conflictingAlts, [NotNull] SimulatorState conflictState)
        {
            //throw new NotImplementedException();
        }

        public void ReportContextSensitivity([NotNull] Parser recognizer, [NotNull] DFA dfa, int startIndex, int stopIndex, int prediction, [NotNull] SimulatorState acceptState)
        {
            //throw new NotImplementedException();
        }

        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            _syntaxErrors.Add(new SyntaxError(_moduleName, msg, offendingSymbol, e?.Context as VBABaseParserRuleContext));
        }
    }
}
