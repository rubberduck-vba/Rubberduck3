using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongPtrValue : VBNumericTypedValue, 
    IVBTypedValue<VBLongPtrValue, long>, 
    INumericValue<VBLongPtrValue>
{
    public VBLongPtrValue(TypedSymbol? declarationSymbol = null)
        : base(VBLongPtrType.TypeInfo, declarationSymbol) { }

    public static bool Is64Bit { get; set; } = true;

    public static VBLongPtrValue MinValue { get; } = new VBLongPtrValue().WithValue(Is64Bit ? long.MinValue : int.MinValue);
    public static VBLongPtrValue MaxValue { get; } = new VBLongPtrValue().WithValue(Is64Bit ? long.MaxValue : int.MaxValue);
    public static VBLongPtrValue Zero { get; } = new VBLongPtrValue().WithValue(0);

    VBLongPtrValue INumericValue<VBLongPtrValue>.MinValue => MinValue;
    VBLongPtrValue INumericValue<VBLongPtrValue>.Zero => Zero;
    VBLongPtrValue INumericValue<VBLongPtrValue>.MaxValue => MaxValue;

    public long Value { get; init; } = default;
    public VBLongPtrValue DefaultValue { get; } = Zero;
    public long NominalValue => Value;

    public override int Size => Is64Bit ? sizeof(long) : sizeof(int);
    protected override double State => Value;

    public VBLongPtrValue WithValue(double value) => WithValue(value, Is64Bit ? VBLongLongType.TypeInfo : VBLongType.TypeInfo);
    public VBLongPtrValue WithValue(double value, VBType ptrType)
    {
        if (ptrType is VBLongType)
        {
            if (value > int.MaxValue || value < int.MinValue)
            {
                throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{int.MinValue:N}** and **{int.MaxValue:N}**.");
            }

            return this with { Value = (int)value };
        }

        if (ptrType is VBLongLongType)
        {
            if (value > long.MaxValue || value < long.MinValue)
            {
                throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{long.MinValue:N}** and **{long.MaxValue:N}**.");
            }

            return this with { Value = (long)value };
        }

        // this would be a bug in RD3, not in the user code; if thrown, this exception will bubble unhandled through the execution context.
        throw new ArgumentOutOfRangeException(nameof(ptrType));
    }
}
