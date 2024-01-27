using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public abstract record class VBReturningMember : VBTypeMember
{
    public VBReturningMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol declaration, Symbol[]? definitions = null, string? asTypeExpression = null, VBType? type = null) 
        : base(uri, name, kind, accessibility, declaration, definitions)
    {
        AsTypeExpression = asTypeExpression;
        ResolvedType = type;
    }

    public VBReturningMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false, VBType? type = null, bool isHidden = false) 
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined, isHidden)
    {
        ResolvedType = type;
    }

    public string? AsTypeExpression { get; init; }
    public VBType? ResolvedType { get; init; }

    public VBReturningMember WithResolvedType(VBType type) => this with { ResolvedType = type };
}
