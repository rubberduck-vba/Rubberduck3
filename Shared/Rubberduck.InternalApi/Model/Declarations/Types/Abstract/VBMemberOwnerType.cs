using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public abstract record class VBMemberOwnerType : VBType<object?>, IVBMemberOwnerType
{
    public VBMemberOwnerType(string name, Uri uri, bool isUserDefined = false, IEnumerable<VBTypeMember>? members = null, bool isHidden = false)
        : base(name, isUserDefined, isHidden)
    {
        Uri = uri;
        Members = members?.ToImmutableArray() ?? [];
    }

    public Uri Uri { get; init; }
    public ImmutableArray<VBTypeMember> Members { get; init; }
    public VBTypeMember? this[string name] => Members.SingleOrDefault(e => e.Name == name 
        && (e is not IVBProperty || e is VBPropertyGetMember || e is VBPropertyLetMember || e is VBPropertySetMember));

    public VBMemberOwnerType WithMembers(IEnumerable<VBTypeMember> members) => this with { Members = members.ToImmutableArray() };
}
