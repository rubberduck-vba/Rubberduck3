using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

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

    public DocumentParserState WithSyntaxTree(IParseTree tree) => this with { SyntaxTree = tree };
    public new DocumentParserState WithSymbol(Symbol module) => this with { Symbol = module };
}
