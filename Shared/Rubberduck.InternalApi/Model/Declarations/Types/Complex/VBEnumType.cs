using System;
using System.Collections.Generic;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Types.Members;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBEnumType : VBMemberOwnerType, IVBDeclaredType
{
    public VBEnumType(string name, Uri uri, Symbol declaration, Symbol[]? definitions = null, IEnumerable<VBEnumMember>? members = null, bool isUserDefined = false)
        : base(name, uri, isUserDefined, members)
    {
        Declaration = declaration;
        Definitions = definitions;
    }

    public override VBType[] ConvertsSafelyToTypes =>
    [
        VBIntegerType.TypeInfo,
        VBLongLongType.TypeInfo,
        VBDecimalType.TypeInfo,
        VBCurrencyType.TypeInfo,
        VBSingleType.TypeInfo,
        VBDoubleType.TypeInfo,
        VBStringType.TypeInfo,
        VBVariantType.TypeInfo
    ];

    public override VBTypedValue DefaultValue { get; } = VBLongValue.Zero;
    public Symbol Declaration { get; init; }
    public Symbol[]? Definitions { get; init; }
}
