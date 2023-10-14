﻿using System;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract
{
    public abstract class TokenStreamParserBase<TParser> : ITokenStreamParser
        where TParser : Parser
    {
        //protected static ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IParsePassErrorListenerFactory _sllErrorListenerFactory;
        private readonly IParsePassErrorListenerFactory _llErrorListenerFactory;

        public TokenStreamParserBase(IParsePassErrorListenerFactory sllErrorListenerFactory,
            IParsePassErrorListenerFactory llErrorListenerFactory)
        {
            _sllErrorListenerFactory = sllErrorListenerFactory;
            _llErrorListenerFactory = llErrorListenerFactory;
        }

        protected IParseTree Parse(ITokenStream tokenStream, PredictionMode predictionMode, IParserErrorListener errorListener)
        {
            var parser = GetParser(tokenStream);
            parser.Interpreter.PredictionMode = predictionMode;
            if (errorListener != null)
            {
                parser.AddErrorListener(errorListener);
            }
            return Parse(parser);
        }
        protected abstract TParser GetParser(ITokenStream tokenStream);
        protected abstract IParseTree Parse(TParser parser);
        public IParseTree Parse(string moduleName, string projectId, CommonTokenStream tokenStream, CancellationToken token, CodeKind codeKind = CodeKind.RubberduckEditorModule, ParserMode parserMode = ParserMode.FallBackSllToLl)
        {
            switch (parserMode)
            {
                case ParserMode.FallBackSllToLl:
                    return ParseWithFallBack(moduleName, tokenStream, codeKind);
                case ParserMode.LlOnly:
                    return ParseLl(moduleName, tokenStream, codeKind);
                case ParserMode.SllOnly:
                    return ParseSll(moduleName, tokenStream, codeKind);
                default:
                    throw new ArgumentException(nameof(parserMode));
            }
        }

        private IParseTree ParseWithFallBack(string moduleName, CommonTokenStream tokenStream, CodeKind codeKind)
        {
            try
            {
                return ParseSll(moduleName, tokenStream, codeKind);
            }
            catch (ParsePassSyntaxErrorException syntaxErrorException)
            {
                var message = $"SLL mode failed while parsing the {codeKind} version of module {moduleName} at symbol {syntaxErrorException.OffendingSymbol.Text} at L{syntaxErrorException.LineNumber}C{syntaxErrorException.Position}. Retrying using LL.";
                LogAndReset(tokenStream, message, syntaxErrorException);
                return ParseLl(moduleName, tokenStream, codeKind);
            }
            catch (Exception exception)
            {
                var message = $"SLL mode failed while parsing the {codeKind} version of module {moduleName}. Retrying using LL.";
                LogAndReset(tokenStream, message, exception);
                return ParseLl(moduleName, tokenStream, codeKind);
            }
        }

        //This method is virtual only because a CommonTokenStream cannot be mocked in tests
        //and there is no interface for it or a base class that has the Reset member.
        protected virtual void LogAndReset(CommonTokenStream tokenStream, string logWarnMessage, Exception exception)
        {
            //Logger.Warn(logWarnMessage);
            //Logger.Debug(exception);
            tokenStream.Reset();
        }

        private IParseTree ParseLl(string moduleName, ITokenStream tokenStream, CodeKind codeKind)
        {
            var errorListener = _llErrorListenerFactory.Create(moduleName, codeKind);
            var tree = Parse(tokenStream, PredictionMode.Ll, errorListener);
            if (errorListener.HasPostponedException(out var exception))
            {
                throw exception;
            }
            return tree;
        }

        private IParseTree ParseSll(string moduleName, ITokenStream tokenStream, CodeKind codeKind)
        {
            var errorListener = _sllErrorListenerFactory.Create(moduleName, codeKind);
            var tree = Parse(tokenStream, PredictionMode.Sll, errorListener);
            if (errorListener.HasPostponedException(out var exception))
            {
                throw exception;
            }
            return tree;
        }
    }
}
