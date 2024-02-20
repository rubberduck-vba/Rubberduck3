using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBFunctionMember : VBReturningMember
{
    public VBFunctionMember(WorkspaceUri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration = null, TypedSymbol[]? definitions = null, bool isUserDefined = false, VBType? type = null, bool isHidden = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined, type, isHidden)
    {
    }

    public VBFunctionMember(WorkspaceUri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol declaration, TypedSymbol[]? definitions = null, string? asTypeExpression = null, VBType? type = null)
        : base(uri, name, kind, accessibility, declaration, definitions, asTypeExpression, type)
    {
    }
}
