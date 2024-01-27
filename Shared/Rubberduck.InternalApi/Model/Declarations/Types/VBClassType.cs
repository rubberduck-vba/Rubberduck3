﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBClassType : VBMemberOwnerType
{
    public VBClassType(string name, Uri uri, bool isUserDefined = false, IEnumerable<VBTypeMember>? members = null, bool isHidden = false)
        : base(name, uri, isUserDefined, members, isHidden)
    {
    }

    public VBType[] Supertypes { get; init; } = [];
    public VBType[] Subtypes { get; init; } = [];
    public VBReturningMember? DefaultMember { get; init; }
    public bool IsInterface => Subtypes.Length != 0;

    public override VBType[] ConvertsSafelyToTypes => Supertypes.Concat([VbVariantType, VbObjectType]).ToArray();
    public override object? DefaultValue { get; } = VBObjectType.Nothing;
}

/// <summary>
/// An object type that can be iterated in a <c>For Each...Next</c> loop.
/// </summary>
public record class VBCollectionType : VBClassType, IEnumerableType
{
    public VBCollectionType(string name, Uri uri, bool isUserDefined = false, IEnumerable<VBTypeMember>? members = null, VBReturningMember? newEnumMember = null) 
        : base(name, uri, isUserDefined, members)
    {
        NewEnumMember = newEnumMember;
    }

    public bool IsArray { get; } = false;
    public VBReturningMember? NewEnumMember { get; init; }
}
