using System;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDateValue : VBTypedValue, IVBTypedValue<DateTime>, INumericValue, INumericCoercion, IStringCoercion
{
    public static VBDateValue Zero = new() { Value = new DateTime(1899, 12, 30) };
    public static VBDateValue FromSerial(double value) => new() { Value = Zero.Value.AddDays(value) };

    public double AsDouble() => SerialValue;

    public VBTypedValue WithValue(double value) => FromSerial(value);

    public VBDateValue(TypedSymbol? declarationSymbol = null) 
        : base(VBDateType.TypeInfo, declarationSymbol) { }

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString("M/dd/yyyy hh:mm:ss tt");
    public DateTime Value { get; init; } = default;
    public DateTime DefaultValue { get; } = default;

    public double SerialValue => (Value - Zero.Value).TotalDays;
}
