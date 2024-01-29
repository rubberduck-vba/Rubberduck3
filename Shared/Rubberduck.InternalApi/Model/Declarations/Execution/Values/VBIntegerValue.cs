﻿using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBIntegerValue : VBTypedValue, IVBTypedValue<short>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBIntegerValue(TypedSymbol? declarationSymbol = null) 
        : base(VBIntegerType.TypeInfo, declarationSymbol) { }

    public short Value { get; init; } = default;
    public short DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;

    public VBTypedValue WithValue(double value) => this with { Value = (short)value };
}
