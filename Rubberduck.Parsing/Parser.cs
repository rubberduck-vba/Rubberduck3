using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;
using System;
using System.Collections.Generic;

namespace Rubberduck.Parsing
{
    public abstract class Parser<TContent> : IParser<TContent>
    {
        private readonly ICommonTokenStreamProvider<TContent> _provider;

        protected Parser(ICommonTokenStreamProvider<TContent> provider)
        {
            _provider = provider;
        }

        protected abstract CommonTokenStream GetCommonTokenStream(ICommonTokenStreamProvider<TContent> provider, TContent content);

        public ParserResult Parse(string moduleName, TContent content, Func<VBAParser, IParseTree> startRule, IEnumerable<IParseTreeListener> parseListeners, ParserMode mode = ParserMode.Default)
        {
            var stream = GetCommonTokenStream(_provider, content);
            return Parse(moduleName, stream, startRule, mode, parseListeners);
        }

        protected virtual ParserResult Parse(string moduleName, CommonTokenStream stream, Func<VBAParser, IParseTree> startRule, ParserMode mode, IEnumerable<IParseTreeListener> parseListeners)
        {
            var errors = new SyntaxErrorListener(moduleName);
            var parser = new VBAParser(stream);
            parser.TrimParseTree = true;
            parser.ErrorHandler = new DefaultErrorStrategy();
            parser.AddErrorListener(errors);

            foreach (var listener in parseListeners)
            {
                parser.AddParseListener(listener);
            }

            parser.Interpreter.PredictionMode = PredictionMode.Sll;

            var tree = startRule.Invoke(parser);
            return new ParserResult(tree, errors.SyntaxErrors, new TokenStreamRewriter(stream));
        }
    }
}
