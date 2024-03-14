using Antlr4.Runtime.Tree;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System.Collections.Immutable;

namespace Rubberduck.Parsing._v3.Pipeline;

public record class DocumentParserState : DocumentState
{
    public DocumentParserState(DocumentState original)
        : base(original)
    {
    }

    public DocumentParserState(DocumentParserState original)
        : base(original)
    {
        SyntaxTree = original.SyntaxTree;
    }

    public DocumentParserState(WorkspaceFileUri uri, string text, int version = 1, bool isOpened = false) 
        : base(uri, text, version, isOpened)
    {
    }

    public IParseTree? SyntaxTree { get; init; }

    public DocumentParserState WithSymbol(Symbol module) => this with { Symbol = module };
}
