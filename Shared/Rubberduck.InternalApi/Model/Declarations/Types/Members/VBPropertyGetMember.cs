using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public interface IVBProperty
{
    string Name { get; }
    VBType? ResolvedType { get; } 
}

public record class VBPropertyGetMember : VBReturningMember, IVBProperty
{
    public VBPropertyGetMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, PropertyGetSymbol declaration, PropertyGetSymbol[]? definitions = null, string? asTypeExpression = null, VBType? type = null)
        : base(uri, name, kind, accessibility, declaration, definitions, asTypeExpression, type)
    {
    }

    public VBPropertyGetMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, PropertyGetSymbol? declaration = null, PropertyGetSymbol[]? definitions = null, bool isUserDefined = false, VBType? type = null, bool isHidden = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined, type, isHidden)
    {
    }
}
