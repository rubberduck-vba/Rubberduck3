using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBStdModuleType : VBMemberOwnerType
{
    public VBStdModuleType(string name, Uri uri, bool isUserDefined = true, IEnumerable<VBTypeMember>? members = null, bool isHidden = false) 
        : base(name, uri, isUserDefined, members, isHidden)
    {
    }

    public override object? DefaultValue { get; } = new();
}