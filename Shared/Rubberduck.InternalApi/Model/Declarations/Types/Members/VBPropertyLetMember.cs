using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBPropertyLetMember : VBProcedureMember, IVBProperty
{
    public VBPropertyLetMember(WorkspaceUri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, PropertyLetSymbol declaration, PropertyLetSymbol[]? definitions = null)
        : base(uri, name, kind, accessibility, declaration, definitions)
    {
        ResolvedType = (Declaration as PropertyLetSymbol)?.Children?.OfType<ParameterSymbol>().OrderBy(e => e.Range).LastOrDefault()?.ResolvedType;
    }

    public VBPropertyLetMember(WorkspaceUri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, PropertyLetSymbol? declaration = null, PropertyLetSymbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
        ResolvedType = (Declaration as PropertyLetSymbol)?.Children?.OfType<ParameterSymbol>().OrderBy(e => e.Range).LastOrDefault()?.ResolvedType;
    }

    public VBType? ResolvedType { get; init; }
}
