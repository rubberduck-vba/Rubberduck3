using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class StandardModuleSymbol : TypedSymbol
{
    public StandardModuleSymbol(string name, WorkspaceUri fileUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.Module, Accessibility.Global, name, fileUri, children)
    {
        ResolvedType = new VBStdModuleType(name, fileUri, members: children.OfType<ProcedureSymbol>().Select(e => new VBProcedureMember(e.Uri, e.Name, (RubberduckSymbolKind)e.Kind, Accessibility.Undefined, e, isUserDefined: true)));
    }
}
