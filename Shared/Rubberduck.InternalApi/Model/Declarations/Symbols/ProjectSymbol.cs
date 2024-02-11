using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class ProjectSymbol : Symbol
{
    public ProjectSymbol(string name, Uri workspaceUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.Project, name, workspaceUri, Accessibility.Global, children)
    {
    }
}
