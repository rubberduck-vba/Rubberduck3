using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBErrorValue : VBTypedValue, 
    IVBTypedValue<VBErrorValue, int>
{
    public VBErrorValue(TypedSymbol? symbol = null, int value = 0) : base(VBType.VbErrorType, symbol)
    {
        Value = value;
    }

    public static VBErrorValue None { get; } = new VBErrorValue().DefaultValue;
    public static VBErrorValue MinValue { get; } = None;
    public static VBErrorValue MaxValue { get; } = new VBErrorValue().WithValue(ushort.MaxValue);

    public int Value { get; init; }
    public VBErrorValue DefaultValue { get; init; } = None;
    public int NominalValue => Value;

    public override int Size => sizeof(int);

    public VBErrorValue WithValue(int value) => this with { Value = value };
    public override string ToString() => $"Error {Value}";
}