using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBNullValue : VBTypedValue, IVBTypedValue<VBNullValue, IntPtr>
{
    public static VBNullValue Null { get; } = new VBNullValue();
    public VBNullValue(TypedSymbol? symbol = null) : base(VBNullType.TypeInfo, symbol) { }

    public IntPtr Value { get; } = IntPtr.Zero;
    public override int Size => 0;
}
