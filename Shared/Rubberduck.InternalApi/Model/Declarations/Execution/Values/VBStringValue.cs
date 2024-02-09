using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBStringValue : VBTypedValue,
    IVBTypedValue<VBStringValue, string>, 
    INumericCoercion, 
    IStringCoercion
{
    public VBStringValue(TypedSymbol? symbol = null)
        : base(VBStringType.TypeInfo, symbol) { }

    public static VBStringValue VBNullString { get; } = new VBStringValue();
    public static VBStringValue ZeroLengthString { get; } = new VBStringValue().WithValue(string.Empty);

    public string? Value { get; init; } = default;
    public VBStringValue DefaultValue { get; } = VBNullString;
    public virtual string NominalValue => Value ?? string.Empty;

    public virtual int Length => NominalValue.Length;

    public override int Size => Value is null ? 0 : 2 * Length + 2;

    public VBStringValue AsCoercedString(int depth = 0) => new VBStringValue(Symbol).WithValue(NominalValue);
    public VBDoubleValue AsCoercedNumeric(int depth = 0)
    {
        if (double.TryParse(Value, out var coerced))
        {
            return new VBDoubleValue(Symbol).WithValue(coerced);
        }

        throw VBRuntimeErrorException.TypeMismatch(Symbol!, $"Numeric coercion failed to coerce \"{Value}\" to a numeric value.");
    }

    public virtual VBStringValue WithValue(string? value) => this with { Value = value };
}
