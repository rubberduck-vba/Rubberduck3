using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBStringValue : VBTypedValue, IVBTypedValue<string?>, INumericCoercion, IStringCoercion
{
    public static VBStringValue VBNullString { get; } = new VBStringValue();

    public VBStringValue(TypedSymbol? declarationSymbol = null) 
        : base(VBStringType.TypeInfo, declarationSymbol) { }

    public int? FixedWidth { get; init; }
    public double? AsCoercedNumeric() => double.TryParse(Value, out var numericValue) ? numericValue : null;
    public string? AsCoercedString() => string.Empty;
    public string? Value { get; init; } = default;
    public string? DefaultValue { get; } = default;

    public VBStringValue WithValue(string? value) => this with { Value = value };

    public double? AsCoercedNumeric(int depth = 0)
    {
        if (double.TryParse(Value, out var coerced))
        {
            return coerced;
        }

        throw VBRuntimeErrorException.TypeMismatch(Symbol!, $"Numeric coercion failed to coerce \"{Value}\" to a numeric value.");
    }

    public string? AsCoercedString(int depth = 0) => Value;
}
