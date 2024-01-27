using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBPropertySetMember : VBTypeMember
{
    public VBPropertySetMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol declaration, Symbol[]? definitions = null)
        : base(uri, name, kind, accessibility, declaration, definitions)
    {
    }

    public VBPropertySetMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
    }
}
