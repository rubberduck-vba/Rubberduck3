using System;
using System.Collections.Generic;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBUserDefinedType : VBMemberOwnerType, IVBDeclaredType
{
    public VBUserDefinedType(string name, Uri uri, Symbol declaration, Symbol[]? definitions = null, IEnumerable<VBUserDefinedTypeMember>? members = null)
        : base(name, uri, isUserDefined: true, members)
    {
        DefaultValue = new VBUserDefinedTypeValue(this, declaration as TypedSymbol);
        Declaration = declaration;
        Definitions = definitions;
    }

    public override VBType[] ConvertsSafelyToTypes { get; } = [VBVariantType.TypeInfo];
    public override VBTypedValue DefaultValue { get; }

    public override bool CanPassByValue { get; } = false;

    public Symbol Declaration { get; init; }
    public Symbol[]? Definitions { get; init; }
}
