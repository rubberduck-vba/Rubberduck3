using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Members;

public record class VBEnumMember : VBTypeMember
{
    public VBEnumMember(WorkspaceUri uri, string name, EnumMemberSymbol declaration, Symbol[]? definitions = null)
        : base(uri, name, RubberduckSymbolKind.EnumMember, Accessibility.Public, declaration, definitions)
    {
    }

    public VBEnumMember(WorkspaceUri uri, string name, EnumMemberSymbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, RubberduckSymbolKind.EnumMember, Accessibility.Public, declaration, definitions, isUserDefined)
    {
    }

    public VBLongValue? ResolvedUnderlyingValue { get; init; }
    public VBEnumMember ResolveUnderlyingValue(VBLongValue? value) => this with { ResolvedUnderlyingValue = value };
}
