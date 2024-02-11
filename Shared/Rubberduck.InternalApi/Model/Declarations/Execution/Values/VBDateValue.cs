using System;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBDateValue : VBTypedValue, 
    IVBTypedValue<VBDateValue, DateTime>,
    INumericCoercion, 
    IStringCoercion
{
    public VBDateValue(TypedSymbol? declarationSymbol = null)
        : base(VBDateType.TypeInfo, declarationSymbol) { }

    public static VBDateValue MinValue { get; } = new() { Value = new DateTime(100, 01, 01) };
    public static VBDateValue MaxValue { get; } = new() { Value = new DateTime(9999, 12, 31, 23, 59, 59) };
    public static VBDateValue Zero { get; } = new() { Value = new DateTime(1899, 12, 30) };

    public const long MinSerial = -657434;
    public const long MaxSerial = 2958465;
    public static VBDateValue FromSerial(double value) => new() { Value = DateTime.FromOADate(value) };

    public double SerialValue => Value.ToOADate();
    public double AsDouble() => SerialValue;

    public DateTime Value { get; init; } = default;
    public override int Size => 8;

    public VBDoubleValue AsCoercedNumeric(int depth = 0) => new VBDoubleValue(Symbol).WithValue(SerialValue);
    public VBStringValue AsCoercedString(int depth = 0) => new VBStringValue(Symbol).WithValue(Value.ToString("M/dd/yyyy hh:mm:ss tt"));

    public VBDateValue WithValue(DateTime value)
    {
        if (value > MaxValue.Value || value < MinValue.Value)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue.Value}** and **{MaxValue.Value}**.");
        }
        return this with { Value = value };
    }

    public VBDateValue WithValue(double value)
    {
        if (value > MaxSerial || value < MinSerial)
        {
            throw VBRuntimeErrorException.Overflow(Symbol!, $"`{TypeInfo.Name}` values must be between **{MinValue.Value}** and **{MaxValue.Value}**.");
        }
        return this with { Value = Zero.Value.AddDays(value) };
    }
}
