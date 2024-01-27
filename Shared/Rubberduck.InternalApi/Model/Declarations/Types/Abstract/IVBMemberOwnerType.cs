using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public interface IVBMemberOwnerType
{
    ImmutableArray<VBTypeMember> Members { get; init; }
    VBMemberOwnerType WithMembers(IEnumerable<VBTypeMember> members);
}

public abstract record class VBTypeMember 
{
    /// <summary>
    /// Creates a new type member associated with a symbol.
    /// </summary>
    protected VBTypeMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol declaration, Symbol[]? definitions = null)
        : this(uri, name, kind, accessibility, declaration, definitions, isUserDefined: true) { }

    /// <summary>
    /// Creates a new type member without a symbol (non-user code).
    /// </summary>
    protected VBTypeMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false, bool isHidden = false)
    {
        Uri = uri;
        IsUserDefined = isUserDefined;
        Name = name;
        Kind = kind;
        Accessibility = accessibility;
        Declaration = declaration;
        Definitions = definitions ?? [];
        IsHidden = isHidden;
    }

    public Uri Uri { get; init; }
    public bool IsUserDefined { get; init; }
    public bool IsHidden { get; init; }
    public string Name { get; init; }
    public RubberduckSymbolKind Kind { get; init; }
    public Accessibility Accessibility { get; init; }

    public Symbol? Declaration { get; init; }
    public Symbol[] Definitions { get; init; }

    public VBTypeMember WithUri(Uri uri) => this with { Uri = uri };
    public VBTypeMember WithName(string name) => this with { Name = name };
    public VBTypeMember WithSymbolKind(RubberduckSymbolKind kind) => this with { Kind = Kind };
    public VBTypeMember WithAccessibility(Accessibility accessibility) => this with { Accessibility = accessibility };
    public VBTypeMember WithDeclaration(Symbol declaration) => this with {  Declaration = declaration };
    public VBTypeMember WithDefinitions(Symbol[] definitions) => this with { Definitions = definitions };
}
