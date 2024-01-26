using System;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBClassType : VBMemberOwnerType
    {
        public VBClassType(string name, Uri uri, bool isUserDefined = false, IEnumerable<VBTypeMember>? members = null)
            : base(name, uri, isUserDefined, members) 
        {
        }

        public VBType[] Supertypes { get; init; } = [];
        public VBType[] Subtypes { get; init; } = [];
        public bool IsInterface => Subtypes.Length != 0;

        public override VBType[] ConvertsSafelyToTypes => Supertypes.Concat([VbVariantType]).ToArray();
        public override object? DefaultValue { get; } = VBObjectType.Nothing;
    }
}
