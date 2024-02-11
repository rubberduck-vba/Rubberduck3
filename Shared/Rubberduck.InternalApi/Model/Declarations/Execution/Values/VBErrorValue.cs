using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBErrorValue : VBTypedValue, 
    IVBTypedValue<VBErrorValue, int>
{
    public VBErrorValue(TypedSymbol? symbol = null, int value = 0) : base(VBErrorType.TypeInfo, symbol)
    {
        Value = value;
    }

    public static VBErrorValue None => (VBErrorValue)VBErrorType.TypeInfo.DefaultValue;
    public static VBErrorValue MinValue => None;
    public static VBErrorValue MaxValue => new VBErrorValue { Value = ushort.MaxValue };
    
    public int Value { get; init; }
    public override int Size => sizeof(int);

    public VBErrorValue WithValue(int value) => this with { Value = value };
    public override string ToString() => $"Error {Value}";
}