using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBUserDefinedTypeValue : VBTypedValue, 
    IVBTypedValue<VBUserDefinedTypeValue, Guid>
{
    public VBUserDefinedTypeValue(VBUserDefinedType type, TypedSymbol? symbol = null) 
        : base(type, symbol)
    {
        Value = Guid.NewGuid();
    }

    public Guid Value { get; }

    public override int Size => ((IVBMemberOwnerType)TypeInfo).Members.OfType<VBUserDefinedTypeMember>()
        .Sum(member => ((TypedSymbol)member.Declaration!).ResolvedType!.DefaultValue.Size);
}
