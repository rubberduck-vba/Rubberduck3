using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class ClassModuleSymbol : TypedSymbol
{
    public ClassModuleSymbol(Instancing instancing, string name, Uri fileUri, IEnumerable<Symbol>? children = null, bool isUserDefined = false)
        : base(RubberduckSymbolKind.Class, instancing == Instancing.Private ? Accessibility.Private : Accessibility.Public, name, fileUri, children)
    {
        ResolvedType = new VBClassType(name, fileUri, isUserDefined: isUserDefined);
        Instancing = instancing;
    }

    public Instancing Instancing { get; init; }
}
