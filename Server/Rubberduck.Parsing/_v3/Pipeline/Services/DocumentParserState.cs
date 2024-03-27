using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.Parsing._v3.Pipeline;

public record class DocumentParserState : CodeDocumentState
{
    public DocumentParserState(CodeDocumentState original)
        : base(original)
    {
    }

    public DocumentParserState(DocumentParserState original)
        : base(original)
    {
        SyntaxTree = original.SyntaxTree;
    }

    public DocumentParserState(WorkspaceFileUri uri, SupportedLanguage language, string text, int version = 1, bool isOpened = false) 
        : base(uri, language, text, version, isOpened)
    {
    }

    public IParseTree? SyntaxTree { get; init; }

    public new DocumentParserState WithSymbol(Symbol module) => this with { Symbol = module };
}
