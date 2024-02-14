using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBUserDefinedTypeMember : VBTypeMember
{
    public VBUserDefinedTypeMember(WorkspaceUri uri, string name, Symbol declaration, Symbol[]? definitions = null)
        : base(uri, name, RubberduckSymbolKind.Field, Accessibility.Public, declaration, definitions)
    {
    }

    public VBUserDefinedTypeMember(WorkspaceUri uri, string name, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, RubberduckSymbolKind.Field, Accessibility.Public, declaration, definitions, isUserDefined)
    {
    }
}
