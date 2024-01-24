﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public class ParseResult
{
    public IParseTree Tree { get; init; } = null!;
    public ITokenStream TokenStream { get; init; } = null!;
    public LogicalLineStore? LogicalLines { get; init; } = null;
    public IEnumerable<IParseTreeListener> Listeners { get; init; } = [];
}

public interface IParser<TContent>
{
    ParseResult Parse(WorkspaceFileUri uri, TContent content, CancellationToken token, CodeKind codeKind = CodeKind.RubberduckEditorModule, ParserMode parserMode = ParserMode.FallBackSllToLl, IEnumerable<IParseTreeListener>? parseListeners = null);
}

public interface IStringParser : IParser<string> { }

public interface ITextReaderParser : IParser<TextReader> { }
