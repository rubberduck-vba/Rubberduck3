using System;
using System.Collections.Generic;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBEnumType : VBMemberOwnerType, IVBDeclaredType
{
    public VBEnumType(string name, Uri uri, Symbol declaration, Symbol[]? definitions = null, IEnumerable<VBEnumMember>? members = null, bool isUserDefined = false)
        : base(name, uri, isUserDefined, members)
    {
        Size = 16;
        Declaration = declaration;
        Definitions = definitions;
    }

    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbIntegerType, VbLongType, VbLongLongType, VbVariantType];

    public override object DefaultValue { get; } = 0;
    public Symbol Declaration { get; init; }
    public Symbol[]? Definitions { get; init; }
}
