using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class StandardModuleSymbol : TypedSymbol
{
    public StandardModuleSymbol(string name, Uri fileUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.Module, Accessibility.Global, name, fileUri, children)
    {
    }
}
