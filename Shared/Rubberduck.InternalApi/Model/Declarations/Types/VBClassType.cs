using System;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBClassType : VBMemberOwnerType
{
    public VBClassType(string name, Uri uri, bool isUserDefined = false, IEnumerable<VBTypeMember>? members = null, bool isHidden = false)
        : base(name, uri, isUserDefined, members, isHidden)
    {
    }

    public VBType[] Supertypes { get; init; } = [VbObjectType];
    public VBType[] Subtypes { get; init; } = [];
    public VBTypeMember? DefaultMember { get; init; }
    public bool IsInterface => Subtypes.Length != 0;

    public override VBType[] ConvertsSafelyToTypes => Supertypes.Concat([VbVariantType]).ToArray();
    public override VBObjectValue DefaultValue { get; } = VBObjectValue.Nothing;
}
