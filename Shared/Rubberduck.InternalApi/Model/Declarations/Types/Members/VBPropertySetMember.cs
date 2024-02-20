using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBPropertySetMember : VBTypeMember, IVBProperty
{
    public VBPropertySetMember(WorkspaceUri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, PropertySetSymbol declaration, PropertySetSymbol[]? definitions = null)
        : base(uri, name, kind, accessibility, declaration, definitions)
    {
        ResolvedType = (Declaration as PropertySetSymbol)?.Children?.OfType<ParameterSymbol>().OrderBy(e => e.Range).LastOrDefault()?.ResolvedType;
    }

    public VBPropertySetMember(WorkspaceUri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, PropertySetSymbol? declaration = null, PropertySetSymbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
        ResolvedType = (Declaration as PropertySetSymbol)?.Children?.OfType<ParameterSymbol>().OrderBy(e => e.Range).LastOrDefault()?.ResolvedType;
    }

    public VBType? ResolvedType { get; init; }
}
