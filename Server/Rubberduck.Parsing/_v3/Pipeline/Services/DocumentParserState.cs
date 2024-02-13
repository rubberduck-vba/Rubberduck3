using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.Parsing._v3.Pipeline;

public record class DocumentParserState : SourceFileDocumentState
{
    public DocumentParserState(SourceFileDocumentState original)
        : base(original)
    {
    }

    public IParseTree? SyntaxTree { get; init; }

    public DocumentParserState WithSyntaxTree(IParseTree tree) => this with { SyntaxTree = tree };
}
