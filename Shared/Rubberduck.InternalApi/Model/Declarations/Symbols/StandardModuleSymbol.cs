using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class StandardModuleSymbol : Symbol
{
    public StandardModuleSymbol(string name, Uri fileUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.Module, name, fileUri, Accessibility.Global, children)
    {
    }
}
