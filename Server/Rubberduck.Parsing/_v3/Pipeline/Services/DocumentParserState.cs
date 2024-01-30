using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.Parsing._v3.Pipeline;

public record class DocumentParserState : DocumentState
{
    public DocumentParserState(DocumentState original)
        : base(original)
    {
    }

    public IParseTree? ParseTree { get; init; }

    public DocumentParserState WithParseTree(IParseTree tree) => this with { ParseTree = tree };
    public new DocumentParserState WithHierarchicalSymbols(Symbol symbol) => this with { Symbols = symbol };
}
