using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongPtrValue : VBTypedValue, IVBTypedValue<long>, INumericCoercion, IStringCoercion
{
    public VBLongPtrValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongPtrType.TypeInfo, declarationSymbol) { }

    public double? AsCoercedNumeric(int depth = 0) => Value;
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public long Value { get; init; } = default;
    public long DefaultValue { get; } = default;

    public VBTypedValue WithValue(double value, VBType ptrSize)
    {
        if (ptrSize is VBLongType)
        {
            if (value > int.MaxValue || value < int.MinValue)
            {
                throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{int.MinValue:N}** and **{int.MaxValue:N}**.");
            }

            return this with { Value = (int)value };
        }

        if (ptrSize is VBLongLongType)
        {
            if (value > long.MaxValue || value < long.MinValue)
            {
                throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{long.MinValue:N}** and **{long.MaxValue:N}**.");
            }

            return this with { Value = (long)value };
        }

        // this would be a bug in RD3, not in the user code; if thrown, this exception will bubble unhandled through the execution context.
        throw new ArgumentOutOfRangeException(nameof(ptrSize));
    }
}
