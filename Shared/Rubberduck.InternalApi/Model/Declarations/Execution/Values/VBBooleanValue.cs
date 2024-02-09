using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBBooleanValue : VBTypedValue,
    IVBTypedValue<VBBooleanValue, bool, short>
{
    public VBBooleanValue(TypedSymbol? declarationSymbol = null)
        : base(VBBooleanType.TypeInfo, declarationSymbol) { }

    public static VBBooleanValue False { get; } = new VBBooleanValue().WithValue(false);
    public static VBBooleanValue True { get; } = new VBBooleanValue().WithValue(true);

    public bool Value { get; init; } = default;
    public VBBooleanValue DefaultValue { get; } = False;
    public short NominalValue => Convert.ToInt16(AsCoercedNumeric());

    public override int Size { get; } = 16;

    public VBDoubleValue AsCoercedNumeric(int depth = 0) => new VBDoubleValue(Symbol).WithValue(Convert.ToDouble(NominalValue));
    public VBStringValue AsCoercedString(int depth = 0) => new VBStringValue(Symbol).WithValue(ToString());

    public VBBooleanValue WithValue(bool value) => this with { Value = value };

    public override string ToString() => Value ? Tokens.True : Tokens.False;
}
