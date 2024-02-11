using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBBooleanValue : VBTypedValue, IVBTypedValue<VBBooleanValue, bool>
{
    public VBBooleanValue(TypedSymbol? declarationSymbol = null)
        : base(VBBooleanType.TypeInfo, declarationSymbol) { }

    public static VBBooleanValue False { get; } = new VBBooleanValue { Value = false };
    public static VBBooleanValue True { get; } = new VBBooleanValue { Value = true };

    public bool Value { get; init; } = default;
    public override int Size { get; } = 16;

    public VBDoubleValue AsCoercedNumeric(int depth = 0) => new VBDoubleValue(Symbol).WithValue(-1 * Convert.ToDouble(Value));
    public VBStringValue AsCoercedString(int depth = 0) => new VBStringValue(Symbol).WithValue(ToString());

    public VBBooleanValue WithValue(bool value) => this with { Value = value };
    public VBBooleanValue WithValue(int value) => this with { Value = value != 0 };

    public override string ToString() => Value ? Tokens.True : Tokens.False;
}
