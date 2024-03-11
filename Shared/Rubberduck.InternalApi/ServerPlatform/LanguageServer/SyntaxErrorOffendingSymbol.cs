using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public record class SyntaxErrorOffendingSymbol : Symbol
{
    public SyntaxErrorOffendingSymbol(string name, WorkspaceUri? parentUri = null)
        : base(RubberduckSymbolKind.UnknownSymbol, name, parentUri, Model.Accessibility.Undefined, [])
    {
    }

    public int TokenIndex { get; init; }
}